# Contacts Manager

## 📋 Visão Geral

**Contacts Manager** é uma aplicação web moderna para gerenciamento de contatos, desenvolvida com **ASP.NET Core Razor Pages** e **.NET 10**. O projeto demonstra boas práticas de engenharia de software, incluindo arquitetura em camadas, padrões de design, testes automatizados e conformidade com princípios SOLID.

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

### Funcionalidades Avançadas
- **Logging Estruturado**: Sistema de logs completo com Serilog
- **Tratamento de Exceções**: Middleware centralizado para erros
- **Filtros de Ação**: Filtros customizados para validação e processamento
- **Relatórios Administrativos**: Páginas de administração para gerenciar dados

---

## 🏗️ Arquitetura e Estrutura

O projeto segue uma **arquitetura em camadas** (Layered Architecture) com separação clara de responsabilidades:

```
ContactsManager/
├── src/
│   ├── ContactsManager.UI/                  # Apresentação (Controllers, Views, Razor Pages)
│   ├── ContactsManager.Core/                # Lógica de negócio e modelos
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
3. **Service Layer**: Lógica de negócio isolada em serviços
4. **DTO (Data Transfer Objects)**: Separação entre modelos de domínio e transfer
5. **Factory Pattern**: Utilizado em testes com `CustomWebApplicationFactory`
6. **Middleware Pattern**: Tratamento centralizado de exceções

---

## 🛠️ Stack Tecnológico

### Framework e Runtime
- **[.NET 10](https://dotnet.microsoft.com/)** - Runtime mais recente
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core/)** - Framework web
- **[Razor Pages](https://docs.microsoft.com/aspnet/core/razor-pages/)** - Modelo de apresentação

### Banco de Dados e ORM
- **[SQL Server](https://www.microsoft.com/sql-server/)** - Banco de dados relacional
- **[Entity Framework Core 10](https://docs.microsoft.com/ef/core/)** - ORM (Object-Relational Mapping)
- **Migrations**: Versionamento de schema do banco de dados

### Autenticação e Segurança
- **[ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity/)** - Sistema de autenticação e autorização
- **[Authorize Attribute](https://docs.microsoft.com/aspnet/core/security/authorization/simple/)** - Autorização declarativa
- **[CSRF Protection](https://docs.microsoft.com/aspnet/core/security/anti-csrf/)** - Proteção contra ataques CSRF

### Logging e Observabilidade
- **[Serilog](https://serilog.net/)** - Logging estruturado
- **[Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore)** - Integração com ASP.NET Core
- **[Serilog.Sinks.MSSqlServer](https://github.com/serilog/serilog-sinks-mssqlserver)** - Persistência em banco de dados
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
O projeto implementa logging estruturado em múltiplas camadas.

Logs são persistidos em:
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
O projeto utiliza uma abordagem de serviços especializados:

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

Cada serviço tem uma responsabilidade bem definida, seguindo o **Single Responsibility Principle (SRP)**.

### Autorização Declarativa
Proteção automática de rotas:
```csharp
[Authorize]
[Route("[controller]")]
public class PersonsController : Controller { }
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
    // Herança do IdentityUser<Guid>
    // Propriedades adicionais conforme necessário
}
```

---

## 🧪 Testes Automatizados

O projeto inclui **três tipos de testes**:

### 1. Testes Unitários de Controller
**Arquivo**: `tests/ContactsManager.ControllerTests/PersonsControllerTest.cs`

- Testa métodos dos controllers isoladamente
- Utiliza Moq para simular dependências
- Verifica comportamentos esperados sem acessar banco de dados
- Exemplo: Validar se Index retorna view correta

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
- Valida cálculos, transformações e regras
- Exemplo: Validar formatação de dados

### 3. Testes de Integração
**Arquivo**: `tests/ContactsManager.IntegrationTests/PersonsControllerIntegrationTest.cs`

- Testa fluxo completo com banco de dados real
- Utiliza `CustomWebApplicationFactory` para configuração
- Simula requisições HTTP reais
- Valida comportamento end-to-end

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
Edite a connection string em `appsettings.json`:
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
   - Passwords hasheadas com algoritmo robusto
   - Políticas de senha configuráveis
   - Suporte a autenticação multi-fator (extensível)

2. **Autorização Baseada em Claims**
   - Fallback policy requer autenticação
   - Decoradores `[Authorize]` em controladores sensíveis

3. **Proteção CSRF**
   - Token anti-forgery automático em formulários
   - Validação automática via `AutoValidateAntiforgeryTokenAttribute`

4. **HTTPS**
   - Redirecionamento automático para HTTPS
   - HSTS habilitado em produção

5. **Validação de Entrada**
   - Data Annotations em modelos
   - Validação do lado servidor

---

## 📈 Padrões de Desenvolvimento

### SOLID Principles - Aplicação Prática

#### **S** - Single Responsibility Principle (SRP)
Cada classe tem uma única responsabilidade bem definida:

```csharp
// ✅ Cada serviço é responsável por uma operação específica
public interface IPersonsGetterService 
{
    Task<List<PersonResponse>> GetAllPersons();
    Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
    Task<MemoryStream> GetPersonsCSV();
    Task<MemoryStream> GetPersonsExcel();
}

public interface IPersonsAdderService 
{
    Task<PersonResponse> AddPerson(PersonAddRequest personRequest);
}

public interface IPersonsDeleterService 
{
    Task<bool> DeletePerson(Guid? personID);
}
```

Em vez de ter um único `PersonsService` com todos os métodos, cada serviço tem uma responsabilidade específica (Get, Add, Delete, Update, Sort).

#### **O** - Open/Closed Principle (OCP)
Classes abertas para extensão, mas fechadas para modificação:

```csharp
// ✅ Novo tipo de filtro pode ser adicionado sem modificar código existente
public interface IPersonsRepository
{
    Task<List<Person>> GetFilteredPersons(string searchBy, string? searchString);
}

// Pode-se criar novos repositórios que implementam a interface
// sem modificar código existente
public class PersonsRepository : IPersonsRepository { }
```

#### **L** - Liskov Substitution Principle (LSP)
Subtipos podem substituir seus tipos base:

```csharp
// ✅ Ambas as implementações podem ser usadas intercambiavelmente
IPersonsGetterService service = new PersonsGetterService(_repository);
IPersonsGetterService serviceWithFewFields = new PersonsGetterServiceWithFewExcelFields(_repository);

// Ambas são injetadas nos controladores sem problemas
services.AddScoped<IPersonsGetterService, PersonsGetterService>();
services.AddScoped<PersonsGetterServiceWithFewExcelFields>();
```

#### **I** - Interface Segregation Principle (ISP)
Múltiplas interfaces específicas em vez de uma genérica:

```csharp
// ✅ Interfaces segregadas por responsabilidade
public interface IPersonsGetterService { }      // Apenas leitura
public interface IPersonsAdderService { }       // Apenas criação
public interface IPersonsUpdaterService { }     // Apenas atualização
public interface IPersonsDeleterService { }     // Apenas exclusão
public interface IPersonsSorterService { }      // Apenas ordenação

// Em vez de:
// public interface IPersonsService { } // Todas as operações em uma interface

// O controlador injeta apenas as dependências que precisa
public PersonsController(
    IPersonsGetterService getterService,
    IPersonsAdderService adderService,
    IPersonsDeleterService deleterService
)
```

#### **D** - Dependency Inversion Principle (DIP)
Dependências em abstrações, não em implementações concretas:

```csharp
// ✅ Depende de interface, não de classe concreta
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

// ✅ Injeção de dependência via IoC Container
services.AddScoped<IPersonsRepository, PersonsRepository>();
services.AddScoped<IPersonsGetterService, PersonsGetterService>();
```

---

### Clean Architecture - Estrutura de Camadas

O projeto implementa **Clean Architecture** com separação clara entre camadas:

#### **1. UI Layer (Apresentação)**
Responsável apenas pela apresentação:
```
ContactsManager.UI/
├── Controllers/          # Lógica de roteamento e requisições
├── Views/               # Templates Razor
├── Filters/             # Filtros de requisição
├── Middleware/          # Middleware customizado
└── Program.cs           # Configuração de startup
```

#### **2. Core Layer (Lógica de Negócio)**
Contém a lógica pura sem dependências externas:
```
ContactsManager.Core/
├── Domain/
│   ├── Entities/        # Modelos de domínio puros
│   └── IdentityEntities/# Entidades de autenticação
├── DTO/                 # Transferência de dados entre camadas
├── ServiceContracts/    # Interfaces dos serviços
├── Enums/              # Enumerações do domínio
└── Services/           # Implementação da lógica de negócio
```

**Característica importante**: Esta camada **não tem nenhuma dependência** da UI ou Infrastructure.

#### **3. Infrastructure Layer (Acesso a Dados)**
Responsável pela comunicação com recursos externos:
```
ContactsManager.Infrastructure/
├── DbContext/           # Entity Framework Core DbContext
├── Repositories/        # Padrão Repository para acesso a dados
└── Migrations/          # Versionamento de schema
```

#### **Fluxo de Requisição (Dependency Rule)**
```
UI → Services (Core) → Repository (Infrastructure) → Database
↑                                                         ↓
└─────────────────── Resposta em DTOs ←────────────────┘
```

**Observação**: A dependência sempre flui **para dentro**, nunca para fora. O Core nunca conhece UI ou Infrastructure.

---

### Exemplos de Implementação Prática

#### Exemplo 1: Separação de Responsabilidades
```csharp
// ✅ PersonsController apenas orquestra requisições
[HttpGet]
[Route("[action]")]
public async Task<IActionResult> Index(string searchBy, string? searchString)
{
    // Delega para o serviço apropriado
    List<PersonResponse> persons = await _personsGetterService
        .GetFilteredPersons(searchBy, searchString);

    return View(persons);
}

// ✅ PersonsGetterService encapsula lógica de negócio
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
            // ... mapping
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
// ✅ A UI recebe apenas o DTO, não a entidade de domínio
public record PersonResponse
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    // ... apenas dados necessários
}

// ✅ A entidade de domínio permanece pura
public class Person
{
    [Key]
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    [ForeignKey("CountryID")]
    public virtual Country? Country { get; set; }

    public override string ToString() { /* ... */ }
}
```

#### Exemplo 3: Dependency Injection Centralizado
```csharp
// ✅ Configuração centralizada em ConfigureServicesExtension
public static IServiceCollection ConfigureServices(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Repositories
    services.AddScoped<ICountriesRepository, CountriesRepository>();
    services.AddScoped<IPersonsRepository, PersonsRepository>();

    // Services - Cada um com responsabilidade específica
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

### Clean Code Practices

- ✅ **Nomes significativos**: `GetFilteredPersons` é claro sobre o que faz
- ✅ **Métodos pequenos e focados**: Cada método faz uma coisa bem
- ✅ **Tratamento de erros apropriado**: Try-catch em pontos estratégicos
- ✅ **Documentação com XML comments**: Em interfaces de serviço
- ✅ **Sem code smells**: Sem duplicação, sem complexidade desnecessária
- ✅ **KISS (Keep It Simple, Stupid)**: Código legível e direto

---

## 📚 Recursos para Aprendizado

### Documentação Oficial
- [Documentação do ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity/)
- [Serilog Documentation](https://serilog.net/)

### Padrões e Práticas
- Clean Architecture
- Repository Pattern
- Dependency Injection
- SOLID Principles
- Domain-Driven Design

---


## 📞 Contato e Links

- **GitHub**: [github.com/oJotaaa/ContactsManager](https://github.com/oJotaaa/ContactsManager)
- **Licença**: MIT

---

## 📝 Notas Adicionais

### Estrutura de Configuração
O arquivo `appsettings.json` inclui configurações para:
- **Logging**: Múltiplos sinks (Console, File, SQL Server, Seq)
- **Banco de Dados**: Connection strings
- **EPPlus**: Licença para manipulação de Excel
- **Serilog**: Nível de log e enriquecimento

### Middleware Personalizado
- `ExceptionHandleMiddleware`: Captura e loga exceções não tratadas
- `UseHttpLogging`: Log detalhado de requisições HTTP

### Filtros Especializados
O projeto implementa filtros em diferentes escopos:
- **Authorization Filters**: Validar permissões
- **Resource Filters**: Processar requisições antes do modelo
- **Action Filters**: Validar/transformar dados
- **Exception Filters**: Tratar exceções
- **Result Filters**: Processar resultados

---

**Desenvolvido como portfólio de desenvolvimento em .NET**

*Última atualização: 2026*
