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
    /// Gets or sets the User-Agent string. This property is internal set and can be null.
    /// </summary>
    public string? UserAgent { get; internal set; }

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
        FlightClient = new SpiceFlightClient(FlightAddress, MaxRetries, AppId, ApiKey, UserAgent);
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