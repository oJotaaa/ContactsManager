# Contacts Manager

## 📋 Visão Geral

**Contacts Manager** é uma aplicação web para gerenciamento de contatos, desenvolvida com **ASP.NET Core MVC** e **.NET 10**. O projeto demonstra boas práticas de engenharia de software, incluindo Clean Architecture, padrões de design, testes automatizados e conformidade com princípios SOLID.

A aplicação permite que usuários autenticados criem, leiam, atualizem e deletem contatos, com suporte a filtros avançados, ordenação, exportação em CSV/Excel e geração de relatórios em PDF.

---

## 🎯 Funcionalidades Principais

### Gestão de Contatos
- **CRUD Completo**: Criar, listar, editar e deletar contatos
- **Busca e Filtros**: Filtrar contatos por múltiplos critérios
- **Ordenação Dinâmica**: Ordenar contatos por qualquer campo (crescente/decrescente)
- **Exportação de Dados**:
  - Baixar contatos em formato CSV
  - Gerar planilhas Excel com formatação
  - Gerar relatórios em PDF
- **Upload em Lote**: Importar contatos através de arquivo Excel
- **Validação de Dados**: Validação robusta no lado servidor e cliente

### Autenticação e Autorização
- **Sistema de Registro**: Cadastro de novos usuários com validações
- **Autenticação**: Login seguro com ASP.NET Core Identity
- **Autorização**: Acesso restrito apenas para usuários autenticados
- **Gerenciamento de Senhas**: Políticas de senha configuráveis

### Funcionalidades Técnicas
- **Logging Estruturado**: Sistema de logs completo com Serilog
- **Tratamento de Exceções**: Middleware centralizado para erros
- **Filtros de Ação**: Filtros customizados para validação e processamento

---

## 🏗️ Arquitetura e Estrutura

O projeto segue **Clean Architecture** com separação clara de responsabilidades e a regra de dependência respeitada: as camadas internas nunca conhecem as externas.

```
ContactsManager/
├── src/
│   ├── ContactsManager.UI/                  # Apresentação (Controllers, Views, Filters, Middleware)
│   ├── ContactsManager.Core/                # Lógica de negócio (sem dependências externas)
│   │   ├── Domain/
│   │   │   ├── Entities/                    # Entidades de domínio (Person, Country)
│   │   │   └── IdentityEntities/            # Entidades de autenticação
│   │   ├── DTO/                             # Data Transfer Objects
│   │   ├── ServiceContracts/                # Interfaces de serviços
│   │   ├── Enums/                           # Enumerações do domínio
│   │   └── Services/                        # Implementação dos serviços
│   └── ContactsManager.Infrastructure/      # Acesso a dados e recursos externos
│       ├── DbContext/                       # Entity Framework Core DbContext
│       ├── Repositories/                    # Padrão Repository
│       └── Migrations/                      # Migrations do Entity Framework
└── tests/
    ├── ContactsManager.ControllerTests/     # Testes unitários dos controllers
    ├── ContactsManager.ServiceTests/        # Testes dos serviços
    └── ContactsManager.IntegrationTests/    # Testes de integração
```

### Padrões de Design Implementados

1. **Repository Pattern**: Abstração de acesso a dados através de interfaces
2. **Dependency Injection**: Injeção de dependências com IoC Container
3. **Service Layer**: Lógica de negócio isolada em serviços especializados
4. **DTO (Data Transfer Objects)**: Separação entre modelos de domínio e transferência de dados
5. **Middleware Pattern**: Tratamento centralizado de exceções

---

## 🛠️ Stack Tecnológico

### Framework e Runtime
- **[.NET 10](https://dotnet.microsoft.com/)** - Runtime
- **[ASP.NET Core MVC](https://docs.microsoft.com/aspnet/core/mvc/)** - Framework web
- **[Razor Views](https://docs.microsoft.com/aspnet/core/mvc/views/razor/)** - Templates de apresentação

### Banco de Dados e ORM
- **[SQL Server](https://www.microsoft.com/sql-server/)** - Banco de dados relacional
- **[Entity Framework Core 10](https://docs.microsoft.com/ef/core/)** - ORM (Object-Relational Mapping)
- **Migrations**: Versionamento de schema do banco de dados

### Autenticação e Segurança
- **[ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity/)** - Sistema de autenticação e autorização
- **[CSRF Protection](https://docs.microsoft.com/aspnet/core/security/anti-csrf/)** - Proteção contra ataques CSRF via `AutoValidateAntiforgeryTokenAttribute`

### Logging e Observabilidade
- **[Serilog](https://serilog.net/)** - Logging estruturado
- **[Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore)** - Integração com ASP.NET Core
- **[Serilog.Sinks.MSSqlServer](https://github.com/serilog/serilog-sinks-mssqlserver)** - Persistência de logs em banco de dados
- **[Serilog.Sinks.Seq](https://github.com/serilog/serilog-sinks-seq)** - Visualização centralizada de logs

### Exportação de Dados
- **[Rotativa.AspNetCore](https://github.com/webce/Rotativa)** - Geração de PDFs
- **[EPPlus](https://www.epplussoftware.com/)** - Manipulação de arquivos Excel

### Testes Automatizados
- **[xUnit](https://xunit.net/)** - Framework de testes
- **[Moq](https://github.com/moq/moq4)** - Biblioteca de mocking
- **[AutoFixture](https://github.com/AutoFixture/AutoFixture)** - Geração automática de dados de teste
- **[FluentAssertions](https://fluentassertions.com/)** - Assertions mais legíveis

---

## 🚀 Funcionalidades Técnicas Destacadas

### Logging Estruturado
O projeto implementa logging estruturado em múltiplas camadas. Os logs são persistidos em:
- **Console** (desenvolvimento)
- **Arquivo** (rolling files por hora)
- **SQL Server** (análise centralizada)
- **Seq** (visualização em tempo real)

### Filtros de Ação (Action Filters)
Filtros customizados para processamento de requisições:
- `PersonsListActionFilter` - Pré-processamento da lista de contatos
- `PersonCreateAndEditPostActionFilter` - Validação de dados na criação/edição
- `TokenAuthorizationFilter` - Validação de tokens
- `FeatureDisabledResourceFilter` - Desabilitar features por configuração
- `HandleExceptionFilter` - Captura e tratamento de exceções

### Serviços Especializados
Em vez de um único `PersonsService` com todos os métodos, cada operação possui seu próprio serviço com responsabilidade única:

```csharp
public class PersonsController : Controller
{
    private readonly IPersonsGetterService _personsGetterService;
    private readonly IPersonsAdderService _personsAdderService;
    private readonly IPersonsSorterService _personsSorterService;
    private readonly IPersonsUpdaterService _personsUpdaterService;
    private readonly IPersonsDeleterService _personsDeleterService;
    // ...
}
```

---

## 📊 Estrutura de Dados

### Entidade: Person (Contato)
```csharp
public class Person
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public string? TIN { get; set; }

    [ForeignKey("CountryID")]
    public virtual Country? Country { get; set; }
}
```

### Entidade: Country (País)
```csharp
public class Country
{
    public Guid CountryID { get; set; }
    public string? CountryName { get; set; }
    public virtual ICollection<Person> Persons { get; set; }
}
```

### Entidade: ApplicationUser (Usuário)
```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    // Herda de IdentityUser<Guid>
    // Extensível para propriedades adicionais conforme necessário
}
```

---

## 🧪 Testes Automatizados

O projeto inclui três tipos de testes:

### 1. Testes Unitários de Controller
**Arquivo**: `tests/ContactsManager.ControllerTests/PersonsControllerTest.cs`

- Testa métodos dos controllers isoladamente
- Utiliza Moq para simular dependências
- Verifica comportamentos esperados sem acessar banco de dados

```csharp
[Fact]
public async Task Index_WithoutSearchParameters_ReturnsAllPersons()
{
    // Arrange
    var persons = new List<PersonResponse> { /* ... */ };
    _personsGetterServiceMock
        .Setup(s => s.GetFilteredPersons(It.IsAny<string>(), null))
        .ReturnsAsync(persons);

    // Act
    var result = await _controller.Index("", null);

    // Assert
    Assert.IsType<ViewResult>(result);
}
```

### 2. Testes de Serviço
**Arquivo**: `tests/ContactsManager.ServiceTests/PersonsServiceTest.cs`

- Testa lógica de negócio dos serviços
- Valida cálculos, transformações e regras de negócio
- Utiliza AutoFixture para geração de dados e FluentAssertions para legibilidade

### 3. Testes de Integração
**Arquivo**: `tests/ContactsManager.IntegrationTests/PersonsControllerIntegrationTest.cs`

- Testa o fluxo completo com banco de dados real
- Utiliza `CustomWebApplicationFactory` para configuração do ambiente de teste
- Simula requisições HTTP reais e valida comportamento end-to-end

---

## 📝 Configuração e Instalação

### Pré-requisitos
- **.NET 10 SDK** ou superior
- **SQL Server** 2019 ou posterior (ou LocalDB)
- **Visual Studio 2022** ou VS Code

### Passos de Instalação

1. **Clonar o repositório**
```bash
git clone https://github.com/oJotaaa/ContactsManager.git
cd ContactsManager
```

2. **Restaurar dependências**
```bash
dotnet restore
```

3. **Configurar banco de dados**

Edite a connection string em `src/ContactsManager.UI/appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ContactsDataBase;Integrated Security=True;"
}
```

4. **Aplicar migrations**
```bash
dotnet ef database update -p src/ContactsManager.Infrastructure -s src/ContactsManager.UI
```

5. **Executar a aplicação**
```bash
dotnet run --project src/ContactsManager.UI
```

A aplicação estará disponível em `https://localhost:5001` ou `http://localhost:5000`

---

## 🔐 Segurança

### Funcionalidades de Segurança Implementadas

1. **Autenticação com Identity**
   - Senhas hasheadas com PBKDF2
   - Políticas de senha configuráveis (comprimento mínimo, caracteres especiais etc.)

2. **Autorização**
   - Fallback policy global exige autenticação em toda a aplicação
   - Decoradores `[Authorize]` em controladores sensíveis
   - Endpoints de autenticação (login/registro) explicitamente liberados com `[AllowAnonymous]`

3. **Proteção CSRF**
   - Token anti-forgery automático em formulários
   - Validação global via `AutoValidateAntiforgeryTokenAttribute`

4. **HTTPS**
   - Redirecionamento automático para HTTPS
   - HSTS habilitado em produção

5. **Validação de Entrada**
   - Data Annotations em modelos
   - Validação no lado servidor antes de qualquer operação no banco

---

## 📈 Padrões de Desenvolvimento

### SOLID Principles — Aplicação Prática

#### **S** — Single Responsibility Principle (SRP)
Cada classe tem uma única responsabilidade bem definida. Em vez de um `PersonsService` monolítico, o domínio é dividido em serviços especializados:

```csharp
public interface IPersonsAdderService
{
    Task<PersonResponse> AddPerson(PersonAddRequest personRequest);
}

public interface IPersonsDeleterService
{
    Task<bool> DeletePerson(Guid? personID);
}

public interface IPersonsSorterService
{
    Task<List<PersonResponse>> GetSortedPersons(
        List<PersonResponse> allPersons,
        string sortBy,
        SortOrderOptions sortOrder);
}
```

#### **O** — Open/Closed Principle (OCP)
Novas implementações podem ser adicionadas sem modificar código existente:

```csharp
// Nova implementação pode ser criada sem alterar nenhum código já existente
public interface IPersonsRepository
{
    Task<List<Person>> GetFilteredPersons(string searchBy, string? searchString);
}

public class PersonsRepository : IPersonsRepository { /* implementação padrão */ }
```

#### **L** — Liskov Substitution Principle (LSP)
Subtipos podem substituir seus tipos base sem alterar o comportamento esperado:

```csharp
// Ambas as implementações são intercambiáveis no IoC container
services.AddScoped<IPersonsGetterService, PersonsGetterService>();
// ou
services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
```

#### **I** — Interface Segregation Principle (ISP)
Interfaces específicas por responsabilidade em vez de uma interface genérica única:

```csharp
// ✅ O controlador injeta apenas as dependências que realmente precisa
public PersonsController(
    IPersonsGetterService getterService,
    IPersonsAdderService adderService,
    IPersonsDeleterService deleterService
)

// ❌ Alternativa que foi evitada:
// public PersonsController(IPersonsService service) // tudo em um
```

#### **D** — Dependency Inversion Principle (DIP)
Dependências em abstrações, não em implementações concretas:

```csharp
// ✅ Controller depende de interface, nunca da classe concreta
public class PersonsController : Controller
{
    private readonly IPersonsGetterService _personsGetterService;
    private readonly ICountriesGetterService _countriesGetterService;

    public PersonsController(
        IPersonsGetterService personsGetterService,
        ICountriesGetterService countriesGetterService)
    {
        _personsGetterService = personsGetterService;
        _countriesGetterService = countriesGetterService;
    }
}
```

---

### Clean Architecture — Estrutura de Camadas

#### **1. UI Layer (Apresentação)**
Responsável por receber requisições e retornar respostas. Não contém lógica de negócio.
```
ContactsManager.UI/
├── Controllers/          # Roteamento e orquestração de requisições
├── Views/               # Templates Razor
├── Filters/             # Filtros de requisição (Action, Resource, Exception)
├── Middleware/          # Middleware customizado (ExceptionHandling, HttpLogging)
└── Program.cs           # Configuração de startup e DI
```

#### **2. Core Layer (Lógica de Negócio)**
Contém toda a lógica da aplicação. **Não possui nenhuma dependência de UI ou Infrastructure.**
```
ContactsManager.Core/
├── Domain/
│   ├── Entities/        # Modelos de domínio (Person, Country)
│   └── IdentityEntities/# Entidades de autenticação
├── DTO/                 # Objetos de transferência de dados entre camadas
├── ServiceContracts/    # Interfaces que definem o contrato dos serviços
├── Enums/              # Enumerações do domínio
└── Services/           # Implementação da lógica de negócio
```

#### **3. Infrastructure Layer (Acesso a Dados)**
Responsável pela comunicação com recursos externos (banco de dados, arquivos etc.).
```
ContactsManager.Infrastructure/
├── DbContext/           # Entity Framework Core DbContext
├── Repositories/        # Implementação do padrão Repository
└── Migrations/          # Versionamento de schema do banco
```

#### **Fluxo de Requisição**
```
UI → Services (Core) → Repository (Infrastructure) → Database
↑                                                         ↓
└─────────────────── Resposta em DTOs ←────────────────┘
```

A dependência sempre flui **para dentro**: UI conhece Core, Core não conhece ninguém. Infrastructure implementa contratos definidos pelo Core.

---

### Exemplos de Implementação Prática

#### Exemplo 1: Separação de Responsabilidades
```csharp
// ✅ Controller apenas orquestra — não contém lógica de negócio
[HttpGet]
[Route("[action]")]
public async Task<IActionResult> Index(string searchBy, string? searchString)
{
    List<PersonResponse> persons = await _personsGetterService
        .GetFilteredPersons(searchBy, searchString);

    return View(persons);
}

// ✅ Service encapsula lógica de negócio
public class PersonsGetterService : IPersonsGetterService
{
    private readonly IPersonsRepository _personsRepository;

    public async Task<List<PersonResponse>> GetFilteredPersons(
        string searchBy, string? searchString)
    {
        List<Person> persons = await _personsRepository
            .GetFilteredPersons(searchBy, searchString);

        return persons.Select(p => new PersonResponse
        {
            PersonID = p.PersonID,
            PersonName = p.PersonName,
            // ... mapeamento
        }).ToList();
    }
}

// ✅ Repository é responsável apenas por acesso a dados
public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _db;

    public async Task<List<Person>> GetFilteredPersons(
        string searchBy, string? searchString)
    {
        IQueryable<Person> query = _db.Persons;

        if (!string.IsNullOrEmpty(searchString))
        {
            query = ApplyFilter(query, searchBy, searchString);
        }

        return await query.ToListAsync();
    }
}
```

#### Exemplo 2: DTOs Isolam Camadas
```csharp
// ✅ A UI recebe apenas o DTO, nunca a entidade de domínio diretamente
public record PersonResponse
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    // ... apenas os dados necessários para a camada de apresentação
}

// ✅ A entidade de domínio permanece pura, sem acoplamento à UI
public class Person
{
    [Key]
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    [ForeignKey("CountryID")]
    public virtual Country? Country { get; set; }
}
```

#### Exemplo 3: Dependency Injection Centralizado
```csharp
// ✅ Toda configuração de DI centralizada em uma extension method
public static IServiceCollection ConfigureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Repositories
    services.AddScoped<ICountriesRepository, CountriesRepository>();
    services.AddScoped<IPersonsRepository, PersonsRepository>();

    // Services — cada um com responsabilidade específica
    services.AddScoped<IPersonsGetterService, PersonsGetterService>();
    services.AddScoped<IPersonsAdderService, PersonsAdderService>();
    services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
    services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
    services.AddScoped<IPersonsSorterService, PersonsSorterService>();

    // DbContext
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"));
    });

    return services;
}
```

---

## 📝 Notas de Configuração

### appsettings.json
O arquivo de configuração inclui:
- **Logging**: Múltiplos sinks (Console, File, SQL Server, Seq) com nível configurável
- **Banco de Dados**: Connection string para SQL Server / LocalDB
- **EPPlus**: Configuração de licença para manipulação de Excel
- **Serilog**: Enriquecimento de logs com informações de contexto

### Middleware Personalizado
- `ExceptionHandleMiddleware`: Captura e loga exceções não tratadas, retornando resposta padronizada
- `UseHttpLogging`: Log detalhado de requisições e respostas HTTP

### Filtros por Escopo
O projeto implementa filtros nos diferentes pontos do pipeline do ASP.NET:
- **Authorization Filters**: Validam permissões antes de qualquer processamento
- **Resource Filters**: Processam requisições antes do model binding (`FeatureDisabledResourceFilter`)
- **Action Filters**: Validam e transformam dados antes/depois das actions
- **Exception Filters**: Tratam exceções de forma centralizada
- **Result Filters**: Processam resultados antes de serem enviados ao cliente

---

## 📞 Contato

- **GitHub**: [github.com/oJotaaa/ContactsManager](https://github.com/oJotaaa/ContactsManager)
- **Licença**: MIT

---

*Desenvolvido como projeto de portfólio em .NET — 2026*
