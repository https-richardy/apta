---
name: conventions
description: >-
  Mandatory style, formatting and architecture conventions for all of our
  team's .NET (C#) projects. Use it whenever you are writing, reviewing or
  generating C# code in these projects — creating/changing controllers,
  handlers, repositories, schemes (DTOs), filters, validators, mappers, service
  registration (IoC) or unit and integration tests. It covers both what to
  FOLLOW (layered architecture, pattern matching in controllers, Descriptions
  on schemes, global usings, Scoped lifetime) and the ANTI-PATTERNS we reject
  (private methods, comments in production code, usings at the top of the file,
  repositories with their own add/delete).
---

# Code conventions

This is the standard **all of our projects** must follow — style, formatting
and architecture. The rules apply to any new boundary/service. When in doubt,
mirror an equivalent file that already exists in the project before inventing
something.

Layered architecture (DDD / Clean) of each boundary:
`Domain` → `Application` → `Infrastructure` (+ `Infrastructure.IoC`) → `WebApi`.

---

## 1. General formatting and style

- **4-space** indentation, `end_of_line = crlf`, trailing newline at the end of
  the file, no trailing whitespace. (The root `.editorconfig` applies — it
  promotes several rules to `error`, including the `IDE0055` formatting rule.)
- **File-scoped** `namespace` (`namespace X;`), never with braces.
- Concrete classes are `public sealed` by default. Use inheritance only where a
  framework base requires it.
- **Primary constructors** for dependency injection:
  `public sealed class AssetRepository(IMongoDatabase database) : ...`.
- One type per file; file name = type name.
- `var` for locals when the type is obvious on the right-hand side.
- No multiple consecutive blank lines (`dotnet_style_allow_multiple_blank_lines_experimental = false`).

### Group members by type — primitives with primitives, complex with complex

Group members by the nature of their type and **separate the groups with a
blank line**. Never drop a lone member in the middle of another group.

```csharp
public sealed class Asset : Aggregate
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string Checksum { get; set; }

    public string Tenant { get; set; }
    public string Bucket { get; set; }

    public long Size { get; set; } = 0;
    public long Offset { get; set; } = 0;
}
```

### `System.Numerics.BigInteger` instead of a bare `long`

For counts and "large" numbers in the domain/repository signature, use
`System.Numerics.BigInteger` (more expressive than a raw `long`), written with
the namespace qualified inline as in the existing standard:

```csharp
public Task<System.Numerics.BigInteger> CountAssetsAsync(
    AssetFilters filters,
    CancellationToken cancellation = default
);
```

---

## 2. Usings → always global, in `Usings.cs`, in sections

**Anti-pattern:** a `using` at the top of a `.cs` file. **Never** do this.

All usings go in a single `Usings.cs` file per project, as `global using`,
**separated into sections** by a blank line (System first, then the boundary's
own namespaces layer by layer, then third-party libraries):

```csharp
global using System.Text.Json.Serialization;
global using System.ComponentModel;

global using Corban.Assets.Domain.Aggregates;
global using Corban.Assets.Domain.Repositories;

global using Corban.Assets.Application.Transport;
global using Corban.Assets.Application.Transport.Assets;

global using FluentValidation;
global using AutoMapper;
```

---

## 3. Controllers — only `IDispatcher` + pattern matching

A controller **has no logic**. It only takes the `IDispatcher` (primary
constructor), dispatches the request and does a `switch` with pattern matching
on the `Result`. No `if`, no injected services other than the dispatcher, no
helper methods.

```csharp
[ApiController]
[Route("api/v1/assets")]
[ApiConventionType(typeof(AssetsMetadata))]
public sealed class AssetsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Permissions.ReadAssets)]
    [Stability(Stability.Stable)]
    [EndpointDescription("searches for assets based on the provided filters")]
    public async Task<IActionResult> FilterAssetsAsync([FromQuery] AssetFiltersParameters request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } => StatusCode(StatusCodes.Status200OK, result.Data),
        };
    }
}
```

- Actions end in `Async`, return `Task<IActionResult>` and take
  `CancellationToken cancellation` as the last parameter.
- Attributes, one per line: `[HttpX]`, `[Authorize]`, `[Stability(...)]`,
  `[RequirePermission(...)]`, `[EndpointDescription("...")]` (description in
  lowercase).
- On updates by id: `request with { Id = id }`.
- The `switch` arms map `Result` → `StatusCode(...)`
  (`Status200OK`/`Status201Created`/`Status204NoContent`/`Status404NotFound`…),
  matching the specific error (`when result.Error == AssetErrors.AssetDoesNotExist`).

### Controller metadata (mandatory)

Every controller has a corresponding static `*Metadata` class in
`WebApi/Metadata`, referenced via `[ApiConventionType(typeof(...))]`. It
declares the `ProducesResponseType` of each action (matched by name prefix),
for documentation/Scalar. Empty methods, same signature as the action:

```csharp
#pragma warning disable IDE0060

namespace Corban.Assets.WebApi.Metadata;

public static class AssetsMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(Pagination<AssetScheme>), StatusCodes.Status200OK)]
    public static void FilterAssetsAsync(AssetFiltersParameters request, CancellationToken cancellation) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(AssetScheme), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public static void UpdateAssetAsync(string id, PatchAssetScheme request, CancellationToken cancellation) { }
}
```

When you create a controller, create alongside it: the **Metadata**, the
**Handler(s)**, the transport **Scheme(s)** and, if there is filtering, the
**Filters**/**Builder** in the domain.

---

## 4. Handlers (Application) — where the logic lives

- `public sealed class XHandler(IRepository repository, IMapper mapper) : IDispatchHandler<TRequest, Result<TResponse>>`.
- A single method `HandleAsync(TRequest parameters, CancellationToken cancellation = default)`.
- Always returns `Result<T>` / `Result` (`Result<T>.Success(...)` /
  `Result<T>.Failure(Error)`).
- Builds filters through the domain builder, queries the repository, decides the
  flow and maps the response with the `IMapper`.

```csharp
public sealed class BucketCreationHandler(IBucketRepository repository, IMapper mapper) :
    IDispatchHandler<BucketCreationScheme, Result<BucketScheme>>
{
    public async Task<Result<BucketScheme>> HandleAsync(BucketCreationScheme parameters, CancellationToken cancellation = default)
    {
        var filters = BucketFilters.AsBuilder()
            .WithLabel(parameters.Label)
            .Build();

        var buckets = await repository.FilterBucketsAsync(filters, cancellation);
        var existingBucket = buckets.FirstOrDefault();

        if (existingBucket is not null)
        {
            return Result<BucketScheme>.Failure(BucketErrors.BucketAlreadyExists);
        }

        var bucket = mapper.Map<Bucket>(parameters);
        var bucketCreated = await repository.InsertAsync(bucket, cancellation: cancellation);

        var result = mapper.Map<BucketScheme>(bucketCreated);

        return Result<BucketScheme>.Success(result);
    }
}
```

### Fetching a single entity → `FilterXAsync(...)` + `FirstOrDefault()`

When the handler needs a **single entity** from the repository, **always** build
the filters, call `FilterXAsync` (which returns the collection) and apply
`FirstOrDefault()` on the result — then check with `is null` / `is not null`.
**Do not** create `GetByIdAsync`/`FindOneAsync` methods on the repository just
for this: the repository only filters and counts (§6), and "fetch one" is
composition in the handler.

```csharp
var filters = BucketFilters.AsBuilder()
    .WithLabel(parameters.Label)
    .Build();

var buckets = await repository.FilterBucketsAsync(filters, cancellation);
var existingBucket = buckets.FirstOrDefault();

if (existingBucket is not null)
{
    return Result<BucketScheme>.Failure(BucketErrors.BucketAlreadyExists);
}
```

The same applies when absence is what characterizes the failure (e.g.,
patch/delete of something that must exist):

```csharp
var assets = await repository.FilterAssetsAsync(filters, cancellation);
var asset = assets.FirstOrDefault();

if (asset is null)
{
    return Result<AssetScheme>.Failure(AssetErrors.AssetDoesNotExist);
}
```

### Fail fast (guard clauses / early return) — happy path last

We prefer to **fail fast**: each error condition is a _guard clause_ that
**returns immediately** (early return), with no `else`. The method body "peels
off" the failures one by one, and the **happy path stays at the end**, without
nesting.

- One `if` per failure condition, each already with its `return Result...Failure(...)`.
- **No `else`**, no nested `if`, no accumulating state to decide at the end.
- The happy path is not inside an `if`: it is what remains after all the guards,
  at the method's base indentation level.

✅ Do (fail fast, happy path last):

```csharp
public async Task<Result<AssetScheme>> HandleAsync(PatchAssetScheme parameters, CancellationToken cancellation = default)
{
    var filters = AssetFilters.AsBuilder()
        .WithIdentifier(parameters.Id)
        .Build();

    var assets = await repository.FilterAssetsAsync(filters, cancellation);
    var asset = assets.FirstOrDefault();

    if (asset is null)
    {
        return Result<AssetScheme>.Failure(AssetErrors.AssetDoesNotExist);
    }

    mapper.Map(parameters, asset);

    var assetUpdated = await repository.UpdateAsync(asset, cancellation);

    return Result<AssetScheme>.Success(mapper.Map<AssetScheme>(assetUpdated));
}
```

❌ Avoid (happy path nested inside `if`/`else`):

```csharp
if (asset is not null)
{
    mapper.Map(parameters, asset);
    var assetUpdated = await repository.UpdateAsync(asset, cancellation);

    return Result<AssetScheme>.Success(mapper.Map<AssetScheme>(assetUpdated));
}
else
{
    return Result<AssetScheme>.Failure(AssetErrors.AssetDoesNotExist);
}
```

---

## 5. Schemes (DTOs) — `record` with `[Description]`

Transport DTOs are `public sealed record`, with `init` properties (or `set`
when the flow requires mutation, e.g., `Id` filled in the controller).

**Every exposed property carries `[Description("...")]`** (from
`System.ComponentModel`) — it is what feeds the documentation in Scalar.
Description in lowercase, usually with an example (`for example, '...'`). Group
primitives/complex types with a blank line, same as entities.

```csharp
public sealed record AssetScheme
{
    [Description("the unique identifier of the asset, for example, 'asset_d3TjZUiCvu32Uq2LiapbNWjKa'")]
    public string Id { get; init; }

    [Description("the display name of the asset, for example, 'contract-2026'")]
    public string Name { get; init; }

    [Description("the size of the asset content in bytes")]
    public long Size { get; init; }
}
```

Filter requests implement `IDispatchable<Result<Pagination<TScheme>>>` and carry
`Page`/`PageSize` with defaults (`1` / `10`).

---

## 6. Repositories — only filter and count

**Anti-pattern:** declaring `Add`, `Insert`, `Update`, `Delete`, etc. on the
boundary's repository. That already comes for free by inheriting the base
abstractions (`IAggregateCollection<T>` / `AggregateCollection<T>`). The
project's repository **only** declares the aggregate-specific **filter** and
**count** methods.

Interface (Domain):

```csharp
public interface IOperatorRepository : IAggregateCollection<Operator>
{
    public Task<IReadOnlyCollection<Operator>> FilterOperatorsAsync(
        OperatorFilters filters,
        CancellationToken cancellation = default
    );

    public Task<System.Numerics.BigInteger> CountOperatorsAsync(
        OperatorFilters filters,
        CancellationToken cancellation = default
    );
}
```

Implementation (Infrastructure) — primary constructor, inherits
`AggregateCollection<T>`, uses the aggregation pipeline. Full example (filter
**and** count), which is the model to follow end to end:

```csharp
public sealed class OperatorRepository(IMongoDatabase database) :
    AggregateCollection<Operator>(database, Collections.Operators),
    IOperatorRepository
{
    public async Task<IReadOnlyCollection<Operator>> FilterOperatorsAsync(OperatorFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = PipelineDefinitionBuilder
            .For<Operator>()
            .As<Operator, Operator, BsonDocument>()
            .FilterOperators(filters)
            .Paginate(filters.Pagination)
            .Sort(filters.Sort);

        var options = new AggregateOptions { AllowDiskUse = true };
        var aggregation = await _collection.AggregateAsync(pipeline, options, cancellation);

        var bsonDocuments = await aggregation.ToListAsync(cancellation);
        var operators = bsonDocuments
            .Select(bson => BsonSerializer.Deserialize<Operator>(bson))
            .ToList();

        return operators;
    }

    public async Task<System.Numerics.BigInteger> CountOperatorsAsync(OperatorFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = PipelineDefinitionBuilder
            .For<Operator>()
            .As<Operator, Operator, BsonDocument>()
            .FilterOperators(filters)
            .Count();

        var aggregation = await _collection.AggregateAsync(pipeline, cancellationToken: cancellation);
        var result = await aggregation.FirstOrDefaultAsync(cancellation);

        return result?.Count ?? 0;
    }
}
```

### Filters + Builder (Domain)

`XFilters : Filters` carries the filterable fields and exposes
`static XFiltersBuilder AsBuilder() => new();`. The `XFiltersBuilder :
FiltersBuilderBase<...>` has a fluent `WithY(...)` method per field, which only
applies when the value is valid:

```csharp
public AssetFiltersBuilder WithName(string? name)
{
    if (!string.IsNullOrWhiteSpace(name))
        _filters.Name = name.Trim();

    return this;
}
```

### Pipeline definition — a list of `FilterDefinitions` + one `Match`, never `if`

The translation of the filters into the query (Mongo) lives in a static
`*PipelineDefinition` in `Infrastructure/Persistence/Pipelines`, as an
**extension method** over the `PipelineDefinition<T, BsonDocument>`.

The pattern is **always the same**: build a **list of
`FilterDefinition<BsonDocument>`** using the `FilterDefinitions.MatchIf...`
helpers (our class, which already returns the right filter and **ignores itself
when the value is null/empty**), and at the end combine everything with a single
`.Match(...And(definitions))`.

**No `if`, no private methods, no building a `BsonDocument` by hand** to decide
whether a filter goes in or not — that is exactly what the `MatchIf...` helpers
solve. Just pick the right method for each field.

```csharp
public static class AssetPipelineDefinition
{
    public static PipelineDefinition<Asset, BsonDocument> FilterAssets(this PipelineDefinition<Asset, BsonDocument> pipeline, AssetFilters filters)
    {
        var definitions = new List<FilterDefinition<BsonDocument>>
        {
            FilterDefinitions.MatchIfNotEmpty(Documents.Assets.Identifier, filters.Id),
            FilterDefinitions.MatchIfNotEmpty(Documents.Assets.Name, filters.Name),
            FilterDefinitions.MatchIfNotEmpty(Documents.Assets.Extension, filters.Extension),
            FilterDefinitions.MatchIfNotEmpty(Documents.Assets.Checksum, filters.Checksum),
            FilterDefinitions.MatchIfContains(Documents.Assets.Bucket, filters.Bucket)
        };

        return pipeline.Match(Builders<BsonDocument>.Filter.And(definitions));
    }
}
```

`FilterDefinitions` helpers in use in the code (choose by the field's behavior;
each one only applies the filter when the value is provided):

- Always reference the field name via `Documents.*` (constants), never a loose
  "magic" string.
- **Most scenarios are already covered by these helpers** — always prefer
  finding the right `MatchIf...`. We know it's not always possible: when a case
  really does not fit any of them, then compose the specific `FilterDefinition`
  and **add it to the same list** (it is still list + `Match`, with no `if` and
  no private method).
- Pagination/sorting do not go here — they stay in the repository, via
  `.Paginate(filters.Pagination).Sort(filters.Sort)` (§6).

These pipeline definitions have their own **unit test** (see the
`*PipelineDefinitionTests` in each boundary).

---

## 7. Errors — static catalog

Errors live in a `static` class per aggregate, as `static readonly Error`, with
`Code` and `Description`:

```csharp
public static class AssetErrors
{
    public static readonly Error AssetDoesNotExist = new(
        Code: "#ASSETS-ERROR-C2C52",
        Description: "The specified asset does not exist."
    );
}
```

### `Code` format and generation

The `Code` **always** follows the format `#NAME-ERROR-XXXXX`, entirely in
**UPPERCASE**:

- Starts with `#`.
- `NAME` = the aggregate/context of the error (e.g., `ASSETS`, `CUSTOMERS`,
  `BUCKETS`), in UPPERCASE.
- The literal `-ERROR-`.
- `XXXXX` = the **first 5 characters**, in UPPERCASE, of the **MD5** hash of any
  string (it just needs to be a distinct value per error — usually the error's
  own name/description does the job). This suffix is just a stable, unique
  identifier; **you (whoever is creating the error) generate this code**, it is
  not made up "by hand".

That is, each new error requires generating an MD5, taking the first 5 hex
digits and placing them in UPPERCASE in place of `XXXXX`.

Examples of how to generate the suffix:

```bash
# bash — generates the hash and already trims/uppercases the first 5 characters
printf '%s' "AssetDoesNotExist" | md5sum | cut -c1-5 | tr '[:lower:]' '[:upper:]'
# -> C2E52  (composes:  #ASSETS-ERROR-C2E52)
```

```powershell
# powershell
$md5 = [System.Security.Cryptography.MD5]::Create()
$bytes = $md5.ComputeHash([Text.Encoding]::UTF8.GetBytes("AssetDoesNotExist"))
(([BitConverter]::ToString($bytes) -replace '-','').Substring(0,5)).ToUpper()
```

> Important: the code is **fixed** once created — it becomes a public identifier
> of the error. Generate it once when adding the error and **do not**
> recompute/change it later, or you break whoever already references that code.

---

## 8. Validators and Mappers

- **Validators** (FluentValidation): `public sealed class XValidator :
  AbstractValidator<X>`, fluent rules in the constructor with
  `.WithMessage("...")` in lowercase.
- **Mappers** (AutoMapper): `public sealed class XMapper : Profile`, `CreateMap`
  with an **explicit `ForMember(...)` for each property** (`MapFrom` or
  `Ignore` — nothing implicit).

---

## 9. IoC — Scoped lifetime by default

Service registration in static `*Extension` classes, marked with
`[ExcludeFromCodeCoverage(Justification = "...")]`. **When in doubt, use
`Scoped`**:

```csharp
[ExcludeFromCodeCoverage(Justification = "this class is responsible for configuring the dependency injection container and does not contain any logic that can be tested.")]
public static class ServicesExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerParser, StandardCsvCustomerParser>();
        services.AddScoped<ICustomerExporter, CsvCustomerExporter>();
    }
}
```

---

## 10. Tests — unit **and** integration

Every behavior has a **unit** test and an **integration** test. The test project
also uses a global `Usings.cs` (AutoFixture, Moq, xUnit, Testcontainers,
WireMock…).

Mandatory patterns:

- `public sealed class XTests`. Fixture fields at the top
  (`private readonly Fixture _fixture = new();`, mocks `Mock<T>`).
- **Method name**: always `Given_When_Then` (here in the format
  `When_Method_IsCalled_WithCondition_Then_Result`).
- **Descriptive `DisplayName`** on every `[Fact]`, in lowercase.
  Integration/e2e tests prefix with `"[e2e] - ..."`.
- **Section comments** with `/* ... */` in lowercase marking
  `arrange` / `act` / `assert` (the only place we comment — see §11).
- **Asserts in blocks of 2**, grouped by intent and separated by a blank line.
- Use `TestContext.Current.CancellationToken` in the asynchronous calls.

```csharp
public sealed class CustomerCreationHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when customer does not exist, customer is inserted and success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithNewCustomer_Then_CustomerIsInsertedAndSuccessResultIsReturned()
    {
        /* arrange: create request, mapped entities and configure repository and mapper */
        var request = _fixture.Create<CustomerCreationScheme>();
        var customer = _fixture.Create<Customer>();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _mapper.Setup(mapper => mapper.Map<Customer>(request))
            .Returns(customer);

        var handler = new CustomerCreationHandler(_repository.Object, _mapper.Object);

        /* act: execute handler */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(customer, result.Data);

        /* assert: verify expected interactions */
        _repository.Verify(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(mapper => mapper.Map<Customer>(request), Times.Once);
    }
}
```

### Integration tests (e2e) with `ApplicationFixture` + WireMock

Integration spins up the real environment through a fixture (`ApplicationFixture`,
Testcontainers for Mongo, WireMock for external HTTP dependencies) and exercises
the API end to end via `HttpClient`.

Integration-specific rules:

- Class `[Collection(nameof(DisableParallelizationCollection))]`, receiving the
  `ApplicationFixture factory` via primary constructor and implementing
  `IClassFixture<ApplicationFixture>` + `IAsyncLifetime`.
- A `JsonSerializerOptions _serializer` at the top (`PropertyNameCaseInsensitive`
  + `JsonNamingPolicy.CamelCase`) reused across all serialization/deserialization.
- **Seed** the required state via the real repository
  (`factory.Services.GetRequiredService<IXRepository>()`), and external HTTP
  dependencies (Keycloak, other services) **stubbed in WireMock** with
  `factory.WireMock.Given(...).RespondWith(...)`.
- At the end, **verify that the external call actually happened** with
  `factory.WireMock.FindLogEntries([...])` + `Assert.NotEmpty(...)`.
- Close with `InitializeAsync`/`DisposeAsync` delegating to the fixture.

#### Always read the `content` as a string right after the response

After each HTTP call, **always** read the body with `ReadAsStringAsync(...)`
**before** any assert — even when you are going to deserialize afterward. This
keeps the `content` (raw string) available in the **debugger** when the test
fails, making it easier to see what the API actually returned. Only then
deserialize from it.

```csharp
/* act: send POST request to the operators endpoint */
var response = await httpClient.PostAsJsonAsync("/api/v1/operators", request, TestContext.Current.CancellationToken);
var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

/* assert: verify http response status and content */
Assert.Equal(HttpStatusCode.Created, response.StatusCode);
Assert.False(string.IsNullOrWhiteSpace(content));

/* assert: deserialize response */
var result = JsonSerializer.Deserialize<OperatorScheme>(content, _serializer);
```

#### No "magic" strings for header/content-type — use `System.Net`

Content-type, header names and the like come from `System.Net.*` constants
(`MediaTypeNames`) and `Microsoft.Net.Http.Headers` (`HeaderNames`), never
hardcoded by hand:

```csharp
.WithHeader(HeaderNames.ContentType, MediaTypeNames.Application.Json)   // ✅
.WithHeader("Content-Type", "application/json")                        // ❌
```

Full, readable example (seed + WireMock stubs + reading the content +
verifying the external call):

```csharp
[Collection(nameof(DisableParallelizationCollection))]
public sealed class OperatorsControllerTests(ApplicationFixture factory) :
    IClassFixture<ApplicationFixture>,
    IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact(DisplayName = "[e2e] - when POST /api/v1/operators is called with a valid request, 201 Created is returned")]
    public async Task When_CreateOperator_IsCalled_WithValidRequest_Then_201CreatedIsReturned()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;

        /* arrange: seed the organization the operator will belong to */
        var organizationRepository = factory.Services.GetRequiredService<IOrganizationRepository>();
        var organization = _fixture.Build<Domain.Aggregates.Organization>()
            .With(entity => entity.IsDeleted, false)
            .Create();

        organization = await organizationRepository.InsertAsync(organization, cancellation: TestContext.Current.CancellationToken);

        var request = _fixture.Build<OperatorCreationScheme>()
            .With(@operator => @operator.FirstName, "John")
            .With(@operator => @operator.LastName, "Doe")
            .With(@operator => @operator.Email, "john.doe@corban.com.br")
            .With(@operator => @operator.Password, "Tr0ub4dor&3")
            .With(@operator => @operator.OrganizationId, organization.Id)
            .Create();

        /* arrange: configures wiremock stubs simulating keycloak user creation */
        var userId = Guid.NewGuid().ToString("N");
        var user = _fixture.Build<UserScheme>()
            .With(user => user.Id, userId)
            .With(user => user.Username, request.Email)
            .With(user => user.Email, request.Email)
            .Create();

        factory.WireMock.Given(Request.Create().UsingPost()
            .WithPath("/admin/realms/corban/users"))
            .RespondWith(Response.Create()
            .WithStatusCode(HttpStatusCode.Created)
            .WithHeader("Location", $"{factory.WireMock.Url}/admin/realms/corban/users/{userId}"));

        factory.WireMock.Given(Request.Create().UsingGet()
            .WithPath($"/admin/realms/corban/users/{userId}"))
            .RespondWith(Response.Create()
            .WithStatusCode(HttpStatusCode.OK)
            .WithHeader(HeaderNames.ContentType, MediaTypeNames.Application.Json)
            .WithBody(JsonSerializer.Serialize(user, _serializer)));

        /* act: send POST request to the operators endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/operators", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<OperatorScheme>(content, _serializer);

        /* assert: validate returned operator */
        Assert.NotNull(result);

        Assert.Equal($"{request.FirstName} {request.LastName}", result.Name);
        Assert.Equal(request.Email, result.Email);

        /* assert: verify keycloak user creation was actually called */
        var requests = factory.WireMock.FindLogEntries(
        [
            Request.Create().UsingPost().WithPath("/admin/realms/corban/users")
        ]);

        Assert.NotEmpty(requests);
    }

    public ValueTask InitializeAsync() => factory.InitializeAsync();
    public ValueTask DisposeAsync() => factory.DisposeAsync();
}
```

---

## 11. Anti-patterns — what NOT to do

- ❌ **Private methods.** We hate private methods — prefer extracting into a
  dedicated type (handler, builder, service) over creating a private helper.
- ❌ **Commenting production code.** We prefer readability over comments. Save
  the rare, truly necessary exceptions — but, preferably, do not comment. (The
  only routine exception is the `/* arrange/act/assert */` in tests.)
- ❌ **A `using` at the top of the file.** Everything becomes a `global using`
  in `Usings.cs`, separated into sections (§2).
- ❌ **Own write methods on the repository** (`Add`/`Insert`/`Update`/
  `Delete`). Already inherited from `IAggregateCollection`; the boundary's repo
  only filters and counts (§6).
- ❌ **Logic in the controller.** Only `IDispatcher` + a `switch` with pattern
  matching (§3).
- ❌ **A bare `long`** where `System.Numerics.BigInteger` fits (§1).
- ❌ **A scheme/entity without grouping types** or with a lone member outside
  the group (§1).
- ❌ **A scheme property without `[Description]`** (§5).
- ❌ **`else`/nested `if` instead of failing fast.** Use guard clauses with
  early return; happy path last (§4).
- ❌ **Hardcoded content-type/header** (`"application/json"`, `"Content-Type"`).
  Use `MediaTypeNames` (`System.Net`) and `HeaderNames` (§10).
- ❌ **Asserting directly on the response without reading the `content` as a
  string first.** Always read with `ReadAsStringAsync` to have the body visible
  in the debugger (§10).
- ❌ **Multiple blank lines** and formatting outside the `.editorconfig`
  (promoted to `error`).
