# Integração Redis Cache - ViaCEP

## Visão Geral

Este documento descreve a implementação de cache Redis para otimizar consultas ao serviço ViaCEP no módulo de endereços da API.

## Objetivo

Reduzir a latência e o número de requisições à API externa ViaCEP através do armazenamento em cache dos resultados de consultas de CEP.

## Arquitetura

### Cache-Aside Pattern

A implementação utiliza o padrão **Cache-Aside** (também conhecido como **Lazy Loading**):

1. **Verificação de Cache**: Antes de consultar a API ViaCEP, o sistema verifica se os dados do CEP solicitado já estão no cache Redis.
2. **Cache Hit**: Se os dados estiverem em cache, eles são retornados imediatamente, evitando a chamada à API externa.
3. **Cache Miss**: Se os dados não estiverem em cache, a API ViaCEP é consultada.
4. **Armazenamento**: Após uma consulta bem-sucedida ao ViaCEP, os dados são armazenados no Redis para consultas futuras.

### Componentes

#### IViaCepCacheService
Interface que define as operações de cache:
- `GetAsync(string cep)`: Recupera dados do CEP do cache
- `SetAsync(string cep, ViaCepResponseDto data, int expirationMinutes)`: Armazena dados no cache
- `RemoveAsync(string cep)`: Remove dados do cache
- `ExistsAsync(string cep)`: Verifica se o CEP existe no cache

#### ViaCepCacheService
Implementação concreta usando `IDistributedCache` do ASP.NET Core com Redis:
- Serializa/deserializa objetos `ViaCepResponseDto` para JSON
- Normaliza chaves de cache removendo formatação do CEP
- Implementa tratamento de erros para não quebrar o fluxo principal
- Registra logs detalhados de operações de cache (hit/miss)

#### AddressService
Serviço de aplicação que orquestra o fluxo de criação/atualização de endereços:
- Verifica cache antes de chamar ViaCEP
- Armazena resultados bem-sucedidos no cache
- Mantém fallback para dados fornecidos pelo usuário

## Configuração

### appsettings.json

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

### Registro de Serviços (AddressModuleConfig.cs)

```csharp
// Registrar Redis Cache
services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = configuration.GetConnectionString("Redis");
	options.InstanceName = "ViaCep_";
});

// Registrar serviço de cache
services.AddScoped<IViaCepCacheService, ViaCepCacheService>();
```

## Tempo de Expiração (TTL)

- **Padrão**: 1440 minutos (24 horas)
- **Configurável**: Através do `appsettings.json`
- **Justificativa**: Dados de CEP raramente mudam, permitindo cache de longa duração

## Estrutura de Chave

- **Formato**: `viacep:{cep_sem_formatacao}`
- **Exemplo**: `viacep:01310100` para o CEP 01310-100
- **Normalização**: Remoção de hífens, pontos e espaços

## Formato de Dados Armazenados

Os dados são armazenados em formato JSON com a seguinte estrutura:

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

## Fluxo de Operações

### Criação de Endereço

```
1. Cliente envia POST /api/address com CEP
2. AddressService verifica cache via IViaCepCacheService.GetAsync(cep)
3a. Se CACHE HIT: usa dados do cache
3b. Se CACHE MISS:
	- Consulta ViaCepService.GetAddressByCepAsync(cep)
	- Se sucesso: armazena no cache via SetAsync(cep, data)
4. Mescla dados do ViaCEP com dados fornecidos pelo usuário
5. Salva endereço no banco de dados
6. Retorna AddressDto
```

### Atualização de Endereço

```
1. Cliente envia PUT /api/address/{id} com novo CEP
2. AddressService busca endereço existente
3. Verifica cache para o novo CEP
4. Segue o mesmo fluxo de cache-aside da criação
5. Atualiza endereço no banco de dados
6. Retorna AddressDto atualizado
```

## Logs

O sistema registra eventos detalhados de cache:

- **Cache HIT**: `Cache HIT para CEP {Cep}`
- **Cache MISS**: `Cache MISS para CEP {Cep}`
- **Armazenamento**: `CEP {Cep} armazenado no cache com expiração de {Expiration} minutos`
- **Erros**: `Erro ao recuperar CEP {Cep} do cache`

## Resiliência

### Tratamento de Erros de Cache

- Erros de cache **não quebram** o fluxo principal
- Em caso de falha no Redis, o sistema continua funcionando consultando diretamente o ViaCEP
- Logs de erro são registrados para monitoramento
- Retorna `null` em caso de erro, forçando consulta à API

### Validação de Dados

O cache **não armazena** dados inválidos:
- CEPs com erro (`data.Erro == true`) não são cacheados
- Apenas respostas bem-sucedidas do ViaCEP são armazenadas

## Benefícios

1. **Performance**: Redução de latência para CEPs consultados anteriormente
2. **Redução de Custos**: Menos chamadas à API externa ViaCEP
3. **Disponibilidade**: Dados em cache permanecem acessíveis mesmo se ViaCEP estiver indisponível
4. **Escalabilidade**: Redis distribui carga entre instâncias da API
5. **Observabilidade**: Logs detalhados de hit/miss rate

## Requisitos de Infraestrutura

### Redis Server

- **Versão mínima**: Redis 5.0+
- **Configuração recomendada**: 
  - Modo standalone para ambientes de desenvolvimento
  - Modo clustered para produção
  - Persistence habilitada (AOF ou RDB)
  - Memória: Estimativa de ~500 bytes por CEP

### Docker (Desenvolvimento)

```bash
docker run -d --name redis-viacep \
  -p 6379:6379 \
  redis:latest
```

### Conexão

A string de conexão suporta:
- **Local**: `localhost:6379`
- **Senha**: `localhost:6379,password=sua_senha`
- **SSL**: `localhost:6379,ssl=true`
- **Azure Redis**: `{nome}.redis.cache.windows.net:6380,password={chave},ssl=true`

## Monitoramento

### Métricas Recomendadas

1. **Hit Rate**: Percentual de requisições atendidas pelo cache
2. **Miss Rate**: Percentual de requisições que precisaram consultar ViaCEP
3. **Latência**: Tempo de resposta médio (cache vs API)
4. **Tamanho do Cache**: Número de chaves armazenadas
5. **Evictions**: Número de chaves removidas por política de memória

### Ferramentas

- **Redis CLI**: `redis-cli --scan --pattern "viacep:*"`
- **Redis Insight**: Interface gráfica para visualização
- **Application Insights**: Integração com Azure para telemetria

## Comandos Úteis

### Verificar Dados no Cache

```bash
# Conectar ao Redis
redis-cli

# Listar todas as chaves de CEP
KEYS viacep:*

# Ver dados de um CEP específico
GET viacep:01310100

# Ver TTL de uma chave
TTL viacep:01310100

# Remover um CEP do cache
DEL viacep:01310100

# Limpar todos os CEPs
FLUSHDB
```

## Testes

### Testar Cache Hit

```http
### 1. Primeira requisição (Cache Miss)
POST http://localhost:5000/api/address
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": "{{userUid}}",
  "cep": "01310-100",
  "number": "1000"
}

### 2. Segunda requisição com mesmo CEP (Cache Hit)
POST http://localhost:5000/api/address
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": "{{userUid}}",
  "cep": "01310-100",
  "number": "2000"
}
```

Observe os logs:
- Primeira: `Cache MISS para CEP 01310-100` → `CEP 01310-100 armazenado no cache`
- Segunda: `Cache HIT para CEP 01310-100`

### Testar Expiração

```bash
# Configurar TTL de 1 minuto para teste
# appsettings.Development.json
{
  "Cache": {
	"ViaCep": {
	  "ExpirationMinutes": 1
	}
  }
}
```

## Troubleshooting

### Redis não está conectado

**Erro**: `It was not possible to connect to the redis server(s)`

**Solução**:
1. Verificar se o Redis está rodando: `docker ps` ou `redis-cli ping`
2. Verificar connection string no `appsettings.json`
3. Verificar firewall/rede

### Cache não está funcionando

**Sintomas**: Todos os logs mostram "Cache MISS"

**Verificações**:
1. Verificar se o serviço `IViaCepCacheService` está registrado
2. Verificar se Redis está configurado corretamente
3. Verificar logs de erro do Redis

### Dados antigos no cache

**Solução**:
1. Reduzir TTL no `appsettings.json`
2. Limpar cache manualmente: `redis-cli FLUSHDB`
3. Implementar invalidação manual via endpoint (futuro)

## Evolução Futura

### Funcionalidades Planejadas

1. **Endpoint de Invalidação**: `DELETE /api/address/cache/{cep}`
2. **Cache Warming**: Pré-carregar CEPs mais comuns
3. **Cache Statistics**: Endpoint para métricas de hit/miss
4. **Distributed Lock**: Evitar thundering herd em cache miss
5. **Cache Versioning**: Invalidação automática ao mudar estrutura de dados

## Referências

- [Microsoft.Extensions.Caching.StackExchangeRedis](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed)
- [Redis Cache-Aside Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside)
- [StackExchange.Redis Documentation](https://stackexchange.github.io/StackExchange.Redis/)
