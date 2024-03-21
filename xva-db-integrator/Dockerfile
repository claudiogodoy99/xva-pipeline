
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["xva-db-integrator/xva-db-integrator.csproj", "xva-db-integrator/"]
COPY ["common/common.csproj", "common/"]
RUN dotnet restore "./xva-db-integrator/xva-db-integrator.csproj"
COPY . .
WORKDIR "/src/xva-db-integrator"
RUN dotnet build "./xva-db-integrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./xva-db-integrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "xva-db-integrator.dll"]