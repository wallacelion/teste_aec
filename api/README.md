# 🚀 Avaliação AEC API

API RESTful desenvolvida em **.NET 10** para gerenciamento de usuários e endereços, com autenticação JWT e documentação Swagger interativa.

## 📖 Documentação Swagger

### Acesso Rápido

Quando em modo **Development**, a documentação interativa Swagger está disponível:

```
http://localhost:5000/
```

ou

```
http://localhost:5000/swagger
```

### ✨ Funcionalidades do Swagger

- ✅ Documentação interativa de todos os endpoints
- ✅ Testador de API integrado
- ✅ Autenticação JWT suportada
- ✅ Comentários XML nos endpoints
- ✅ Modelos de dados com exemplos

### 🔐 Como Autenticar no Swagger

1. Faça login via **POST /api/Auth/login**
2. Copie o `accessToken` da resposta
3. Clique no botão **Authorize** 🔒 (canto superior direito)
4. Digite: `Bearer {seu-token-aqui}`
5. Clique em **Authorize** e depois **Close**

Agora você pode testar todos os endpoints protegidos!

📚 **Documentação completa do Swagger**: [docs/SWAGGER.md](docs/SWAGGER.md)

## 🛠 Tecnologias

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core 10**
- **SQL Server**
- **JWT (JSON Web Tokens)**
- **Swagger / Swashbuckle** ✨ (Novo!)
- **Docker**

## 📍 Endpoints Principais

### 🔓 Público
- `POST /api/Auth/login` - Autenticação

### 🔒 Protegido (Requer JWT)

**Usuários**
- `GET /api/User` - Lista usuários
- `GET /api/User/{uid}` - Busca por ID
- `POST /api/User` - Cria usuário
- `PUT /api/User/{uid}` - Atualiza usuário
- `DELETE /api/User/{uid}` - Remove usuário

**Endereços**
- `GET /api/Address` - Lista endereços
- `GET /api/Address/{id}` - Busca por ID
- `POST /api/Address` - Cria endereço
- `PUT /api/Address/{id}` - Atualiza endereço
- `DELETE /api/Address/{id}` - Remove endereço

## ⚡ Início Rápido

```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Configurar banco (edite appsettings.json)
dotnet ef database update

# 3. Executar
dotnet run

# 4. Acessar Swagger
# Abra http://localhost:5000 no navegador
```

## 📂 Estrutura

```
api/
├── config/
│   └── appsettings.json
├── docs/
│   └── SWAGGER.md         ← Documentação Swagger detalhada
├── src/
│   ├── modules/
│   │   ├── address/       ← Módulo de endereços
│   │   ├── auth/          ← Módulo de autenticação
│   │   └── user/          ← Módulo de usuários
│   ├── shared/            ← Código compartilhado
│   └── tests/             ← Arquivos .http para testes
└── Program.cs
```

## 🔧 Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=avaliacaoaec;..."
  },
  "Jwt": {
	"Secret": "sua-chave-secreta"
  },
  "Token": {
	"expiration": 60,
	"refreshExpiration": 120
  }
}
```

## 🧪 Testes

### Via Swagger UI
1. Acesse `http://localhost:5000`
2. Use a interface gráfica

### Via Arquivos .http
- `src/tests/auth.http`
- `src/tests/user.http`
- `src/tests/address.http`
- `src/tests/complete-tests.http`

## 🐳 Docker

```bash
docker build -t avaliacao-aec-api .
docker run -p 8080:80 avaliacao-aec-api
```

## 📝 Changelog - Swagger

### ✨ O que foi adicionado

1. **Swashbuckle.AspNetCore** 7.2.0
2. Configuração completa do Swagger em `Program.cs`
3. Documentação XML habilitada
4. Comentários XML em todos os controllers
5. Suporte a JWT Bearer no Swagger UI
6. Documentação detalhada em `docs/SWAGGER.md`

### ❌ O que foi removido

1. ~~Microsoft.AspNetCore.OpenApi~~ (comentado)
2. ~~Microsoft.OpenApi~~ (removido do .csproj)
3. Referências antigas ao OpenAPI

## 👥 Autor

**Wallace Lion**  
GitHub: [@wallacelion](https://github.com/wallacelion)

---

⭐ **Dica**: Explore a documentação interativa acessando o Swagger UI!
