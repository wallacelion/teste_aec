# 🐳 Docker Compose - Ambiente de Desenvolvimento

## 📋 Visão Geral

Este arquivo `docker-compose.yml` configura um ambiente completo para desenvolvimento da API, incluindo:

- **SQL Server 2022**: Banco de dados principal
- **Redis 7**: Cache distribuído para ViaCEP
- **Redis Commander**: Interface web para visualizar e gerenciar o cache

## 🚀 Como Usar

### 1. Iniciar o Ambiente

```bash
# Navegar até a pasta do projeto
cd api

# Iniciar todos os serviços
docker-compose up -d

# Verificar status dos containers
docker-compose ps
```

### 2. Verificar Saúde dos Serviços

```bash
# Logs de todos os serviços
docker-compose logs -f

# Logs específicos
docker-compose logs -f redis
docker-compose logs -f database
docker-compose logs -f redis-commander
```

### 3. Parar o Ambiente

```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes (CUIDADO: remove dados!)
docker-compose down -v
```

## 📦 Serviços

### SQL Server (`database`)

- **Imagem**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Porta**: `1433`
- **Usuário**: `sa`
- **Senha**: `(9Xupi6I3fQ!77zAA/wgA/~rymn-HNMl`
- **Banco**: `avaliacaoaec` (criado automaticamente pela API)

**Connection String**:
```
Server=localhost;Database=avaliacaoaec;User Id=sa;Password=(9Xupi6I3fQ!77zAA/wgA/~rymn-HNMl;Encrypt=False;
```

### Redis (`redis`)

- **Imagem**: `redis:7-alpine`
- **Porta**: `6379`
- **Persistência**: AOF (Append Only File)
- **Memória Máxima**: 256MB
- **Política de Eviction**: `allkeys-lru` (remove chaves menos usadas)

**Connection String**:
```
localhost:6379
```

**Comandos Úteis**:
```bash
# Conectar ao Redis CLI
docker exec -it avaliacaoaec-redis redis-cli

# Verificar status
docker exec -it avaliacaoaec-redis redis-cli INFO

# Listar chaves de cache
docker exec -it avaliacaoaec-redis redis-cli KEYS "viacep:*"

# Ver conteúdo de uma chave
docker exec -it avaliacaoaec-redis redis-cli GET "viacep:01310100"

# Limpar todo o cache
docker exec -it avaliacaoaec-redis redis-cli FLUSHDB
```

### Redis Commander (`redis-commander`)

- **Imagem**: `rediscommander/redis-commander:latest`
- **Porta**: `8081`
- **URL**: http://localhost:8081

Interface web para:
- ✅ Visualizar todas as chaves do cache
- ✅ Inspecionar valores armazenados
- ✅ Ver TTL de cada chave
- ✅ Deletar chaves manualmente
- ✅ Monitorar comandos em tempo real

![Redis Commander](https://redis.com/wp-content/uploads/2020/03/redis-commander-1.png)

## 🔧 Configuração Avançada

### Ajustar Memória do Redis

Edite o arquivo `docker-compose.yml`:

```yaml
redis:
  command: redis-server --appendonly yes --maxmemory 512mb --maxmemory-policy allkeys-lru
```

### Adicionar Senha ao Redis

```yaml
redis:
  command: redis-server --requirepass sua_senha_aqui
```

E atualize o `appsettings.json`:
```json
{
  "ConnectionStrings": {
	"Redis": "localhost:6379,password=sua_senha_aqui"
  }
}
```

### Configurar Persistência

O Redis está configurado com **AOF (Append Only File)** para persistência:

```yaml
redis:
  command: redis-server --appendonly yes
  volumes:
	- redis-data:/data
```

## 📊 Monitoramento

### Verificar Uso de Memória do Redis

```bash
docker exec -it avaliacaoaec-redis redis-cli INFO memory
```

Saída exemplo:
```
used_memory:1048576
used_memory_human:1.00M
used_memory_peak:2097152
used_memory_peak_human:2.00M
maxmemory:268435456
maxmemory_human:256.00M
```

### Verificar Hit/Miss Ratio

```bash
docker exec -it avaliacaoaec-redis redis-cli INFO stats | grep keyspace
```

### Monitorar Comandos em Tempo Real

```bash
docker exec -it avaliacaoaec-redis redis-cli MONITOR
```

## 🧪 Testar Conexões

### Testar SQL Server

```bash
# Conectar via sqlcmd
docker exec -it avaliacaoaec-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P '(9Xupi6I3fQ!77zAA/wgA/~rymn-HNMl'

# Verificar bancos de dados
SELECT name FROM sys.databases;
GO
```

### Testar Redis

```bash
# Ping
docker exec -it avaliacaoaec-redis redis-cli ping
# Resposta: PONG

# Testar SET/GET
docker exec -it avaliacaoaec-redis redis-cli SET teste "valor"
docker exec -it avaliacaoaec-redis redis-cli GET teste
# Resposta: "valor"
```

## 🔄 Backup e Restore

### Backup do SQL Server

```bash
# Criar backup
docker exec -it avaliacaoaec-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P '(9Xupi6I3fQ!77zAA/wgA/~rymn-HNMl' \
  -Q "BACKUP DATABASE avaliacaoaec TO DISK='/var/opt/mssql/backup/avaliacaoaec.bak'"

# Copiar backup para o host
docker cp avaliacaoaec-sqlserver:/var/opt/mssql/backup/avaliacaoaec.bak ./backup/
```

### Backup do Redis

```bash
# Forçar snapshot
docker exec -it avaliacaoaec-redis redis-cli BGSAVE

# Copiar snapshot para o host
docker cp avaliacaoaec-redis:/data/dump.rdb ./backup/redis-dump.rdb
```

## 🛠️ Troubleshooting

### SQL Server não inicia

```bash
# Verificar logs
docker-compose logs database

# Verificar se a porta está em uso
netstat -an | findstr 1433  # Windows
lsof -i :1433              # Linux/Mac
```

### Redis não conecta

```bash
# Verificar logs
docker-compose logs redis

# Testar conexão
docker exec -it avaliacaoaec-redis redis-cli ping
```

### Redis Commander não carrega

```bash
# Verificar se Redis está rodando
docker-compose ps

# Reiniciar Redis Commander
docker-compose restart redis-commander
```

### Limpar Completamente o Ambiente

```bash
# Parar e remover tudo
docker-compose down -v

# Remover imagens (opcional)
docker rmi mcr.microsoft.com/mssql/server:2022-latest
docker rmi redis:7-alpine
docker rmi rediscommander/redis-commander:latest

# Limpar volumes órfãos
docker volume prune
```

## 📋 Checklist de Inicialização

1. ✅ Docker Desktop instalado e rodando
2. ✅ Executar `docker-compose up -d`
3. ✅ Aguardar ~30s para inicialização
4. ✅ Verificar status: `docker-compose ps`
5. ✅ Testar Redis: `docker exec -it avaliacaoaec-redis redis-cli ping`
6. ✅ Testar SQL Server: verificar logs para "SQL Server is now ready"
7. ✅ Acessar Redis Commander: http://localhost:8081
8. ✅ Executar API: `dotnet run` ou F5 no Visual Studio
9. ✅ Verificar migrations aplicadas no SQL Server

## 🎯 Fluxo de Trabalho Recomendado

```bash
# 1. Iniciar ambiente
docker-compose up -d

# 2. Aguardar health checks
docker-compose ps

# 3. Executar API
cd api
dotnet run

# 4. Rodar testes
# Executar requests em api/src/tests/address-cache.http

# 5. Monitorar cache
# Abrir http://localhost:8081

# 6. Ao finalizar
docker-compose down
```

## 🌐 URLs Úteis

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Redis Commander**: http://localhost:8081
- **SQL Server**: `localhost:1433`
- **Redis**: `localhost:6379`

## 📚 Referências

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server Docker Hub](https://hub.docker.com/_/microsoft-mssql-server)
- [Redis Docker Hub](https://hub.docker.com/_/redis)
- [Redis Commander GitHub](https://github.com/joeferner/redis-commander)
