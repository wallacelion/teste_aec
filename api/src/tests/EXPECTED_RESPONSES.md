# 📋 Respostas Esperadas da API

## 🔐 Auth Endpoints

### ✅ POST `/api/auth/login` - Login Bem-Sucedido
**Status:** `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
}
```

### ❌ POST `/api/auth/login` - Credenciais Inválidas
**Status:** `401 Unauthorized`
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

## 👥 User Endpoints

### ✅ GET `/api/user` - Listar Todos os Usuários
**Status:** `200 OK`
```json
[
  {
	"uid": "123e4567-e89b-12d3-a456-426614174000",
	"name": "Admin User",
	"email": "admin@example.com",
	"password": null,
	"role": 1
  },
  {
	"uid": "223e4567-e89b-12d3-a456-426614174001",
	"name": "Regular User",
	"email": "user@example.com",
	"password": null,
	"role": 0
  }
]
```

### ✅ GET `/api/user/{uid}` - Buscar Usuário por UID
**Status:** `200 OK`
```json
{
  "uid": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Admin User",
  "email": "admin@example.com",
  "password": null,
  "role": 1
}
```

### ❌ GET `/api/user/{uid}` - Usuário Não Encontrado
**Status:** `404 Not Found`

### ✅ POST `/api/user` - Criar Novo Usuário
**Status:** `201 Created`
```json
{
  "uid": "323e4567-e89b-12d3-a456-426614174002",
  "name": "Novo Usuario",
  "email": "novo@example.com",
  "password": null,
  "role": 1
}
```

**Headers:**
```
Location: /api/user/323e4567-e89b-12d3-a456-426614174002
```

### ✅ PUT `/api/user/{uid}` - Atualizar Usuário
**Status:** `200 OK`
```json
{
  "uid": "323e4567-e89b-12d3-a456-426614174002",
  "name": "Usuario Atualizado",
  "email": "novo@example.com",
  "password": null,
  "role": 1
}
```

### ❌ PUT `/api/user/{uid}` - Usuário Não Encontrado
**Status:** `404 Not Found`

### ✅ DELETE `/api/user/{uid}` - Deletar Usuário
**Status:** `204 No Content`

### ❌ DELETE `/api/user/{uid}` - Usuário Não Encontrado
**Status:** `404 Not Found`

---

## 🏠 Address Endpoints

### ✅ GET `/api/address` - Listar Todos os Endereços
**Status:** `200 OK`
```json
[
  {
	"id": 1,
	"userId": 1,
	"cep": "01310-100",
	"logradouro": "Avenida Paulista",
	"number": "1578",
	"complemento": "Andar 15",
	"bairro": "Bela Vista",
	"localidade": "São Paulo",
	"uf": "SP"
  },
  {
	"id": 2,
	"userId": 1,
	"cep": "01001-000",
	"logradouro": "Praça da Sé",
	"number": "100",
	"complemento": "Centro",
	"bairro": "Sé",
	"localidade": "São Paulo",
	"uf": "SP"
  }
]
```

### ✅ GET `/api/address/{id}` - Buscar Endereço por ID
**Status:** `200 OK`
```json
{
  "id": 1,
  "userId": 1,
  "cep": "01310-100",
  "logradouro": "Avenida Paulista",
  "number": "1578",
  "complemento": "Andar 15",
  "bairro": "Bela Vista",
  "localidade": "São Paulo",
  "uf": "SP"
}
```

### ❌ GET `/api/address/{id}` - Endereço Não Encontrado
**Status:** `404 Not Found`

### ✅ POST `/api/address` - Criar Novo Endereço
**Status:** `201 Created`
```json
{
  "id": 3,
  "userId": 1,
  "cep": "04543-907",
  "logradouro": "Avenida Brigadeiro Faria Lima",
  "number": "3477",
  "complemento": "10º andar",
  "bairro": "Itaim Bibi",
  "localidade": "São Paulo",
  "uf": "SP"
}
```

**Headers:**
```
Location: /api/address/3
```

### ✅ PUT `/api/address/{id}` - Atualizar Endereço
**Status:** `200 OK`
```json
{
  "id": 3,
  "userId": 1,
  "cep": "04543-907",
  "logradouro": "Avenida Brigadeiro Faria Lima",
  "number": "3477",
  "complemento": "15º andar",
  "bairro": "Itaim Bibi",
  "localidade": "São Paulo",
  "uf": "SP"
}
```

### ❌ PUT `/api/address/{id}` - Endereço Não Encontrado
**Status:** `404 Not Found`

### ✅ DELETE `/api/address/{id}` - Deletar Endereço
**Status:** `204 No Content`

### ❌ DELETE `/api/address/{id}` - Endereço Não Encontrado
**Status:** `404 Not Found`

---

## 🚫 Erros de Autenticação

### ❌ Requisição Sem Token
**Status:** `401 Unauthorized`
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### ❌ Token Inválido ou Expirado
**Status:** `401 Unauthorized`
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

## 📊 Códigos de Status HTTP

| Código | Significado | Quando Ocorre |
|--------|-------------|---------------|
| 200 | OK | Requisição bem-sucedida (GET, PUT) |
| 201 | Created | Recurso criado com sucesso (POST) |
| 204 | No Content | Recurso deletado com sucesso (DELETE) |
| 400 | Bad Request | Dados inválidos na requisição |
| 401 | Unauthorized | Token ausente, inválido ou expirado |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro no servidor |

---

## 🔧 Headers Padrão

### Requisições Autenticadas
```http
Authorization: Bearer {token}
Content-Type: application/json
Accept: application/json
```

### Requisições de Login
```http
Content-Type: application/json
```

---

## 💡 Dicas

1. **Token no Header**: Sempre inclua o token JWT no header `Authorization` com o prefixo `Bearer`
2. **Content-Type**: Use `application/json` para requisições POST e PUT
3. **IDs/UIDs**: Use os valores reais retornados pela API, não os exemplos
4. **Senha**: Nunca é retornada nas respostas (sempre `null` ou omitida)
5. **Location Header**: Em requisições POST bem-sucedidas, o header `Location` contém a URL do novo recurso

---

**Última atualização:** 2024
