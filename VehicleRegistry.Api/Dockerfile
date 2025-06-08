# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VehicleRegistry.Api/VehicleRegistry.Api.csproj ./VehicleRegistry.Api/
RUN dotnet restore ./VehicleRegistry.Api/VehicleRegistry.Api.csproj

COPY . .
WORKDIR /src/VehicleRegistry.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VehicleRegistry.Api.dll"]