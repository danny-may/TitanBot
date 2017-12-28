FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY Titanbot.LiveTesting/Titanbot.LiveTesting.csproj Titanbot.LiveTesting/
COPY Titanbot.BaseCommands/Titanbot.BaseCommands.csproj Titanbot.BaseCommands/
COPY Titanbot/Titanbot.csproj Titanbot/
COPY Titansmasher.Utilities/Titansmasher.Utilities.csproj Titansmasher.Utilities/
COPY Titanbot.Extensions/Titanbot.Extensions.csproj Titanbot.Extensions/
RUN dotnet restore
COPY . .
WORKDIR /src/Titanbot.LiveTesting
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Titanbot.LiveTesting.dll"]
