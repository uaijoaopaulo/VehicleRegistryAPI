FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

# Copia apenas arquivos de projeto
COPY VehicleRegistry.Tests/VehicleRegistry.Tests.csproj VehicleRegistry.Tests/
COPY VehicleRegistry.Api/VehicleRegistry.Api.csproj VehicleRegistry.Api/
COPY VehicleRegistry.Contracts/VehicleRegistry.Contracts.csproj VehicleRegistry.Contracts/
COPY VehicleRegistry.Manager/VehicleRegistry.Manager.csproj VehicleRegistry.Manager/

# Restaura os pacotes
RUN dotnet restore VehicleRegistry.Tests/VehicleRegistry.Tests.csproj

# Copia tudo
COPY . .

# Build com saída para /app/teste — obrigatório para manter testhost.deps.json
RUN dotnet build VehicleRegistry.Tests/VehicleRegistry.Tests.csproj -c Debug -o /app/teste

WORKDIR /app/teste

# ENTRYPOINT usando dotnet test diretamente no projeto, agora com logs visíveis
ENTRYPOINT ["dotnet", "test", "VehicleRegistry.Tests.dll", "--filter", "Category=Integration", "--logger:trx"]
