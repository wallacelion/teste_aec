# 📍 Integração ViaCEP - Módulo Address

## 📋 Visão Geral

O módulo **Address** possui integração completa com a API **ViaCEP** (https://viacep.com.br/) para preenchimento automático de endereços a partir do CEP.

A integração implementa **políticas de resiliência** incluindo:
- ✅ **Timeout** (5 segundos)
- ✅ **Retry** com exponential backoff (3 tentativas)
- ✅ **Fallback** (dados manuais quando ViaCEP está offline)

## 🏗 Arquitetura

```
AddressController
	↓
AddressService (lógica de negócio)
	↓
ViaCepService (integração externa)
	↓ (com Polly)
ViaCEP API (https://viacep.com.br/)
```

### Componentes

1. **ViaCepService** (`infrastructure/external/services/ViaCepService.cs`)
   - Responsável pela comunicação com a API ViaCEP
   - Implementa timeout, retry e fallback usando **Polly**

2. **IViaCepService** (`infrastructure/external/interfaces/IViaCepService.cs`)
   - Interface do serviço de integração

3. **ViaCepResponseDto** (`infrastructure/external/dtos/ViaCepResponseDto.cs`)
   - DTO que representa a resposta da API ViaCEP

4. **AddressService** (`application/services/AddressService.cs`)
   - Usa ViaCepService para enriquecer dados de endereço

## 🚀 Como Funciona

### 1. CREATE (POST /api/address)

#### Cenário A: CEP válido + ViaCEP disponível

**Request:**
```json
{
  "userId": 1,
  "cep": "01310-100",
  "number": "1000",
  "complemento": "Sala 101"
}
```

**Fluxo:**
1. Sistema consulta ViaCEP para CEP `01310-100`
2. ViaCEP retorna:
   ```json
   {
	 "cep": "01310-100",
	 "logradouro": "Avenida Paulista",
	 "bairro": "Bela Vista",
	 "localidade": "São Paulo",
	 "uf": "SP"
   }
   ```
3. Sistema **preenche automaticamente** os campos vazios
4. Resultado salvo no banco:
   ```json
   {
	 "id": 1,
	 "userId": 1,
	 "cep": "01310-100",
	 "logradouro": "Avenida Paulista",
	 "number": "1000",
	 "complemento": "Sala 101",
	 "bairro": "Bela Vista",
	 "localidade": "São Paulo",
	 "uf": "SP"
   }
   ```

#### Cenário B: Dados manuais têm prioridade

**Request:**
```json
{
  "userId": 1,
  "cep": "01310-100",
  "logradouro": "Meu Logradouro Custom",
  "number": "2000",
  "bairro": "Meu Bairro",
  "localidade": "São Paulo",
  "uf": "SP"
}
```

**Fluxo:**
1. Sistema consulta ViaCEP
2. ViaCEP retorna dados padrão
3. Sistema **usa dados fornecidos pelo usuário** (têm prioridade!)
4. Resultado salvo no banco usa os dados fornecidos

#### Cenário C: ViaCEP offline (fallback)

**Request:**
```json
{
  "userId": 1,
  "cep": "01310-100",
  "logradouro": "Rua Manual",
  "number": "123",
  "bairro": "Bairro Manual",
  "localidade": "Cidade",
  "uf": "SP"
}
```

**Fluxo:**
1. Sistema tenta consultar ViaCEP
2. Após 3 tentativas com timeout de 5s cada, falha
3. Log: `"ViaCEP indisponível para CEP 01310-100. Usando dados fornecidos pelo usuário."`
4. Sistema **usa dados manuais** como fallback
5. Endereço é criado normalmente com os dados fornecidos

### 2. UPDATE (PUT /api/address/{id})

O comportamento é **idêntico ao CREATE**:
- Consulta ViaCEP novamente para o CEP fornecido
- Dados manuais têm prioridade
- Fallback para dados fornecidos se ViaCEP estiver offline

### 3. Consulta Direta (GET /api/address/viacep/{cep})

Endpoint adicional para consultar ViaCEP diretamente.

**Request:**
```http
GET /api/address/viacep/01310-100
Authorization: Bearer {token}
```

**Response (sucesso):**
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

**Response (CEP não encontrado):**
```json
{
  "error": "CEP não encontrado ou serviço ViaCEP temporariamente indisponível"
}
```

## ⚙️ Configuração

### Pacotes Utilizados

```xml
<PackageReference Include="Polly" Version="8.5.0" />
```

### Registro no IoC

Em `AddressModuleConfig.cs`:

```csharp
// Registra HttpClient para ViaCepService
services.AddHttpClient<IViaCepService, ViaCepService>();

// Registra serviços
services.AddScoped<IAddressService, AddressService>();
```

### Políticas de Resiliência (Polly)

#### Timeout
```csharp
Timeout: 5 segundos por tentativa
Strategy: Pessimistic (cancela a operação)
```

#### Retry
```csharp
Max Retries: 3
Backoff: Exponencial (2^n segundos)
  - Tentativa 1: Imediato
  - Tentativa 2: Aguarda 2s
  - Tentativa 3: Aguarda 4s
```

#### Exceções Tratadas
- `HttpRequestException` - Erros de rede
- `TaskCanceledException` - Timeout
- `TimeoutException` - Timeout explícito

## 📊 Logs

O sistema registra logs detalhados:

### Sucesso
```
[Information] Consultando CEP 01310-100 na API ViaCEP
[Information] CEP 01310-100 consultado com sucesso
[Information] Dados obtidos do ViaCEP para CEP 01310-100
[Information] Endereço 1 criado com sucesso para usuário 123
```

### Retry
```
[Warning] Tentativa 1 de 3 falhou ao consultar CEP 01310-100. 
		  Aguardando 2s antes de tentar novamente.
```

### Fallback
```
[Warning] ViaCEP indisponível para CEP 01310-100. 
		  Usando dados fornecidos pelo usuário.
[Warning] Usando fallback para CEP 01310-100. 
		  Os dados de endereço deverão ser preenchidos manualmente.
```

### Erro
```
[Error] Timeout ao consultar CEP 01310-100 após 5s
[Error] Erro de rede ao consultar CEP 01310-100
```

## 🧪 Testes

### Arquivo de Testes

Utilize o arquivo `src/tests/address-viacep.http` para testar todos os cenários.

### Casos de Teste Principais

1. ✅ **CEP válido** - Deve preencher campos automaticamente
2. ✅ **CEP válido + dados manuais** - Dados manuais têm prioridade
3. ✅ **CEP inválido** - Deve usar fallback (dados manuais)
4. ✅ **ViaCEP offline** - Deve usar fallback após 3 retries
5. ✅ **Consulta direta** - GET /viacep/{cep}
6. ✅ **Update** - Consulta ViaCEP novamente

## 🔧 Validação de CEP

### Formato Aceito

- `12345678` (apenas números)
- `12345-678` (com hífen)

### Validação

```csharp
bool IsValidCep(string cep)
{
	// Remove formatação
	var cleanCep = Regex.Replace(cep, @"[^\d]", string.Empty);

	// Valida 8 dígitos
	return Regex.IsMatch(cleanCep, @"^\d{8}$");
}
```

## 📝 Boas Práticas

### ✅ Recomendações

1. **Sempre forneça o número do imóvel**
   - ViaCEP não retorna número
   - Campo `number` é obrigatório

2. **Forneça complemento quando necessário**
   - Apartamento, sala, bloco, etc.

3. **Dados manuais têm prioridade**
   - Se você sabe que ViaCEP está desatualizado, forneça os dados corretos

4. **Use o endpoint de consulta antes de criar**
   - Valide o CEP antes do cadastro
   - GET `/api/address/viacep/{cep}`

### ❌ Evite

1. **Não criar endereços sem CEP**
   - Sistema espera CEP válido

2. **Não confiar 100% no ViaCEP**
   - Sempre permita que usuário sobrescreva dados

3. **Não ignorar logs de fallback**
   - Se ViaCEP está sempre offline, investigue

## 🚨 Troubleshooting

### ViaCEP sempre retorna timeout

**Causa:** API ViaCEP pode estar com problemas ou rede bloqueada

**Solução:**
1. Verifique conectividade com `https://viacep.com.br/`
2. Logs mostrarão tentativas de retry
3. Sistema usará fallback automaticamente

### CEP válido retorna 404

**Causa:** CEP pode não existir na base do ViaCEP

**Solução:**
- Forneça dados manualmente
- Verifique se CEP está correto

### Timeout muito curto/longo

**Configuração atual:** 5 segundos

**Alterar em:** `ViaCepService.cs`
```csharp
private const int TIMEOUT_SECONDS = 5; // Ajustar aqui
```

### Muitos retries

**Configuração atual:** 3 tentativas

**Alterar em:** `ViaCepService.cs`
```csharp
private const int MAX_RETRIES = 3; // Ajustar aqui
```

## 📚 Referências

- [ViaCEP - Documentação Oficial](https://viacep.com.br/)
- [Polly - Biblioteca de Resiliência](https://github.com/App-vNext/Polly)
- [Exemplo de uso do ViaCEP](https://viacep.com.br/ws/01310100/json/)

## 🔄 Fluxograma

```
┌─────────────────────────┐
│ Cliente envia request   │
│ POST /api/address       │
└───────────┬─────────────┘
			│
			▼
┌─────────────────────────┐
│ AddressService recebe   │
│ e valida dados          │
└───────────┬─────────────┘
			│
			▼
┌─────────────────────────┐
│ Consulta ViaCEP         │
│ (timeout: 5s, retry: 3) │
└───────────┬─────────────┘
			│
	  ┌─────┴─────┐
	  │           │
	Sucesso    Falha
	  │           │
	  ▼           ▼
┌─────────┐  ┌────────────┐
│ Preenche│  │  Fallback  │
│ campos  │  │ (usa dados │
│ do      │  │  forneci-  │
│ ViaCEP  │  │   dos)     │
└────┬────┘  └─────┬──────┘
	 │             │
	 └──────┬──────┘
			│
			▼
┌─────────────────────────┐
│ Mescla com dados do     │
│ request (manual tem     │
│ prioridade)             │
└───────────┬─────────────┘
			│
			▼
┌─────────────────────────┐
│ Salva no banco de dados │
└───────────┬─────────────┘
			│
			▼
┌─────────────────────────┐
│ Retorna AddressDto      │
│ (201 Created)           │
└─────────────────────────┘
```

---

**Desenvolvido para Avaliação AEC**  
Repositório: https://github.com/wallacelion/avaliacao_dev
