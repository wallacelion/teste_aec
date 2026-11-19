# Cache Redis - Integração ViaCEP

## 📋 Resumo

Implementação de cache Redis para otimizar consultas ao serviço ViaCEP, reduzindo latência e número de requisições à API externa.

## 🎯 Benefícios

- ✅ **Performance**: Redução de ~80-95% no tempo de resposta para CEPs já consultados
- ✅ **Economia**: Menos chamadas à API externa ViaCEP
- ✅ **Resiliência**: Dados em cache permanecem acessíveis mesmo se ViaCEP estiver indisponível
- ✅ **Escalabilidade**: Cache distribuído entre instâncias da API

## 🏗️ Arquitetura

### Cache-Aside Pattern

```
┌──────────┐      ┌─────────────┐      ┌───────────┐      ┌─────────┐
│ Cliente  │─────▶│AddressService│─────▶│Redis Cache│      │ ViaCEP  │
└──────────┘      └─────────────┘      └───────────┘      └─────────┘
						 │                    │                 │
						 │  1. Check cache    │                 │
						 │───────────────────▶│                 │
						 │                    │                 │
						 │  2. Cache HIT?     │                 │
						 │◀───────────────────│                 │
						 │     (MISS)         │                 │
						 │                    │                 │
						 │  3. Fetch from API │                 │
						 │────────────────────┼────────────────▶│
						 │                    │                 │
						 │  4. API Response   │                 │
						 │◀───────────────────┼─────────────────│
						 │                    │                 │
						 │  5. Store in cache │                 │
						 │───────────────────▶│                 │
						 │                    │                 │
```

## 📦 Componentes Criados

### 1. Interface de Cache
**Arquivo**: `api/src/modules/address/infrastructure/cache/interfaces/IViaCepCacheService.cs`

```csharp
Task<ViaCepResponseDto?> GetAsync(string cep);
Task SetAsync(string cep, ViaCepResponseDto data, int expirationMinutes = 1440);
Task RemoveAsync(string cep);
Task<bool> ExistsAsync(string cep);
```

### 2. Implementação Redis
**Arquivo**: `api/src/modules/address/infrastructure/cache/services/ViaCepCacheService.cs`

- Usa `IDistributedCache` do ASP.NET Core
- Serialização JSON automática
- Normalização de chaves (remove formatação do CEP)
- Tratamento de erros robusto
- Logs detalhados (HIT/MISS)

### 3. Integração no AddressService
**Arquivo**: `api/src/modules/address/application/services/AddressService.cs`

**Fluxo atualizado**:
```csharp
// Verificar cache
var cachedData = await _cacheService.GetAsync(cep);

if (cachedData != null)
{
	// Cache HIT - usar dados do cache
}
else
{
	// Cache MISS - consultar ViaCEP
	var viaCepData = await _viaCepService.GetAddressByCepAsync(cep);

	// Armazenar no cache
	if (viaCepData != null && !viaCepData.Erro)
	{
		await _cacheService.SetAsync(cep, viaCepData);
	}
}
```

## ⚙️ Configuração

### 1. Instalar Redis

**Docker** (recomendado para desenvolvimento):
```bash
docker run -d --name redis-viacep -p 6379:6379 redis:latest
```

**Windows** (via Chocolatey):
```bash
choco install redis-64
redis-server
```

**Linux** (Ubuntu/Debian):
```bash
sudo apt-get install redis-server
sudo systemctl start redis-server
```

### 2. Configurar Connection String

**appsettings.json**:
```json
{
  "ConnectionStrings": {
	"Redis": "localhost:6379"
  },
  "Cache": {
	"ViaCep": {
	  "ExpirationMinutes": 1440
	}
  }
}
```

**appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
	"Redis": "localhost:6379"
  }
}
```

### 3. Registro de Serviços

O cache é registrado automaticamente em `AddressModuleConfig.cs`:

```csharp
// Redis Cache
services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = configuration.GetConnectionString("Redis");
	options.InstanceName = "ViaCep_";
});

// Serviço de Cache
services.AddScoped<IViaCepCacheService, ViaCepCacheService>();
```

## 🧪 Testes

### Arquivo de Testes
**Arquivo**: `api/src/tests/address-cache.http`

### Testar Cache HIT/MISS

1. **Primeira chamada** (MISS - consulta ViaCEP):
```http
POST http://localhost:5000/api/address
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": 1,
  "cep": "01310-100",
  "number": "1000"
}
```

**Logs esperados**:
```
Cache MISS para CEP 01310100
Consultando ViaCEP
CEP 01310100 armazenado no cache com expiração de 1440 minutos
```

2. **Segunda chamada** (HIT - usa cache):
```http
POST http://localhost:5000/api/address
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": 1,
  "cep": "01310-100",
  "number": "2000"
}
```

**Logs esperados**:
```
Cache HIT para CEP 01310100
```

### Performance Esperada

| Cenário | Tempo Médio | Observação |
|---------|-------------|------------|
| Cache MISS | 200-500ms | Depende da latência do ViaCEP |
| Cache HIT | 10-50ms | Apenas consulta ao Redis local |
| **Ganho** | **80-95%** | Redução significativa de latência |

## 🔍 Monitoramento

### Verificar Cache via Redis CLI

```bash
# Conectar ao Redis
redis-cli

# Listar todas as chaves de CEP
KEYS viacep:*

# Ver dados de um CEP específico
GET viacep:01310100

# Ver tempo de expiração (TTL)
TTL viacep:01310100

# Remover um CEP específico
DEL viacep:01310100

# Limpar todo o cache
FLUSHDB
```

### Exemplo de Saída

```bash
127.0.0.1:6379> KEYS viacep:*
1) "viacep:01310100"
2) "viacep:20040020"
3) "viacep:30130100"

127.0.0.1:6379> GET viacep:01310100
"{\"cep\":\"01310-100\",\"logradouro\":\"Avenida Paulista\",\"bairro\":\"Bela Vista\",\"localidade\":\"S\u00e3o Paulo\",\"uf\":\"SP\"}"

127.0.0.1:6379> TTL viacep:01310100
(integer) 86340  # Tempo restante em segundos (~24h)
```

## 📊 Estrutura de Dados

### Chave
```
Formato: viacep:{cep_normalizado}
Exemplo: viacep:01310100
```

### Valor (JSON)
```json
{
  "cep": "01310-100",
  "logradouro": "Avenida Paulista",
  "complemento": "",
  "bairro": "Bela Vista",
  "localidade": "São Paulo",
  "uf": "SP",
  "ibge": "3550308",
  "gia": "1004",
  "ddd": "11",
  "siafi": "7107",
  "erro": false
}
```

## 🛡️ Resiliência

### Tratamento de Erros

- ❌ **Erro no Redis** → Sistema continua funcionando (fallback para ViaCEP)
- ❌ **Erro no ViaCEP** → Usa dados fornecidos pelo usuário
- ❌ **CEP inválido** → NÃO é armazenado no cache

### Validações

✅ CEPs com erro (`data.Erro == true`) não são cacheados  
✅ Apenas respostas bem-sucedidas são armazenadas  
✅ Erros de cache não quebram o fluxo principal  

## 📈 Escalabilidade

### Recomendações para Produção

1. **Redis em Cluster**
   - Alta disponibilidade
   - Replicação automática
   - Failover automático

2. **Azure Redis Cache**
   ```json
   {
	 "ConnectionStrings": {
	   "Redis": "{nome}.redis.cache.windows.net:6380,password={chave},ssl=true"
	 }
   }
   ```

3. **Monitoramento**
   - Application Insights para telemetria
   - Alertas de hit/miss rate
   - Métricas de latência

## 🔧 Troubleshooting

### Redis não conecta

**Erro**: `It was not possible to connect to the redis server(s)`

**Soluções**:
1. Verificar se Redis está rodando: `docker ps` ou `redis-cli ping`
2. Verificar connection string no `appsettings.json`
3. Verificar firewall: `telnet localhost 6379`

### Cache não funciona

**Sintomas**: Todos os logs mostram "Cache MISS"

**Verificações**:
1. Redis está rodando?
2. `IViaCepCacheService` está registrado?
3. Connection string está correta?
4. Verificar logs de erro

## 📚 Documentação Adicional

- **Detalhes técnicos**: [REDIS_CACHE.md](./REDIS_CACHE.md)
- **Integração ViaCEP**: [VIACEP_INTEGRATION.md](./VIACEP_INTEGRATION.md)
- **Swagger**: [SWAGGER.md](./SWAGGER.md)

## 🎓 Conceitos Utilizados

- **Cache-Aside Pattern**: Padrão de cache sob demanda
- **Distributed Cache**: Cache compartilhado entre instâncias
- **TTL (Time To Live)**: Tempo de expiração automática
- **Key Normalization**: Normalização de chaves para consistência
- **Graceful Degradation**: Sistema continua funcionando em caso de falha do cache

## ✅ Checklist de Implantação

- [x] Pacote NuGet `Microsoft.Extensions.Caching.StackExchangeRedis` instalado
- [x] Interface `IViaCepCacheService` criada
- [x] Implementação `ViaCepCacheService` criada
- [x] `AddressService` integrado com cache
- [x] Configuração Redis em `appsettings.json`
- [x] Registro de serviços em `AddressModuleConfig`
- [x] Testes HTTP criados
- [x] Documentação completa
- [x] Build bem-sucedida ✅

## 🚀 Próximos Passos (Opcional)

1. Criar endpoint para invalidar cache manualmente
2. Implementar cache warming para CEPs mais usados
3. Adicionar endpoint de estatísticas de cache
4. Implementar distributed lock para evitar thundering herd
5. Adicionar cache de endereços completos (não apenas ViaCEP)
