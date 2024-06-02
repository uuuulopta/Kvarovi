namespace Kvarovi.AnnouncementGetters.KeywordsParsers;

using System.Text;

public class ElectricityKeywordsParserStrategy:KeywordsParserStrategy 
{

    
    // Parse the street numbers (ex. 2-5 gets truned into 2,3,4,5)
    public IEnumerable<string> getKeywordsFromText(string text)
    {

        HashSet<string> keywords = new();
        int pos = -1;
        while((pos=text.IndexOf('-',pos+1))!=-1)
        {
            var parsedRange = parseHyphened(pos,text);
            if (parsedRange == null) continue;
            int totalLength = parsedRange.Value.EndTextIndex - parsedRange.Value.BeginTextIndex + 1;
            string streetName = getStreetNameForHyphen(pos, text);
            for (int i = parsedRange.Value.Num1; i <= parsedRange.Value.Num2; i++)
            {
                
               keywords.Add($"{streetName.ToLower()} {i}"); 
            }
    
        }

        return keywords;
    }

    private string getStreetNameForHyphen(int hyphenIndex,string text)
    { 
        string streetName = "";
        int index = hyphenIndex;
        bool wordFlag = false;
        while (true)
        {
            index--;
            var current = text[index];

            if (!wordFlag && current != ':') continue;
            if (!wordFlag && current == ':')
            {
                wordFlag = true;
                continue;
            }
            if (char.IsDigit(current) || current == ',' || current == ':') break;
            streetName = current + streetName;

        }
    
        return streetName.Trim();
    }
    
    private struct ParsedRange
    {
        public int BeginTextIndex { get; }
        public int EndTextIndex { get; }
        public int Num1 { get; }
        public int Num2 { get; }

        public ParsedRange(int beginTextIndex, int endTextIndex, int num1, int num2)
        {
            this.BeginTextIndex = beginTextIndex;
            this.EndTextIndex = endTextIndex;
            this.Num1 = num1;
            this.Num2 = num2;
        }

    }
    
    private ParsedRange? parseHyphened(int indexOfHyphen,string text)
    {
        int currentPos = indexOfHyphen;
        bool first = true;
        string num1 = "";
        string num2 = "";
        while (true)
        {
            currentPos -= 1;
            char curr = text[currentPos];
            if (first)
            {
                first = false;
                if (text[currentPos - 1] == 'L' || text[currentPos - 1] == 'N') first = true;
                if (char.IsLetter(curr)) continue;
            }

            if (!char.IsDigit(curr)) break;
            num1 = curr + num1;
        }

        int begin = currentPos + 1;
        currentPos = indexOfHyphen;
        while (true)
        {
            currentPos += 1;
            char curr = text[currentPos];
            if (!char.IsDigit(curr)) break;
            num2 += curr;
        }

        int end = currentPos - 1;
        

        if (string.IsNullOrWhiteSpace(num1) || string.IsNullOrWhiteSpace(num2)) return null;
        if (!int.TryParse(num1, out var num1i) || !int.TryParse(num2, out var num2i)) return null;
        return new ParsedRange(begin, end, num1i, num2i);
    }
}
