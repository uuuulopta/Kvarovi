namespace Kvarovi.Utils;

using Microsoft.Extensions.Logging.Abstractions;

public class Retry
{
    
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="validateResult"></param>
    /// <param name="maxRetries"></param>
    /// <param name="maxDelayMilliseconds"></param>
    /// <param name="delayMilliseconds"></param>
    /// <param name="onEachFail"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    public static async Task<T> DoAsync<T>(Func<Task<T>> action, Func<T, bool> validateResult = null,
        int maxRetries = 10, int maxDelayMilliseconds = 2000, int delayMilliseconds = 200,
        Action<int,Exception>? onEachFail = null
)

    {
        var backoff = new ExponentialBackoff(delayMilliseconds, maxDelayMilliseconds);

        var exceptions = new List<Exception>();

        int retryCount = 0;
        for (var retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                retryCount = retry;
                var result = await action()
                    .ConfigureAwait(false);
                var isValid = validateResult?.Invoke(result);
                if (isValid.HasValue && isValid.Value)
                    return result;
            }
            catch (Exception ex)
            {
                onEachFail?.Invoke(retryCount,ex);
                exceptions.Add(ex);
                await backoff.Delay()
                    .ConfigureAwait(false);
            }
        }

        throw new AggregateException(exceptions);
    }

    private struct ExponentialBackoff
    {
        private readonly int _delayMilliseconds;
        private readonly int _maxDelayMilliseconds;
        private int _retries;
        private int _pow;

        public ExponentialBackoff(int delayMilliseconds, int maxDelayMilliseconds)
        {
            _delayMilliseconds = delayMilliseconds;
            _maxDelayMilliseconds = maxDelayMilliseconds;
            _retries = 0;
            _pow = 1;
        }

        public Task Delay()
        {
            ++_retries;
            if (_retries < 31)
            {
                _pow = _pow << 1; // m_pow = Pow(2, m_retries - 1)
            }

            var delay = Math.Min(_delayMilliseconds * (_pow - 1) / 2, _maxDelayMilliseconds);
            return Task.Delay(delay);
        }
    }
}