FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["src/Stayza.App/Stayza.App.csproj", "src/Stayza.App/"]
COPY ["src/Stayza.Core/Stayza.Core.csproj", "src/Stayza.Core/"]
COPY ["src/Stayza.Application/Stayza.Application.csproj", "src/Stayza.Application/"]
COPY ["src/Stayza.Domain/Stayza.Domain.csproj", "src/Stayza.Domain/"]
COPY ["src/Stayza.Infrastructure/Stayza.Infrastructure.csproj", "src/Stayza.Infrastructure/"]
RUN dotnet restore "src/Stayza.App/Stayza.App.csproj"
COPY . .
WORKDIR "/src/src/Stayza.App"
RUN dotnet build "Stayza.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Stayza.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stayza.App.dll"]
