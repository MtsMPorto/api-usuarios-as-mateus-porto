# API de Gerenciamento de Usuários

## Descrição

Esta API REST foi desenvolvida como parte da Avaliação Semestral da disciplina de Desenvolvimento Backend. O projeto implementa um sistema completo de gerenciamento de usuários utilizando ASP.NET Core com Minimal APIs, seguindo os princípios de Clean Architecture e aplicando padrões de projeto consagrados pela indústria.

A API permite operações CRUD (Create, Read, Update, Delete) sobre usuários, com validação rigorosa de dados de entrada, persistência em banco de dados SQLite através do Entity Framework Core, e implementação de soft delete para remoção lógica de registros.

O projeto demonstra a aplicação prática de conceitos fundamentais de arquitetura de software, incluindo separação de responsabilidades em camadas, inversão de dependências, e validação de dados com FluentValidation.

## Tecnologias Utilizadas

- **.NET 8.0** - Framework principal para desenvolvimento da API
- **ASP.NET Core** - Framework web com Minimal APIs para criação de endpoints RESTful
- **Entity Framework Core 8.0** - ORM para persistência de dados e gerenciamento de migrations
- **SQLite** - Banco de dados relacional leve para armazenamento de dados
- **FluentValidation 11.3** - Biblioteca para validação de DTOs com regras de negócio
- **Swashbuckle (Swagger)** - Documentação interativa da API
- **C# 12.0** - Linguagem de programação com recursos modernos

## Padrões de Projeto Implementados

### Repository Pattern
Abstração da camada de persistência de dados, isolando a lógica de acesso ao banco de dados do restante da aplicação. Implementado através de `IUsuarioRepository` e `UsuarioRepository`.

### Service Pattern
Camada de lógica de negócio que orquestra operações entre repositories e controladores. Implementado através de `IUsuarioService` e `UsuarioService`, concentrando regras como validação de email único, normalização de dados e soft delete.

### DTO Pattern (Data Transfer Object)
Objetos dedicados para transferência de dados entre camadas, evitando exposição direta das entidades de domínio. Implementados três DTOs: `UsuarioCreateDto`, `UsuarioReadDto` e `UsuarioUpdateDto`.

### Dependency Injection
Inversão de controle e injeção de dependências configuradas no `Program.cs`, permitindo baixo acoplamento e alta testabilidade.

### Validator Pattern
Separação da lógica de validação em classes dedicadas usando FluentValidation, com validadores `UsuarioCreateDtoValidator` e `UsuarioUpdateDtoValidator`.

## Arquitetura do Projeto

O projeto segue os princípios de **Clean Architecture**, organizando o código em camadas com responsabilidades bem definidas:

\`\`\`
APIUsuarios/
├── Domain/                          # Camada de Domínio (Entidades)
│   └── Entities/
│       └── Usuario.cs               # Entidade principal do sistema
│
├── Application/                     # Camada de Aplicação (Regras de Negócio)
│   ├── DTOs/                        # Data Transfer Objects
│   │   ├── UsuarioCreateDto.cs     # DTO para criação
│   │   ├── UsuarioReadDto.cs       # DTO para leitura
│   │   └── UsuarioUpdateDto.cs     # DTO para atualização
│   │
│   ├── Interfaces/                  # Contratos das dependências
│   │   ├── IUsuarioRepository.cs   # Interface do Repository
│   │   └── IUsuarioService.cs      # Interface do Service
│   │
│   ├── Services/                    # Implementação da lógica de negócio
│   │   └── UsuarioService.cs       # Service de usuários
│   │
│   └── Validators/                  # Validações com FluentValidation
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
│
├── Infrastructure/                  # Camada de Infraestrutura (Dados)
│   ├── Persistence/
│   │   └── AppDbContext.cs         # Contexto do EF Core
│   │
│   └── Repositories/
│       └── UsuarioRepository.cs    # Implementação do Repository
│
├── Migrations/                      # Migrations do EF Core
│
├── Program.cs                       # Configuração e endpoints da API
├── appsettings.json                # Configurações da aplicação
└── APIUsuarios.csproj              # Arquivo do projeto
\`\`\`

## Como Executar o Projeto

### Pré-requisitos

- **.NET SDK 8.0 ou superior** - [Download](https://dotnet.microsoft.com/download)
- **Editor de código** - Visual Studio 2022, VS Code ou JetBrains Rider

### Passos para Execução

1. **Clone o repositório**
\`\`\`bash
git clone https://github.com/seu-usuario/api-usuarios-as-[seu-nome].git
cd api-usuarios-as-[seu-nome]/APIUsuarios
\`\`\`

2. **Restaure as dependências**
\`\`\`bash
dotnet restore
\`\`\`

3. **Execute as migrations** (criação do banco de dados)
\`\`\`bash
dotnet ef database update
\`\`\`
> Nota: As migrations são aplicadas automaticamente ao iniciar a aplicação.

4. **Execute a aplicação**
\`\`\`bash
dotnet run
\`\`\`

5. **Acesse a documentação Swagger**
\`\`\`
https://localhost:[porta]/swagger
\`\`\`

A aplicação estará rodando e pronta para receber requisições!

## Endpoints da API

| Método | Endpoint | Descrição | Status Sucesso |
|--------|----------|-----------|----------------|
| GET | `/usuarios` | Lista todos os usuários | 200 OK |
| GET | `/usuarios/{id}` | Busca usuário por ID | 200 OK |
| POST | `/usuarios` | Cria novo usuário | 201 Created |
| PUT | `/usuarios/{id}` | Atualiza usuário completo | 200 OK |
| DELETE | `/usuarios/{id}` | Remove usuário (soft delete) | 204 No Content |

### Códigos de Status HTTP

- **200 OK** - Requisição bem-sucedida
- **201 Created** - Recurso criado com sucesso
- **204 No Content** - Recurso removido com sucesso
- **400 Bad Request** - Dados inválidos (erro de validação)
- **404 Not Found** - Usuário não encontrado
- **409 Conflict** - Email já cadastrado
- **500 Internal Server Error** - Erro interno do servidor

## Exemplos de Requisições

### 1. Listar todos os usuários
\`\`\`bash
GET /usuarios
\`\`\`

**Resposta (200 OK):**
\`\`\`json
[
  {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@example.com",
    "dataNascimento": "1990-05-15",
    "telefone": "(11) 98765-4321",
    "ativo": true,
    "dataCriacao": "2024-01-15T10:30:00Z"
  }
]
\`\`\`

### 2. Criar novo usuário
\`\`\`bash
POST /usuarios
Content-Type: application/json

{
  "nome": "Maria Santos",
  "email": "maria@example.com",
  "senha": "senha123",
  "dataNascimento": "1995-08-20",
  "telefone": "(11) 91234-5678"
}
\`\`\`

**Resposta (201 Created):**
\`\`\`json
{
  "id": 2,
  "nome": "Maria Santos",
  "email": "maria@example.com",
  "dataNascimento": "1995-08-20",
  "telefone": "(11) 91234-5678",
  "ativo": true,
  "dataCriacao": "2024-01-15T11:00:00Z"
}
\`\`\`

### 3. Buscar usuário por ID
\`\`\`bash
GET /usuarios/1
\`\`\`

**Resposta (200 OK):**
\`\`\`json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@example.com",
  "dataNascimento": "1990-05-15",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2024-01-15T10:30:00Z"
}
\`\`\`

### 4. Atualizar usuário
\`\`\`bash
PUT /usuarios/1
Content-Type: application/json

{
  "nome": "João Silva Santos",
  "email": "joao.novo@example.com",
  "dataNascimento": "1990-05-15",
  "telefone": "(11) 98765-4321",
  "ativo": true
}
\`\`\`

**Resposta (200 OK):**
\`\`\`json
{
  "id": 1,
  "nome": "João Silva Santos",
  "email": "joao.novo@example.com",
  "dataNascimento": "1990-05-15",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2024-01-15T10:30:00Z"
}
\`\`\`

### 5. Remover usuário (soft delete)
\`\`\`bash
DELETE /usuarios/1
\`\`\`

**Resposta (204 No Content)**

> Nota: O usuário não é removido fisicamente do banco. O campo `ativo` é marcado como `false`.

### 6. Exemplo de erro de validação
\`\`\`bash
POST /usuarios
Content-Type: application/json

{
  "nome": "AB",
  "email": "email-invalido",
  "senha": "123",
  "dataNascimento": "2010-01-01"
}
\`\`\`

**Resposta (400 Bad Request):**
\`\`\`json
{
  "errors": [
    {
      "field": "Nome",
      "message": "Nome deve ter entre 3 e 100 caracteres"
    },
    {
      "field": "Email",
      "message": "Email deve ser válido"
    },
    {
      "field": "Senha",
      "message": "Senha deve ter no mínimo 6 caracteres"
    },
    {
      "field": "DataNascimento",
      "message": "Usuário deve ter pelo menos 18 anos"
    }
  ]
}
\`\`\`

## Regras de Validação

### Criação de Usuário (UsuarioCreateDto)
- **Nome**: Obrigatório, entre 3 e 100 caracteres
- **Email**: Obrigatório, formato válido, deve ser único no sistema
- **Senha**: Obrigatória, mínimo 6 caracteres
- **Data de Nascimento**: Obrigatória, usuário deve ter pelo menos 18 anos
- **Telefone**: Opcional, formato `(XX) XXXXX-XXXX`

### Atualização de Usuário (UsuarioUpdateDto)
- Mesmas regras do Create, exceto senha (não é atualizada)
- Email deve ser único (exceto para o próprio usuário)

### Regras de Negócio Adicionais
- **Email único**: Sistema não permite emails duplicados
- **Normalização de email**: Emails são armazenados em lowercase
- **Soft delete**: Exclusão lógica marcando `ativo = false`
- **Auditoria**: `dataCriacao` e `dataAtualizacao` são gerenciadas automaticamente

## Testando com Postman

Uma collection do Postman está incluída no repositório: `APIUsuarios.postman_collection.json`

Para importar:
1. Abra o Postman
2. Clique em **Import**
3. Selecione o arquivo `APIUsuarios.postman_collection.json`
4. A collection contém todos os endpoints com exemplos de requisições válidas e inválidas

## Estrutura do Banco de Dados

### Tabela: Usuarios

| Coluna | Tipo | Restrições |
|--------|------|------------|
| Id | INTEGER | PRIMARY KEY, AUTO INCREMENT |
| Nome | NVARCHAR(100) | NOT NULL |
| Email | NVARCHAR(255) | NOT NULL, UNIQUE |
| Senha | NVARCHAR(255) | NOT NULL |
| DataNascimento | TEXT | NOT NULL |
| Telefone | NVARCHAR(20) | NULL |
| Ativo | INTEGER | NOT NULL, DEFAULT 1 |
| DataCriacao | TEXT | NOT NULL |
| DataAtualizacao | TEXT | NULL |

> Nota: Em produção, a senha deve ser armazenada com hash (BCrypt, SHA256, etc.)

## Melhorias Futuras

- Implementar hash de senhas com BCrypt
- Adicionar paginação na listagem de usuários
- Implementar autenticação JWT
- Adicionar logging estruturado (Serilog)
- Implementar testes unitários e de integração
- Adicionar cache com Redis
- Implementar rate limiting
- Adicionar health checks
- Documentação com exemplos no Swagger

## Autor

**[Seu Nome Completo]**  
RA: [Seu RA]  
Curso: [Nome do Curso]  
Instituição: [Nome da Instituição]

---

**Desenvolvido como parte da Avaliação Semestral de Desenvolvimento Backend**
