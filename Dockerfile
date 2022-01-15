# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY ["DictionaryBackWorker/DictionaryBackWorker.csproj", "./DictionaryBackWorker/DictionaryBackWorker.csproj"]
COPY ["DictionaryBack.Contracts/DictionaryBack.Contracts.csproj", "./DictionaryBack.Contracts/DictionaryBack.Contracts.csproj"]
COPY ["DictionaryBackWorker.sln", "DictionaryBackWorker.sln"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app .

ENV ASPNETCORE_URLS http://*:4350


ENTRYPOINT ["dotnet", "DictionaryBackWorker.dll"]