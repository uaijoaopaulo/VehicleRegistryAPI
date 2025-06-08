# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VehicleRegistry.Worker/VehicleRegistry.Worker.csproj ./VehicleRegistry.Worker/
RUN dotnet restore ./VehicleRegistry.Worker/VehicleRegistry.Worker.csproj

COPY . .
WORKDIR /src/VehicleRegistry.Worker
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VehicleRegistry.Worker.dll"]