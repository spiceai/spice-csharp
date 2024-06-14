using Spice;

namespace SpiceTest;

public class FlightQueryTest
{
    private SpiceClient _spiceClient;
    private string? ApiKey { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        ApiKey = Environment.GetEnvironmentVariable("API_KEY");
    }

    [SetUp]
    public void Setup()
    {
        if (ApiKey == null)
        {
            throw new Exception("No API_KEY provided");
        }
        _spiceClient = new SpiceClientBuilder()
            .WithSpiceCloud(ApiKey)
            .Build();
    }

    [Test]
    public async Task TestQuery()
    {
        var result =
            await _spiceClient.Query(
                """SELECT number, "timestamp", base_fee_per_gas, base_fee_per_gas / 1e9 AS base_fee_per_gas_gwei FROM eth.recent_blocks limit 10""");

        var enumerator = result.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
        {
            var batch = enumerator.Current;
            Assert.Multiple(() =>
            {
                Assert.That(batch.ColumnCount, Is.EqualTo(4));
                Assert.That(batch.Length, Is.EqualTo(10));
            });
        }
    }

    [Test]
    public async Task TestQueryLarge()
    {
        var result =
            await _spiceClient.Query(
                """SELECT number, "timestamp", base_fee_per_gas, base_fee_per_gas / 1e9 AS base_fee_per_gas_gwei FROM eth.blocks limit 2000""");

        var enumerator = result.GetAsyncEnumerator();

        var totalTows = 0;
        var numBatches = 0;
        while (await enumerator.MoveNextAsync())
        {
            var batch = enumerator.Current;
            numBatches += 1;
            totalTows += batch.Length;
        }

        Assert.Multiple(() =>
        {
            Assert.That(numBatches, Is.Not.EqualTo(1));
            Assert.That(totalTows, Is.EqualTo(2000));
        });
    }
}