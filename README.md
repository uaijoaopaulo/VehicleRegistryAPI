
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

- Docker + Docker Compose instalados
- Conta AWS com:
  - Bucket S3: `vehiclesregistry-vehicle-files`
  - Fila SQS: `vehiclesregistry-FileUploadedS3Notifications`
- Arquivo de credenciais da AWS em `~/.aws/credentials` com perfil `[default]` configurado

- Caso prefira utilizar um nome diferente para o Bucket ou a Fila SQS, lembre-se de atualizar os respectivos valores nas variáveis de ambiente e arquivos de configuração do projeto.

### 2. Configuração da AWS

- Crie o bucket S3 com o nome desejado (ou utilize `vehiclesregistry-vehicle-files`). 
- Configure o bucket para enviar notificações de eventos PUT para a sua própria fila SQS, criada e referenciada nas variáveis de ambiente do projeto.
  Exemplo de URL (substitua pela sua):
  `https://sqs.us-east-2.amazonaws.com/475515413144/vehiclesregistry-FileUploadedS3Notifications`

  #### 🛠️ Como configurar o S3 para enviar notificações de PUT para a fila SQS
    1. Acesse o Console da AWS
      Vá até o [Amazon S3 Console](https://s3.console.aws.amazon.com/s3/).

    2. Selecione o Bucket desejado
      Clique no nome do bucket que você criou (ex: vehiclesregistry-vehicle-files).

    3. Acesse a aba “Properties” (Propriedades)

    4. Role até a seção “Event notifications” (Notificações de eventos)
      Clique em "Create event notification".

    5. Configure os detalhes da notificação

        - Name: Dê um nome descritivo à notificação (ex: PutToSQS).
        - Event types: Marque a opção PUT.
        - Prefix / Suffix (opcional): Deixe em branco, a menos que deseje filtrar por pasta ou extensão de arquivo.

    6. Destino (Destination)
    
        - Selecione SQS Queue.
        - Escolha a fila SQS criada anteriormente.
        - Caso ela não apareça na lista, verifique se:
            - A fila está na mesma região do bucket;
            - O usuário/role tem permissão para acessá-la.

    7. Salvar
      Clique em "Save changes" para concluir a configuração.

  #### 🔐 Permissões Recomendadas
    Certifique-se de que a fila SQS permita que o bucket S3 envie mensagens. Você pode adicionar uma política de acesso à fila SQS como esta (ajuste bucket-name e queue-arn):

    ```json
    {
      "Version": "2012-10-17",
      "Statement": [
        {
          "Effect": "Allow",
          "Principal": { "Service": "s3.amazonaws.com" },
          "Action": "SQS:SendMessage",
          "Resource": "arn:aws:sqs:us-east-2:account-id:queue-name",
          "Condition": {
            "ArnLike": {
              "aws:SourceArn": "arn:aws:s3:::bucket-name"
            }
          }
        }
      ]
    }
    ```

### 3. Subir os containers

```bash
docker-compose up --build
```

Esse comando irá:

- Subir os serviços `api`, `worker`, `mongo`, `postgres`
- Criar o banco de dados, usuário e as collections do MongoDB:
  - `usuarios` (com 2 usuários pré-configurados)
  - `vehicle-files` com TTL de 24 horas
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
