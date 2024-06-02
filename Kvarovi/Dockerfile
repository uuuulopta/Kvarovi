FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Kvarovi/Kvarovi.csproj", "Kvarovi/"]
RUN dotnet restore "Kvarovi/Kvarovi.csproj"
COPY . .
WORKDIR "/src/Kvarovi"
RUN dotnet build "Kvarovi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kvarovi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN touch output
ENTRYPOINT ["dotnet", "Kvarovi.dll",">>","output"]
