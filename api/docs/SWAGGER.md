# Documentação Swagger - Avaliação AEC API

## 📘 Visão Geral

Este projeto utiliza **Swagger** (via Swashbuckle.AspNetCore) para documentação interativa da API.

## 🚀 Acesso ao Swagger

### Ambiente de Desenvolvimento

Quando a aplicação está em modo `Development`, o Swagger UI está disponível na raiz da aplicação:

```
http://localhost:<porta>/
```

Ou pelo endpoint direto:

```
http://localhost:<porta>/swagger
```

### Swagger JSON

A especificação OpenAPI em formato JSON está disponível em:

```
http://localhost:<porta>/swagger/v1/swagger.json
```

## 🔐 Autenticação JWT no Swagger

A API utiliza autenticação JWT Bearer Token. Para testar endpoints autenticados no Swagger:

### Passo 1: Obter o Token

1. Acesse o endpoint **POST /api/Auth/login**
2. Insira as credenciais:
   ```json
   {
	 "username": "seu-email@exemplo.com",
	 "password": "sua-senha"
   }
   ```
3. Clique em **Execute**
4. Copie o valor do campo `accessToken` da resposta

### Passo 2: Autorizar no Swagger

1. Clique no botão **Authorize** 🔒 (canto superior direito)
2. No campo **Value**, insira:
   ```
   Bearer seu-token-aqui
   ```
   ⚠️ **Importante**: Inclua a palavra "Bearer" seguida de um espaço antes do token
3. Clique em **Authorize**
4. Clique em **Close**

### Passo 3: Testar Endpoints Protegidos

Agora você pode testar qualquer endpoint que requer autenticação (marcados com 🔒).

## 📚 Endpoints Documentados

### Autenticação (`/api/Auth`)
- `POST /api/Auth/login` - Realiza login e retorna tokens JWT

### Usuários (`/api/User`)
- `GET /api/User` - Lista todos os usuários 🔒
- `GET /api/User/{uid}` - Obtém usuário por ID 🔒
- `POST /api/User` - Cria novo usuário 🔒
- `PUT /api/User/{uid}` - Atualiza usuário 🔒
- `DELETE /api/User/{uid}` - Exclui usuário 🔒

### Endereços (`/api/Address`)
- `GET /api/Address` - Lista todos os endereços 🔒
- `GET /api/Address/{id}` - Obtém endereço por ID 🔒
- `POST /api/Address` - Cria novo endereço 🔒
- `PUT /api/Address/{id}` - Atualiza endereço 🔒
- `DELETE /api/Address/{id}` - Exclui endereço 🔒

🔒 = Requer autenticação

## 📝 Comentários XML

O projeto está configurado para gerar documentação XML automaticamente, que é integrada ao Swagger UI.

Para adicionar documentação aos seus controllers/métodos, use comentários XML:

```csharp
/// <summary>
/// Descrição do método
/// </summary>
/// <param name="parametro">Descrição do parâmetro</param>
/// <returns>Descrição do retorno</returns>
/// <response code="200">Descrição da resposta de sucesso</response>
/// <response code="404">Descrição quando não encontrado</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(MinhaDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> MeuMetodo(int id)
{
	// ...
}
```

## ⚙️ Configuração Técnica

### Pacotes Utilizados

- **Swashbuckle.AspNetCore** v7.2.0 - Geração de Swagger/OpenAPI

### Configuração em `Program.cs`

```csharp
// Registro do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Avaliação AEC API",
		Description = "API para gerenciamento de usuários e endereços"
	});

	// Autenticação JWT
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header
	});

	// Comentários XML
	var xmlPath = Path.Combine(AppContext.BaseDirectory, "api.xml");
	options.IncludeXmlComments(xmlPath);
});

// Middleware do Swagger (apenas em Development)
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
		options.RoutePrefix = string.Empty; // Swagger na raiz
	});
}
```

### Geração de XML em `api.csproj`

```xml
<PropertyGroup>
	<GenerateDocumentationFile>true</GenerateDocumentationFile>
	<NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## 🔧 Customizações Possíveis

### Alterar a URL do Swagger UI

Em `Program.cs`, modifique:

```csharp
options.RoutePrefix = "api-docs"; // Acesso via /api-docs
```

### Adicionar Múltiplas Versões da API

```csharp
options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API v1" });
options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API v2" });
```

### Habilitar Swagger em Produção

⚠️ **Atenção**: Avaliar riscos de segurança antes de expor Swagger em produção.

```csharp
// Remover a condição if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();
```

## 🛠️ Troubleshooting

### Swagger não está carregando

1. Verifique se a aplicação está em modo `Development`
2. Confirme que os pacotes estão instalados:
   ```bash
   dotnet list package | findstr Swashbuckle
   ```
3. Verifique se não há erros de compilação

### Comentários XML não aparecem

1. Verifique se `<GenerateDocumentationFile>` está `true` no `.csproj`
2. Confirme que o arquivo XML está sendo gerado na pasta `bin`
3. Verifique se o caminho do XML em `Program.cs` está correto

### Token JWT não está funcionando

1. Verifique se o token foi copiado corretamente
2. Confirme que incluiu "Bearer " antes do token
3. Verifique se o token não expirou (tempo de expiração configurado em `appsettings.json`)

## 📚 Recursos Adicionais

- [Documentação Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Especificação OpenAPI](https://swagger.io/specification/)
- [ASP.NET Core Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

---

**Desenvolvido para Avaliação AEC**  
Repositório: https://github.com/wallacelion/avaliacao_dev
