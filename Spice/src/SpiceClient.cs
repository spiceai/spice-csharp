using Apache.Arrow.Flight.Client;
using Spice.Config;
using Spice.Flight;

namespace Spice;

public class SpiceClient
{
    
    /// <summary>
    /// Gets or sets the application ID. This property is internal set and can be null.
    /// </summary>
    public string? AppId { get; internal set; }

    /// <summary>
    /// Gets or sets the API key. This property is internal set and can be null.
    /// </summary>
    public string? ApiKey { get; internal set; }

    /// <summary>
    /// Gets or sets the flight address. This property is internal set and defaults to local flight endpoint.
    /// </summary>
    public string FlightAddress { get; internal set; } = SpiceDefaultConfigLocal.FlightAddress;

    /// <summary>
    /// Gets or sets the maximum number of retries. This property is internal set and defaults to 3.
    /// </summary>
    public int MaxRetries { get; internal set; } = 3;

    private SpiceFlightClient? FlightClient { get; set; }

    internal void Init()
    {
        FlightClient = new SpiceFlightClient(FlightAddress, AppId, ApiKey);
    }

    /// <summary>
    /// Runs the query against the Flight endpoint. 
    /// </summary>
    /// <returns>A task representing asynchronus operation, with a result of type <see cref="FlightClientRecordBatchStreamReader"/></returns>
    /// <param name="sql">SQL to be executed against Spice</param>
    /// <exception cref="System.ArgumentException">Thrown when provided sql is null or empty</exception>
    /// <exception cref="Spice.Errors.SpiceException">Spice exception</exception>
    /// <exception cref="Grpc.Core.RpcException">gRPC exception</exception>
    public Task<FlightClientRecordBatchStreamReader> Query(string sql)
    {
        if (FlightClient == null) throw new Exception("FlightClient not initialized");

        return FlightClient.Query(sql);
    }
}