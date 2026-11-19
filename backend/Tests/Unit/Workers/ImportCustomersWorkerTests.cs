namespace Apta.Tests.Unit.Workers;

public sealed class ImportCustomersWorkerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICustomerParser> _parser = new();

    [Fact(DisplayName = "when parsing fails, nothing is persisted")]
    public async Task When_HandleAsync_IsCalled_WithParsingFailure_Then_NothingIsPersisted()
    {
        /* arrange: configure the parser to fail */
        _parser.Setup(parser => parser.ParseCustomersAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyCollection<Customer>>.Failure(_fixture.Create<Error>()));

        var context = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = Stream.Null }
        };

        var worker = new ImportCustomersWorker(_repository.Object, _unitOfWork.Object, _parser.Object, NullLogger<ImportCustomersWorker>.Instance);

        /* act: process the import */
        await worker.HandleAsync(context, TestContext.Current.CancellationToken);

        /* assert: verify no transaction and no persistence happened */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Never);
        _repository.Verify(repository => repository.InsertManyAsync(It.IsAny<IEnumerable<Customer>>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when parsing yields no customers, nothing is persisted")]
    public async Task When_HandleAsync_IsCalled_WithEmptyResult_Then_NothingIsPersisted()
    {
        /* arrange: configure the parser to return an empty collection */
        _parser.Setup(parser => parser.ParseCustomersAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyCollection<Customer>>.Success([]));

        var context = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = Stream.Null }
        };

        var worker = new ImportCustomersWorker(_repository.Object, _unitOfWork.Object, _parser.Object, NullLogger<ImportCustomersWorker>.Instance);

        /* act: process the import */
        await worker.HandleAsync(context, TestContext.Current.CancellationToken);

        /* assert: verify no transaction and no persistence happened */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Never);
        _repository.Verify(repository => repository.InsertManyAsync(It.IsAny<IEnumerable<Customer>>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when parsing yields duplicated documents, only unique and documentless customers are persisted in a transaction")]
    public async Task When_HandleAsync_IsCalled_WithDuplicatedDocuments_Then_OnlyUniqueAndDocumentlessArePersisted()
    {
        /* arrange: build customers with a duplicated document and two without a document */
        var first = _fixture.Build<Customer>()
            .With(customer => customer.Document, "52998224725")
            .Create();

        var duplicate = _fixture.Build<Customer>()
            .With(customer => customer.Document, "52998224725")
            .Create();

        var other = _fixture.Build<Customer>()
            .With(customer => customer.Document, "11144477735")
            .Create();

        var documentlessA = _fixture.Build<Customer>()
            .With(customer => customer.Document, string.Empty)
            .Create();

        var documentlessB = _fixture.Build<Customer>()
            .With(customer => customer.Document, string.Empty)
            .Create();

        var persisted = new List<Customer>();

        _parser.Setup(parser => parser.ParseCustomersAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyCollection<Customer>>.Success([first, duplicate, other, documentlessA, documentlessB]));

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task> action) => action(token));

        _repository.Setup(repository => repository.InsertManyAsync(It.IsAny<IEnumerable<Customer>>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Customer> batch, InsertionBehavior behavior, CancellationToken token) => persisted = batch.ToList())
            .Returns(Task.CompletedTask);

        var context = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = Stream.Null }
        };

        var worker = new ImportCustomersWorker(_repository.Object, _unitOfWork.Object, _parser.Object, NullLogger<ImportCustomersWorker>.Instance);

        /* act: process the import */
        await worker.HandleAsync(context, TestContext.Current.CancellationToken);

        /* assert: verify the duplicate was dropped while the documentless ones were kept */
        Assert.Equal(4, persisted.Count);
        Assert.DoesNotContain(duplicate, persisted);

        /* assert: verify the unique and other documentless customers were persisted */
        Assert.Contains(first, persisted);
        Assert.Contains(other, persisted);

        /* assert: verify persistence happened once inside a transaction */
        _repository.Verify(repository => repository.InsertManyAsync(It.IsAny<IEnumerable<Customer>>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Once);
    }
}
