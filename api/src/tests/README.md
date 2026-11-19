# 🧪 Guia de Testes da API

Este diretório contém arquivos `.http` para testar os endpoints da API usando ferramentas como **REST Client** (extensão do VS Code) ou **HTTP Client** do Visual Studio.

## 📁 Arquivos de Teste

### 1️⃣ `auth.http`
Testes de autenticação:
- Login de usuário administrador
- Login de usuário comum

### 2️⃣ `user.http`
CRUD completo de usuários:
- ✅ GET - Listar todos os usuários
- ✅ GET - Buscar usuário por UID
- ✅ POST - Criar novo usuário
- ✅ PUT - Atualizar usuário
- ✅ DELETE - Remover usuário

### 3️⃣ `address.http`
CRUD completo de endereços:
- ✅ GET - Listar todos os endereços
- ✅ GET - Buscar endereço por ID
- ✅ POST - Criar novo endereço
- ✅ PUT - Atualizar endereço
- ✅ DELETE - Remover endereço

### 4️⃣ `complete-tests.http` ⭐
Arquivo com fluxo completo de testes integrados:
- Login automático
- Captura automática do token
- Testes de todos os endpoints
- Testes de autorização (sem token)

## 🔐 Autenticação

Todos os endpoints de **User** e **Address** requerem autenticação JWT.

### Fluxo de Autenticação:

1. **Fazer login** no endpoint `/api/auth/login`
2. **Copiar o token** retornado no campo `accessToken`
3. **Adicionar o token** nos headers das requisições:
   ```
   Authorization: Bearer SEU_TOKEN_AQUI
   ```

### Exemplo de Login:

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "Admin@123"
}
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## 🚀 Como Usar

### Usando REST Client (VS Code)

1. Instale a extensão **REST Client** no VS Code
2. Abra qualquer arquivo `.http`
3. Clique em **"Send Request"** acima de cada requisição
4. Veja a resposta no painel lateral

### Usando HTTP Client (Visual Studio 2026)

1. Abra qualquer arquivo `.http` no Visual Studio
2. Clique no botão ▶️ ao lado de cada requisição
3. Veja a resposta na janela de output

### Usando o arquivo `complete-tests.http`

Este arquivo usa **variáveis dinâmicas** para facilitar os testes:

```http
# 1. Execute o login primeiro
POST {{api_url}}/api/auth/login

# 2. O token é capturado automaticamente
@token = {{login.response.body.$.accessToken}}

# 3. As próximas requisições usam o token automaticamente
GET {{api_url}}/api/user
Authorization: Bearer {{token}}
```

## 📊 Estrutura dos Dados

### UserDto
```json
{
  "name": "Nome do Usuário",
  "email": "email@example.com",
  "password": "Senha@123",
  "role": 1
}
```

**Roles disponíveis:**
- `0` = User
- `1` = Admin

### AddressDto
```json
{
  "userId": 1,
  "cep": "01001-000",
  "logradouro": "Praça da Sé",
  "number": "123",
  "complemento": "Andar 5",
  "bairro": "Sé",
  "localidade": "São Paulo",
  "uf": "SP"
}
```

## ⚠️ Observações Importantes

1. **Substitua os IDs/UIDs**: Os valores `00000000-0000-0000-0000-000000000000` e `1` são apenas exemplos. Use os IDs reais retornados pelas requisições.

2. **Token expirado**: O token JWT expira após o tempo configurado (padrão: 60 minutos). Se receber erro 401, faça login novamente.

3. **Ordem de execução**: 
   - Sempre faça **login primeiro**
   - Crie registros antes de tentar atualizá-los ou deletá-los

4. **Porta da API**: A porta padrão é `5000`. Se sua API estiver rodando em outra porta, atualize a variável `@api_url`.

## 🔒 Endpoints Protegidos

Todos estes endpoints requerem autenticação:

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/user` | Listar usuários |
| GET | `/api/user/{uid}` | Buscar usuário |
| POST | `/api/user` | Criar usuário |
| PUT | `/api/user/{uid}` | Atualizar usuário |
| DELETE | `/api/user/{uid}` | Deletar usuário |
| GET | `/api/address` | Listar endereços |
| GET | `/api/address/{id}` | Buscar endereço |
| POST | `/api/address` | Criar endereço |
| PUT | `/api/address/{id}` | Atualizar endereço |
| DELETE | `/api/address/{id}` | Deletar endereço |

## 📝 Endpoint Público

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/auth/login` | Login (público) |

---

**Dica:** Use o arquivo `complete-tests.http` para um fluxo de testes automatizado! 🚀
