using Spice;

namespace SpiceTest;

public class FlightQueryTest
{
    [Test]
    public async Task TestDoGetAsync()
    {
        var client = new SpiceClientBuilder()
            .WithApiKey("323337|b42eceab2e7c4a60a04ad57bebea830d")
            .WithSpiceCloud()
            .Build();
        var result =
            client.DoGetAsync("""SELECT number, "timestamp", hash FROM eth.recent_blocks ORDER BY number LIMIT 10""");

        while (await result.MoveNextAsync())
        {
            var batch = result.Current;
            Assert.That(batch.Length, Is.EqualTo(10));
        }
    }

    [Test]
    public void TestDoGetSync()
    {
        var client = new SpiceClientBuilder()
            .WithApiKey("323337|b42eceab2e7c4a60a04ad57bebea830d")
            .WithSpiceCloud()
            .Build();
        var result =
            client.DoGet("""SELECT number, "timestamp", hash FROM eth.recent_blocks ORDER BY number LIMIT 10""");

        foreach (var batch in result) Assert.That(batch.Length, Is.EqualTo(10));
    }
}