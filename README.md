
# 📦 Vehicle Registry API - Documentação da Integração

Este projeto é composto por dois serviços principais:

- `VehicleRegistry.Api`: API REST responsável por expor endpoints para registro e consulta de arquivos de veículos.
- `VehicleRegistry.Worker`: Serviço que consome notificações da fila SQS e processa arquivos armazenados no S3.

Ambos os serviços dependem de **MongoDB** e **PostgreSQL**, e são orquestrados por Docker.

---

## 🔧 Tecnologias Utilizadas

- .NET 8
- Docker + Docker Compose
- MongoDB
- PostgreSQL
- Amazon S3
- Amazon SQS
- JWT
- xUnit

---

## 🚀 Como Executar o Projeto

### 1. Pré-requisitos

- **Permissões no EC2 (Produção):**  
  O perfil da IAM Role associado à instância EC2.

- **Credenciais da AWS (Local):**  
  As credenciais devem estar configuradas no arquivo `~/.aws/credentials` utilizando o perfil padrão (`[default]`).
  
- Em ambas as credenciais devem conter as seguintes permissões:
  - `AmazonS3FullAccess`
  - `AmazonSQSFullAccess`

### 2. Subir os containers

```bash
docker-compose up --build
```

Esse comando irá:

- Subir os serviços `api`, `worker`, `mongo`, `postgres`
- Criar o banco de dados, usuário e as collections do MongoDB:
  - `usuarios` (com 2 usuários pré-configurados)
  - `vehicle-files` com TTL de 24 horas
- Criar o Bucket e a Queue na AWS já configurados.
- Iniciar o consumo contínuo da fila SQS pelo `VehicleRegistry.Worker`

---

## 🌍 Endpoints da API

### 📤 POST `/api/auth/login`

- **Descrição**: Autentica um usuário e retorna um token JWT

#### Exemplo de requisição:

```bash
curl -X POST http://localhost:8080/api/auth/login   -H "Content-Type: application/json"   -d '{"username": "admin@acme.com", "password": "admin123"}'
```

#### Exemplo de resposta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
  "roles": ["vehicle-read", "vehicle-admin"]
}
```

---

### 📤 POST `/api/vehicles/{vehicleId}/file`

- **Descrição**: Gera um link `presignedUrl` para upload de arquivo no S3.

#### Exemplo:

```bash
curl -X POST http://localhost:8080/api/vehicles/1/file   -H "Authorization: Bearer {token}" -d '{"fileName": "file.pdf", "fileMimetype": "application/pdf"}'
```

#### Resposta:

```json
{
  "errors": [],
  "result":{
    "fileName": "file.pdf",
    "fileMimetype": "application/pdf",
    "uploadUrl": "https://vehiclesregistry-vehicle-files.s3.amazonaws.com/1/file.pdf?..."
  }
}
```

---

### 📥 GET `/api/vehicles/{vehicleId}/file`

- **Descrição**: Lista os arquivos com status `uploaded` de um veículo.

#### Exemplo:

```bash
curl http://localhost:5036/api/vehicles/1/file   -H "Authorization: Bearer {token}"
```

#### Resposta:

```json
{
  "errors": [],
  "result":{
    "id": 1,
    "vehicleId": 1,
    "fileName": "file.pdf",
    "fileMimetype": "application/pdf",
    "fileUrl": "https://vehiclesregistry-vehicle-files.s3.amazonaws.com/1/file.pdf?...",
    "createdAt": "2025-06-09T19:50:34.85Z"
  }
}
```

---

## ⚙️ Funcionamento Interno

1. API gera `presignedUrl` para upload no S3
2. S3 envia notificação para a fila SQS
3. `Worker` consome notificação, atualiza data de criação e status para `uploaded`
4. API permite consulta do arquivo
5. Arquivos pendentes por 24h são removidos automaticamente via TTL

---

## 🧪 Testes

### Executar testes de integração

```bash
docker-compose -f docker-compose-test.yml up --build --abort-on-container-exit
```

---

## 🔐 Autenticação

- Credencial AWS: perfil `[default]` no `~/.aws/credentials`

---

## ✅ Usuários Pré-configurados

- Dois usuários são criados automaticamente na collection `usuarios` para testes iniciais.

---

## 📂 Estrutura do Projeto

```
/
├── VehicleRegistry.Api/
├── VehicleRegistry.Worker/
├── docker-compose.yml
├── docker-compose-test.yml
```

---
