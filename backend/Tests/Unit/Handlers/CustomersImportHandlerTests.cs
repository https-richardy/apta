namespace Apta.Tests.Unit.Handlers;

public sealed class CustomersImportHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IEnqueuer<CustomersImportScheme>> _enqueuer = new();

    [Fact(DisplayName = "when the uploaded file is empty, the stream is discarded and a no file content failure is returned")]
    public async Task When_HandleAsync_IsCalled_WithEmptyFile_Then_FailureIsReturnedAndNothingIsEnqueued()
    {
        /* arrange: build a seekable but empty stream carried by the import request */
        var stream = new Mock<Stream>();

        stream.Setup(instance => instance.CanSeek).Returns(true);
        stream.Setup(instance => instance.Length).Returns(0);

        var handler = new CustomersImportHandler(_enqueuer.Object);
        var request = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = stream.Object }
        };

        /* act: handle the import request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the no file content failure */
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.NoFileContent, result.Error);

        /* assert: verify the stream was discarded and nothing was enqueued */
        stream.Verify(instance => instance.DisposeAsync(), Times.Once);
        _enqueuer.Verify(enqueuer => enqueuer.EnqueueAsync(It.IsAny<CustomersImportScheme>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the uploaded file has content, the request is enqueued and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithPopulatedFile_Then_RequestIsEnqueuedAndSuccessIsReturned()
    {
        /* arrange: build a seekable stream reporting content carried by the import request */
        var stream = new Mock<Stream>();

        stream.Setup(instance => instance.CanSeek).Returns(true);
        stream.Setup(instance => instance.Length).Returns(32);

        var handler = new CustomersImportHandler(_enqueuer.Object);
        var request = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = stream.Object }
        };

        /* act: handle the import request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the success result */
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);

        /* assert: verify the request was enqueued and the stream was kept */
        _enqueuer.Verify(enqueuer => enqueuer.EnqueueAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        stream.Verify(instance => instance.DisposeAsync(), Times.Never);
    }

    [Fact(DisplayName = "when the uploaded stream is not seekable, the empty check is skipped and the request is enqueued")]
    public async Task When_HandleAsync_IsCalled_WithNonSeekableStream_Then_EmptyCheckIsSkippedAndRequestIsEnqueued()
    {
        /* arrange: build a non-seekable stream carried by the import request */
        var stream = new Mock<Stream>();

        stream.Setup(instance => instance.CanSeek).Returns(false);

        var handler = new CustomersImportHandler(_enqueuer.Object);
        var request = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = stream.Object }
        };

        /* act: handle the import request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the success result */
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);

        /* assert: verify the length was never inspected and the request was enqueued */
        stream.VerifyGet(instance => instance.Length, Times.Never);
        _enqueuer.Verify(enqueuer => enqueuer.EnqueueAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
