namespace Apta.Tests.Unit.Services;

public sealed class CsvCustomerParserTests
{
    [Fact(DisplayName = "when parsing a csv with customer rows, every row is mapped by position")]
    public async Task When_ParseCustomersAsync_IsCalled_WithPopulatedCsv_Then_EveryRowIsMappedByPosition()
    {
        /* arrange: build a semicolon-delimited csv with a header and two customer rows */
        var content = "document;name;phone\n52998224725;João da Silva;11988887777\n11144477735;Maria Oliveira;21977776666";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var parser = new CsvCustomerParser();

        /* act: parse the stream */
        var result = await parser.ParseCustomersAsync(stream, TestContext.Current.CancellationToken);
        var customers = result.Data!.ToList();

        /* assert: verify success and the row count */
        Assert.True(result.IsSuccess);
        Assert.Equal(2, customers.Count);

        /* assert: verify the first row mapped document and name by position */
        Assert.Equal("52998224725", customers[0].Document);
        Assert.Equal("João da Silva", customers[0].Name);

        /* assert: verify the second row mapped document and phone by position */
        Assert.Equal("11144477735", customers[1].Document);
        Assert.Equal("21977776666", customers[1].Phone);
    }

    [Fact(DisplayName = "when parsing a csv that only has a header, an empty collection is returned")]
    public async Task When_ParseCustomersAsync_IsCalled_WithHeaderOnly_Then_EmptyCollectionIsReturned()
    {
        /* arrange: build a csv containing only the header record */
        var content = "document;name;phone";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        /* act: parse the stream */
        var parser = new CsvCustomerParser();
        var result = await parser.ParseCustomersAsync(stream, TestContext.Current.CancellationToken);

        /* assert: verify success and that no customers were produced */
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }
}
