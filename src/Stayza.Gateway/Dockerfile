FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Stayza.Gateway/Stayza.Gateway.csproj", "src/Stayza.Gateway/"]
RUN dotnet restore "src/Stayza.Gateway/Stayza.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Stayza.Gateway"
RUN dotnet build "Stayza.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Stayza.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stayza.Gateway.dll"]
