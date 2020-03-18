#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["nexus-broker-sample/Nexus.Samples.Broker.csproj", "nexus-broker-sample/"]
COPY ["Nexus.Samples.Sdk/Nexus.Samples.Sdk.csproj", "Nexus.Samples.Sdk/"]
RUN dotnet restore "nexus-broker-sample/Nexus.Samples.Broker.csproj"
COPY . .
WORKDIR "/src/nexus-broker-sample"
RUN dotnet build "Nexus.Samples.Broker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexus.Samples.Broker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.Samples.Broker.dll"]