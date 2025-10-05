FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["src/Stayza.MigrationService/Stayza.MigrationService.csproj", "src/Stayza.MigrationService/"]
COPY ["src/Stayza.Core/Stayza.Core.csproj", "src/Stayza.Core/"]
COPY ["src/Stayza.Infrastructure/Stayza.Infrastructure.csproj", "src/Stayza.Infrastructure/"]
COPY ["src/Stayza.Application/Stayza.Application.csproj", "src/Stayza.Application/"]
COPY ["src/Stayza.Domain/Stayza.Domain.csproj", "src/Stayza.Domain/"]
RUN dotnet restore "src/Stayza.MigrationService/Stayza.MigrationService.csproj"
COPY . .
WORKDIR "/src/src/Stayza.MigrationService"
RUN dotnet build "Stayza.MigrationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Stayza.MigrationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stayza.MigrationService.dll"]
