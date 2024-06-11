using Apache.Arrow;
using Spice.config;
using Spice.flight;

namespace Spice;

public class SpiceClient
{
    public string? AppId { get; internal set; }
    public string? ApiKey { get; internal set; }
    public string FlightAddress { get; internal set; } = SpiceDefaultConfigLocal.FlightAddress;
    public string FirecacheAddress { get; internal set; } = SpiceDefaultConfigLocal.FirecacheAddress;
    public string HttpAddress { get; internal set; } = SpiceDefaultConfigLocal.HttpAddress;
    public int MaxRetries { get; internal set; } = 3;

    private SpiceFlightClient? FlightClient { get; set; }

    internal void Init()
    {
        FlightClient = new SpiceFlightClient(FlightAddress, AppId, ApiKey);
    }

    public IAsyncEnumerator<RecordBatch> DoGet(string sql)
    {
        if (FlightClient == null)
        {
            throw new Exception("FlightClient not initialized");
        }

        return FlightClient.Query(sql);
    }
}