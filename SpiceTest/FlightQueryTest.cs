/*
Copyright 2024 The Spice.ai OSS Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
            .WithApiKey(ApiKey)
            .WithSpiceCloud()
            .Build();
    }

    [Test]
    [Ignore("Ignoring cloud tests for now")]
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