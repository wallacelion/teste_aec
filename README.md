# avaliacao_dev

Avaliação técnica com backend em ASP.NET Core e banco SQL Server, organizada para separar responsabilidades por módulo de negócio e manter a infraestrutura compartilhada em um núcleo comum.

## Arquitetura adotada

O backend segue uma abordagem de monólito modular com camadas por responsabilidade. Cada módulo de negócio fica isolado em sua própria pasta dentro de `api/src/modules`, com separação entre:

- `domain`: entidades e contratos de domínio;
- `application`: DTOs, interfaces e serviços de aplicação;
- `infrastructure`: persistência, contexto de dados e registro de dependências;
- `controller`: ponto de entrada HTTP para cada recurso.

Os módulos atuais são `user`, `address` e `auth`, enquanto preocupações transversais ficam em `api/src/shared`, como `AppDbContext`, configuração de injeção de dependência e middleware de logging.

Na prática, isso caracteriza uma arquitetura em camadas dentro de um único deploy, com divisão por contexto funcional. O frontend está desacoplado em uma aplicação separada com React + Vite em `app`, mas ainda se encontra no estágio inicial de scaffold e não representa uma interface integrada ao backend.

## Justificativa das principais decisões técnicas

### 1. Monólito modular em vez de microsserviços

Para o escopo da avaliação, a escolha reduz custo operacional, simplifica setup local e mantém o código organizado por domínio. O projeto ganha separação lógica suficiente para evoluir sem assumir a complexidade de deploy, observabilidade distribuída e comunicação entre serviços.

### 2. ASP.NET Core com injeção de dependência nativa

O `Program.cs` usa o modelo moderno de hospedagem mínima do ASP.NET Core, com composição explícita dos módulos. Isso reduz boilerplate, facilita leitura do bootstrap da aplicação e aproveita os recursos nativos de DI, configuração e middleware.

### 3. Entity Framework Core com SQL Server

O uso de EF Core acelera a implementação de persistência, migrações e mapeamento objeto-relacional. Como o ambiente já sobe SQL Server via Docker Compose, a escolha mantém alinhamento entre infraestrutura local e modelagem do backend.

### 4. Autenticação baseada em JWT

O módulo de autenticação gera tokens stateless, o que favorece APIs HTTP e reduz dependência de sessão no servidor. Para uma aplicação orientada a consumo por cliente web ou integrações, esse modelo é simples de operar e escalar.

### 5. Docker Compose para o ambiente local

O `docker-compose.yml` sobe o banco SQL Server e a API com configuração centralizada. Isso reduz diferenças entre ambientes de desenvolvimento e facilita avaliação ou onboarding rápido.

### 6. Middleware de logging orientado por atributo

O projeto já introduz uma preocupação transversal de observabilidade com `LogRequestMiddleware` e o atributo `SaveLog`. A decisão é interessante porque evita logar indiscriminadamente todas as rotas e permite controle mais fino por endpoint.

## Bibliotecas utilizadas e seus motivos

### Backend

- `Microsoft.EntityFrameworkCore`: abstração ORM para persistência e migrações.
- `Microsoft.EntityFrameworkCore.SqlServer`: provedor relacional compatível com o banco adotado no ambiente Docker.
- `Microsoft.EntityFrameworkCore.Tools`: suporte a operações de migração durante o desenvolvimento.
- `Microsoft.AspNetCore.Authentication.JwtBearer`: base oficial para autenticação com bearer token em APIs ASP.NET.
- `JWT` e `System.IdentityModel.Tokens.Jwt`: geração de tokens JWT e composição de claims.
- `Microsoft.AspNetCore.Identity` (usada no código): hashing e verificação de senha com `PasswordHasher`, evitando armazenamento de senha em texto puro.

### Frontend

- `React`: base da interface SPA.
- `Vite`: servidor de desenvolvimento e build rápido, adequado para projetos frontend modernos.
- `ESLint`: padronização de código e prevenção de erros simples no cliente.

## Trade-offs considerados

### 1. Simplicidade operacional vs. isolamento forte

O monólito modular é mais simples de manter do que uma arquitetura distribuída, mas o isolamento entre módulos ainda depende de disciplina de código. Isso é visível no uso de infraestrutura compartilhada e no banco único.

### 2. Produtividade com EF Core vs. maior controle manual

EF Core acelera entregas, porém traz acoplamento ao ORM e pode exigir atenção extra em consultas, rastreamento de entidades e tuning de performance quando o sistema crescer.

### 3. Banco único vs. autonomia por contexto

Há um `AppDbContext` compartilhado e também contextos por módulo. Isso ajuda a evoluir o desenho, mas hoje gera sobreposição conceitual: a solução sugere separação modular, enquanto a persistência ainda permanece centralizada no mesmo banco e parcialmente duplicada.

### 4. JWT stateless vs. responsabilidades adicionais de segurança

JWT elimina sessão server-side, mas exige cuidado maior com expiração, rotação de segredo, revogação e configuração correta do pipeline de autenticação/autorização.

### 5. Setup rápido vs. segurança de configuração

O projeto prioriza facilidade de execução local, mas hoje há segredos e connection strings definidos diretamente em arquivos de configuração e no `docker-compose.yml`. Isso é aceitável para avaliação, mas inadequado para produção.

### 6. Entrega focada no backend vs. experiência ponta a ponta

Existe um frontend separado, porém ainda não integrado ao fluxo principal da API. O trade-off foi concentrar esforço na estrutura do backend, ao custo de uma demonstração incompleta do uso fim a fim.

## Melhorias que seriam implementadas em produção

### 1. Segurança e gestão de segredos

- remover segredos versionados do repositório;
- usar variáveis de ambiente seguras, secret manager ou cofre como Azure Key Vault;
- configurar autenticação JWT completa no pipeline com `AddAuthentication`, `UseAuthentication` e políticas de autorização.

### 2. Qualidade e testes

- adicionar testes unitários para serviços e regras de domínio;
- adicionar testes de integração para controllers e persistência;
- substituir a validação manual via arquivos `.http` por uma suíte automatizada.

### 3. Observabilidade

- incluir logs estruturados com correlação por request;
- adicionar health checks, métricas e tracing distribuído;
- integrar com ferramentas como OpenTelemetry, Application Insights, Grafana ou similares.

### 4. Robustez de API

- padronizar tratamento global de exceções;
- adicionar validação de entrada com filtros ou bibliotecas específicas;
- reabilitar e publicar documentação OpenAPI/Swagger para facilitar consumo.

### 5. Evolução arquitetural

- consolidar a estratégia de persistência entre contextos compartilhados e contextos modulares;
- introduzir contratos mais consistentes entre controllers, services e repositories;
- avaliar separação real por bounded context caso o domínio cresça.

### 6. Frontend e entrega contínua

- substituir o scaffold atual do React por uma interface integrada ao backend;
- definir estratégia de estado, autenticação e consumo da API no cliente;
- criar pipeline CI/CD com build, lint, testes e deploy automatizado.

## Resumo

O projeto demonstra uma direção arquitetural sólida para uma avaliação técnica: backend modular, uso de camadas, persistência com EF Core, autenticação por JWT e ambiente local conteinerizado. O principal ponto de evolução está em transformar essa base em uma solução pronta para produção, com fechamento das lacunas de segurança, testes, observabilidade e integração frontend-backend.
