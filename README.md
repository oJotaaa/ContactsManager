# Contacts Manager

Aplicação web ASP.NET Core MVC para gestão de contatos pessoais com autenticação, exportação de dados e logging estruturado.

---

## Visão Geral

O Contacts Manager é um CRUD de contatos desenvolvido sobre **.NET 10** e **ASP.NET Core MVC**, voltado para demonstrar técnicas de arquitetura limpa em um cenário real de aplicação empresarial. O sistema permite que usuários autenticados cadastrem, busquem, filtrem, ordenem e exportem registros de pessoas associadas a países, com geração de relatórios em PDF, CSV e Excel. O projeto foi construído como peça de portfólio para evidenciar domínio sobre separação de camadas, inversão de dependência, cobertura de testes e ferramentas de observabilidade no ecossistema .NET.

---

## Arquitetura e Decisões de Design

### Padrão arquitetônico: Clean Architecture (3 camadas)

O projeto adota uma arquitetura em três camadas inspirada nos princípios da Clean Architecture, onde a regra de dependência é respeitada: **camadas internas nunca referenciam camadas externas**. A motivação para essa escolha, em detrimento de uma abordagem mais simples como uma única API monolítica, é demonstrar como isolar regras de negócio do acoplamento a frameworks e bibliotecas de infraestrutura -- o que facilita testabilidade, migração de banco de dados e substituição de componentes sem reescrever lógica de domínio.

- **ContactsManager.Core**: camada de domínio e lógica de negócio. Contém entidades, DTOs, contratos de serviço e repositório, enums e implementações dos serviços. Não referencia nenhum pacote de infraestrutura (banco de dados, HTTP, sistema de arquivos). As dependências são apenas as bibliotecas auxiliares (CsvHelper, EPPlus, Serilog) que manipulam formatos de exportação e logging.
- **ContactsManager.Infrastructure**: camada de acesso a dados. Implementa os contratos de repositório definidos no Core, utilizando Entity Framework Core com SQL Server. Contém o DbContext, as migrações Code First e as classes concretas de repositório. É o único projeto que referencia o pacote `Microsoft.EntityFrameworkCore.SqlServer`.
- **ContactsManager.UI**: camada de apresentação em ASP.NET Core MVC com Razor Views. Contém Controllers, Views, Filtros e Middleware. Orquestra as requisições, mas **não contém lógica de negócio** -- apenas delega para os serviços injetados.

O dependency graph é unidirecional: `UI --> Core <-- Infrastructure`. O Core define os contratos; UI e Infrastructure os implementam ou consomem. Essa inversão garante que mudanças no mecanismo de persistência (ex: trocar SQL Server por PostgreSQL) não afetem as regras de negócio.

### Por que autenticação via cookies e não JWT

A aplicação é um MVC server-rendered tradicional, não uma SPA ou API REST. Nesse cenário, autenticação baseada em cookies com **ASP.NET Core Identity** é a escolha natural -- o cookie de autenticação é gerenciado automaticamente pelo framework, protegido contra CSRF via `AutoValidateAntiforgeryTokenAttribute` global, e integra-se nativamente com o sistema de roles e policies de autorização. JWT seria apropriado se houvesse um frontend desacoplado ou consumo mobile, o que não é o caso aqui.

### Por que EF Core Code First e não Database First

O modelo de domínio é a fonte da verdade. As entidades `Person` e `Country` são mapeadas via Fluent API (ex: renomeação da coluna `TIN` para `TaxIdentificationNumber`, restrição de tipo `varchar(8)`, valor default). As migrações versionam o schema do banco de forma auditável. Essa abordagem permite evoluir o modelo em código e propagar mudanças para o banco com `dotnet ef database update`, sem depender de scripts SQL manuais ou de um DBA para sincronização.

### Por que dividir os serviços por operação e não por entidade

Em vez de um único `IPersonsService` com métodos `Add`, `Get`, `Update`, `Delete`, `Sort`, `Export`, o contrato de Persons foi segregado em **seis interfaces** especializadas: `IPersonsAdderService`, `IPersonsGetterService`, `IPersonsUpdaterService`, `IPersonsDeleterService`, `IPersonsSorterService` e uma interface separada para upload (`ICountriesUploaderService`). Essa decisão aplica o **Interface Segregation Principle** (o `I` do SOLID): um controller que apenas lista contatos não precisa depender de métodos de deleção ou exportação. Além disso, a implementação `PersonsGetterServiceWithFewExcelFields` demonstra o **Decorator Pattern**: ela compõe `IPersonsGetterService` e sobrescreve apenas o método `GetPersonsExcel()` para exportar um subconjunto reduzido de colunas, sem alterar o serviço original.

### Por que DTOs separados para entrada e saída

Cada operação trafega objetos específicos: `PersonAddRequest`, `PersonUpdateRequest`, `PersonResponse`, `CountryAddRequest`, `CountryResponse`. A camada de apresentação nunca recebe ou envia entidades de domínio diretamente. Isso evita vazamento de campos sensíveis (ex: propriedades de navegação do EF), over-posting em atualizações e acoplamento da View a detalhes de schema do banco. A conversão entre entidade e DTO é feita via **métodos de extensão** (`ToPersonResponse()`, `ToCountry()`) definidos nas próprias classes de domínio e DTO, mantendo a lógica de mapeamento próxima dos tipos que ela relaciona.

### Observabilidade com Serilog em 4 sinks

A aplicação emite logs estruturados para quatro destinos simultaneamente: Console (desenvolvimento), arquivo em disco com rolling por hora e limite de 1 MB, tabela em SQL Server (`ContactsManagerLogs`) e Seq para visualização em tempo real. A escolha do Serilog (em vez do `Microsoft.Extensions.Logging` padrão) se deve ao modelo de **sinks desacopláveis** e ao suporte a enriquecimento automático de contexto. O projeto ainda utiliza a biblioteca `SerilogTimings` para medir a duração de operações de banco com o pattern `using (Operation.Time("..."))`, registrando métricas diretamente no log estruturado sem instrumentação manual com `Stopwatch`.

### Filtros customizados no pipeline MVC

O projeto implementa filtros em todos os cinco pontos de extensão do pipeline do ASP.NET Core MVC, demonstrando domínio sobre o ciclo de vida de uma requisição:

| Estágio | Filtro | Função |
|---|---|---|
| Authorization | `TokenAuthorizationFilter` | Bloqueia requisições sem cookie `Auth-Key` com valor esperado |
| Resource | `FeatureDisabledResourceFilter` | Retorna HTTP 501 se feature estiver desabilitada |
| Action | `PersonsListActionFilter` | Valida parâmetros de busca e ordenação; injeta `ViewData` para a View |
| Action | `PersonCreateAndEditPostActionFilter` | Intercepta falhas de ModelState e recarrega a lista de países antes de reexibir o formulário |
| Exception | `HandleExceptionFilter` | Loga exceções não tratadas e retorna mensagem em desenvolvimento |
| Result | `PersonsListResultFilter` | Adiciona header HTTP `Last-Modified` na resposta |
| Result | `TokenResultFilter` | Injeta cookie `Auth-Key` na resposta para uso do filtro de autorização |

### Middleware de exceção com diagnóstico

O `ExceptionHandleMiddleware` registra exceções usando `IDiagnosticContext` do Serilog, diferenciando entre exceção raiz e exceção interna no log, e relança o erro após registro -- delegando o tratamento final ao pipeline padrão do ASP.NET Core configurado com `UseExceptionHandler("/Error")` em produção.

---

## Stack Tecnológica

| Tecnologia | Versão | Finalidade |
|---|---|---|
| .NET SDK / Runtime | 10.0 | Plataforma de execução |
| ASP.NET Core MVC | 10.0 | Framework web com renderização server-side |
| Entity Framework Core | 10.0.3 | ORM e gerenciamento de migrações |
| SQL Server (LocalDB) | 2019+ | Banco de dados relacional |
| ASP.NET Core Identity | 10.0.3 | Autenticação, autorização e gestão de usuários |
| Serilog | 4.3.1 | Logging estruturado |
| Serilog.AspNetCore | 10.0.0 | Integração do Serilog com o pipeline HTTP |
| Serilog.Sinks.MSSqlServer | 9.0.3 | Persistência de logs em tabela SQL Server |
| Serilog.Sinks.Seq | 9.0.0 | Envio de logs para dashboard Seq |
| SerilogTimings | 3.1.0 | Medição de duração de operações no log |
| EPPlus | 8.4.2 | Leitura e escrita de arquivos Excel (.xlsx) |
| CsvHelper | 33.1.0 | Geração de arquivos CSV |
| Rotativa.AspNetCore | 1.4.0 | Geração de PDF via wkhtmltopdf |
| Newtonsoft.Json | 13.0.4 | Serialização JSON para seed data |
| xUnit | 2.9.3 | Framework de testes unitários e de integração |
| Moq | 4.20.72 | Mocking de dependências em testes |
| AutoFixture | 4.18.1 | Geração automática de dados de teste |
| FluentAssertions | 8.8.0 | Assertions fluentes e legíveis |
| Fizzler / HtmlAgilityPack | 1.3.1 / 1.2.1 | Parse e query de HTML em testes de integração |
| Coverlet | 8.0.0 | Cobertura de código para testes |

---

## Funcionalidades Principais

### Operações de negócio
- CRUD completo de contatos (nome, email, data de nascimento, gênero, país, endereço, newsletter)
- Busca textual por qualquer campo do contato (nome, email, gênero, país, endereço, data)
- Ordenação bidirecional (ASC/DESC) por coluna com indicador visual na interface
- Importação de países em lote a partir de arquivo Excel (.xlsx) com planilha nomeada "Countries"
- Exportação tabular nos formatos CSV, Excel (com cabeçalho estilizado) e PDF (via Rotativa/wkhtmltopdf orientação paisagem)
- Exibição de idade calculada a partir da data de nascimento no DTO de resposta

### Autenticação e autorização
- Registro de usuários com validação remota de email único e escolha de perfil (User/Admin)
- Login com Identity e cookie authentication, com suporte a ReturnUrl para redirecionamento pós-autenticação
- Roteamento condicional pós-login: usuários com role Admin são direcionados para área administrativa
- Fallback policy global: toda página exige usuário autenticado; apenas login, registro e página de erro são anônimos
- Área Admin acessível exclusivamente por usuários na role Admin
- Senhas com política configurada: mínimo 5 caracteres, exige maiúscula, minúscula e ao menos 3 caracteres únicos, permite símbolos não alfanuméricos

### Implementação técnica destacada
- Segregação de interface de serviço: cada operação (add, get, update, delete, sort, upload) possui contrato independente
- Decorator `PersonsGetterServiceWithFewExcelFields` que estende a exportação Excel com subconjunto reduzido de colunas sem modificar o serviço base
- Coluna `TIN` mapeada via Fluent API para `TaxIdentificationNumber` com tipo `varchar(8)` e valor default `ABC12345`
- Seed data de países e pessoas carregado de arquivos JSON durante a migração via `HasData()`
- Atributo `SkipFilter` como marcador para que o `PersonsAlwaysRunResultFilter` ignore actions específicas

---

## Estrutura do Projeto

```
ContactsManagerSolution.slnx                  # Arquivo de solução no formato .slnx
src/
  ContactsManager.Core/                        # Camada de domínio e regras de negócio
    Domain/
      Entities/                                # Entidades de domínio: Country, Person
      IdentityEntities/                        # Entidades Identity customizadas: ApplicationUser, ApplicationRole
      RepositoryContracts/                     # Interfaces de repositório (ICountriesRepository, IPersonsRepository)
    DTO/                                       # Objetos de transferência: AddRequest, UpdateRequest, Response, Register, Login
    Enums/                                     # GenderOptions, SortOrderOptions, UserTypeOptions
    Exceptions/                                # InvalidPersonIDException (exceção de domínio tipada)
    Helpers/                                   # ValidationHelper (validação por Data Annotations)
    ServiceContracts/                          # Interfaces segregadas: IAdder, IGetter, IUpdater, IDeleter, ISorter, IUploader
    Services/                                  # Implementações dos serviços e decorator PersonsGetterServiceWithFewExcelFields
  ContactsManager.Infrastructure/              # Camada de persistência e acesso a dados
    DbContext/                                 # ApplicationDbContext com configuração Fluent API e stored procedures
    Migrations/                                # Migrações Code First (Initial + IdentityTables) e snapshot
    Repositories/                              # Implementações concretas de repositórios (CountriesRepository, PersonsRepository)
  ContactsManager.UI/                          # Camada de apresentação MVC
    Areas/Admin/                               # Área administrativa com rotas e autorização por role
    Controllers/                               # AccountController, PersonsController, CountriesController, HomeController
    Filters/                                   # Filtros MVC customizados (Authorization, Resource, Action, Exception, Result)
    Middleware/                                # ExceptionHandleMiddleware com extensão de pipeline
    StartupExtensions/                         # Extension method ConfigureServices para registro de DI
    Views/                                     # Razor Views com layout compartilhado e partial views
    wwwroot/                                   # Arquivos estáticos: CSS, JS, imagens, wkhtmltopdf.exe
tests/
  ContactsManager.ControllerTests/             # Testes unitários de controllers com Moq e AutoFixture
  ContactsManager.ServiceTests/                # Testes de lógica de negócio dos serviços
  ContactsManager.IntegrationTests/            # Testes end-to-end com WebApplicationFactory e banco em memória
```

---

## Testes Automatizados

A suíte de testes está organizada em três projetos, cada um com responsabilidade distinta, somando cobertura em todas as camadas da aplicação.

### Testes unitários de Controller (`ContactsManager.ControllerTests`)

Testam os controllers de forma isolada, sem dependências reais de banco de dados ou serviços. Cada dependência do controller (serviços, logger) é simulada com **Moq**, permitindo verificar exclusivamente o comportamento de orquestração: se o controller delega para o serviço correto, se retorna a View esperada e se o modelo passado para a View corresponde ao que o serviço retornou. O **AutoFixture** gera objetos de teste anônimos, eliminando a necessidade de criar manualmente instâncias de DTOs e entidades para cada cenário. As assertions utilizam **FluentAssertions** para produzir mensagens de falha mais descritivas que os assertions nativos do xUnit.

### Testes unitários de Serviço (`ContactsManager.ServiceTests`)

Validam a lógica de negócio isoladamente, injetando mocks dos repositórios via Moq nos serviços concretos. Cada método de serviço é testado em cenários de sucesso, falha de validação (nome nulo, email inválido) e borda (lista vazia, ID inexistente). Os testes utilizam **AutoFixture** com `.Build<T>().With()` para customizar propriedades específicas (ex: forçar email em formato válido) enquanto randomiza as demais, evitando testes frágeis acoplados a valores fixos. As assertions com `FluentAssertions.Should().ThrowAsync<T>()` verificam que exceções tipadas são lançadas nos cenários de erro esperados.

### Testes de integração (`ContactsManager.IntegrationTests`)

Testam o fluxo completo da requisição HTTP até a resposta, simulando o ambiente real da aplicação. O `CustomWebApplicationFactory` estende `WebApplicationFactory<Program>` e sobrescreve `ConfigureWebHost` para:

- Definir o ambiente como `"Test"`, o que impede o registro do DbContext SQL Server real no `ConfigureServicesExtension`
- Registrar um **Entity Framework Core InMemoryDatabase** como substituto, permitindo que os testes de integração executem sem SQL Server instalado
- Manter o pipeline de middleware, filtros e autenticação idêntico ao de produção

Os testes realizam requisições HTTP reais com `HttpClient` e validam o HTML retornado usando **HtmlAgilityPack** para parsing e **Fizzler** para queries CSS (`document.QuerySelectorAll("table.persons")`). Isso permite verificar que elementos específicos da interface estão presentes na resposta renderizada, indo além da simples checagem de status code.

### Por que essa cobertura é relevante

A testabilidade do projeto é uma consequência direta da arquitetura em camadas. Como cada classe depende de abstrações (interfaces) e não de implementações concretas, é possível substituir qualquer dependência por um mock em testes unitários ou por uma implementação alternativa (InMemoryDatabase) em testes de integração. Essa característica não exige frameworks de mocking complexos nem alteração do código de produção para viabilizar testes -- os mesmos contratos que promovem desacoplamento no runtime habilitam isolamento nos testes. O uso de Coverlet permite medir objetivamente a cobertura de código e identificar pontos cegos na suíte.

---

## Como Executar Localmente

### Pré-requisitos

- .NET 10 SDK
- SQL Server 2019+ ou LocalDB (incluído no Visual Studio 2022)
- Visual Studio 2022 (recomendado) ou VS Code com extensão C# Dev Kit
- Seq (opcional, para dashboard de logs -- download em https://datalust.co/seq)

### Passos

**1. Clonar o repositório**

```bash
git clone https://github.com/oJotaaa/ContactsManager.git
cd ContactsManager
```

**2. Restaurar dependências**

```bash
dotnet restore
```

**3. Configurar a connection string**

Editar o arquivo `src/ContactsManager.UI/appsettings.json`, chave `ConnectionStrings.DefaultConnection`. O valor padrão utiliza LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ContactsDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False"
}
```

Se estiver usando SQL Server completo, ajustar `Data Source` para o nome da instância (ex: `localhost` ou `.\\SQLEXPRESS`).

**4. Aplicar migrações ao banco de dados**

```bash
dotnet ef database update -p src/ContactsManager.Infrastructure -s src/ContactsManager.UI
```

Isso criará o banco `ContactsDataBase` com as tabelas `Countries`, `Persons` e as tabelas do Identity (`AspNetUsers`, `AspNetRoles`, etc.), além de popular dados iniciais de países e contatos a partir dos arquivos `countries.json` e `persons.json`.

**5. Executar a aplicação**

```bash
dotnet run --project src/ContactsManager.UI
```

A aplicação estará disponível em `https://localhost:5001`.

**6. Criar um usuário**

Acessar `/Account/Register`, preencher o formulário e selecionar o tipo de usuário (User ou Admin). Após o registro, o login ocorre automaticamente.

### Executar os testes

```bash
dotnet test
```

Os testes de integração utilizam banco de dados em memória (`InMemoryDatabase`) e não exigem SQL Server.

---

## Variáveis de Ambiente e Configuração

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | String de conexão para o banco de dados principal (ContactsDataBase) |
| `EPPlus:ExcelPackage:License` | Chave de licença do EPPlus (preenchida para uso não comercial) |
| `Serilog:MinimumLevel` | Nível mínimo de log (padrão: `Information`) |
| `Serilog:WriteTo:File:Args:path` | Caminho do arquivo de log (padrão: `logs/log.txt`) |
| `Serilog:WriteTo:File:Args:rollingInterval` | Intervalo de rotação do arquivo (padrão: `Hour`) |
| `Serilog:WriteTo:File:Args:fileSizeLimitBytes` | Tamanho máximo por arquivo antes da rotação |
| `Serilog:WriteTo:MSSqlServer:Args:connectionString` | String de conexão para o banco de logs (ContactsManagerLogs) |
| `Serilog:WriteTo:MSSqlServer:Args:tableName` | Nome da tabela de logs (padrão: `logs`) |
| `Serilog:WriteTo:Seq:Args:serverUrl` | URL do servidor Seq (padrão: `http://localhost:5341`) |
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução (`Development`, `Production`, `Test`) |

Em ambiente `Test`, o DbContext SQL Server não é registrado -- os testes de integração fornecem seu próprio contexto em memória via `CustomWebApplicationFactory`.

---

## Referência de Rotas

| Método | Rota | Controlador | Autenticação | Descrição |
|---|---|---|---|---|
| GET | `/` ou `/Persons/Index` | PersonsController | Requerida | Lista de contatos com busca e ordenação |
| GET | `/Persons/Create` | PersonsController | Requerida | Formulário de criação de contato |
| POST | `/Persons/Create` | PersonsController | Requerida | Persiste novo contato |
| GET | `/Persons/Edit/{personID}` | PersonsController | Requerida | Formulário de edição de contato |
| POST | `/Persons/Edit/{personID}` | PersonsController | Requerida | Atualiza contato existente |
| GET | `/Persons/Delete/{personID}` | PersonsController | Requerida | Confirmação de exclusão |
| POST | `/Persons/Delete/{personID}` | PersonsController | Requerida | Remove contato |
| GET | `/Persons/PersonsPDF` | PersonsController | Requerida | Download da lista em PDF |
| GET | `/Persons/PersonsCSV` | PersonsController | Requerida | Download da lista em CSV |
| GET | `/Persons/PersonsExcel` | PersonsController | Requerida | Download da lista em Excel |
| GET | `/Countries/UploadFromExcel` | CountriesController | Requerida | Formulário de upload de países |
| POST | `/Countries/UploadFromExcel` | CountriesController | Requerida | Processa arquivo Excel com países |
| GET | `/Account/Login` | AccountController | Anônimo | Formulário de login |
| POST | `/Account/Login` | AccountController | Anônimo | Autentica usuário |
| GET | `/Account/Register` | AccountController | Anônimo | Formulário de registro |
| POST | `/Account/Register` | AccountController | Anônimo | Cria novo usuário |
| GET | `/Account/Logout` | AccountController | Requerida | Encerra sessão |
| GET | `/Account/IsEmailAlreadyRegistered` | AccountController | Anônimo | Validação remota de email (JSON) |
| GET | `/Admin/Home/Index` | Admin/HomeController | Admin | Painel administrativo |
| GET | `/Error` | HomeController | Anônimo | Página de erro |

---

## Limitações Conhecidas

### Limitações atuais

- **Lista sem paginação**: a tela de contatos carrega todos os registros de uma vez. Em volumes altos, isso compromete performance e usabilidade. A próxima iteração deve implementar paginação server-side com `Skip` e `Take`.
- **Seed data acoplado ao sistema de arquivos**: as migrações leem `countries.json` e `persons.json` do disco via `File.Exists()`. Se os arquivos não estiverem presentes no momento da migração, o seed falha silenciosamente. Ideal migrar para um seeder via `IApplicationBuilder` ou hosting startup.
- **Filtro de autorização com token fixo**: o `TokenAuthorizationFilter` valida um valor de cookie hardcoded (`"A100"`). Isso é um exercício didático, não um mecanismo seguro. O padrão correto em produção seria usar policies de autorização baseadas em claims.
- **Testes de integração com InMemoryDatabase**: o provider in-memory do EF Core não suporta operações relacionais completas (ex: constraints de chave estrangeira são ignoradas). Para testes de integração mais fiéis, recomenda-se migrar para um container de teste com SQL Server via Testcontainers.

---

## Autor

**João Felipe** -- Desenvolvedor .NET

- GitHub: [github.com/oJotaaa](https://github.com/oJotaaa)
- LinkedIn: [linkedin.com/in/joaofelipefernandes](https://www.linkedin.com/in/joaofelipefernandes/)

---

*Projeto de portfólio desenvolvido em 2026 com .NET 10 e ASP.NET Core MVC.*
