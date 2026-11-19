namespace Apta.Infrastructure.Services;

public sealed class CsvCustomerParser : ICustomerParser
{
    private readonly ICollection<Customer> _customers = [];
    private readonly CsvConfiguration _configuration = new(cultureInfo: CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        Delimiter = ";"
    };

    public async Task<Result<IReadOnlyCollection<Customer>>> ParseCustomersAsync(Stream stream, CancellationToken cancellation = default)
    {
        using var reader = new StreamReader(stream);
        using var parser = new CsvReader(reader, _configuration);

        await parser.ReadAsync();

        /* consume csv header record before processing customer rows. */
        parser.ReadHeader();

        while (await parser.ReadAsync())
        {
            _customers.Add(new Customer
            {
                Name = parser.GetField<string>(0) ?? string.Empty.OrFallback(),
                Document = parser.GetField<string>(1) ?? string.Empty.OrFallback(),
                Phone = parser.GetField<string>(2) ?? string.Empty.OrFallback(),
            });
        }

        return Result<IReadOnlyCollection<Customer>>.Success([.. _customers]);
    }
}
