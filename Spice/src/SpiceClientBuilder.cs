using Spice.Config;

namespace Spice;

public class SpiceClientBuilder
{
    private readonly SpiceClient _spiceClient = new();

    /// <summary>
    /// Sets the client's flight address.
    /// </summary>
    /// <param name="flightAddress">Flight address the client will query</param>
    /// <returns>The current instance of <see cref="SpiceClientBuilder"/> for method chaining.</returns>
    public SpiceClientBuilder WithFlightAddress(string flightAddress)
    {
        _spiceClient.FlightAddress = flightAddress;
        return this;
    }

    /// <summary>
    /// Sets the client's apiKey token and default paths flight path to Spice Cloud.
    /// </summary>
    /// <param name="apiKey">The Spice Cloud api key</param>
    /// <returns>The current instance of <see cref="SpiceClientBuilder"/> for method chaining.</returns>
    /// <exception cref="System.ArgumentException">Thrown when the apiKey is in wrong format.</exception>
    public SpiceClientBuilder WithSpiceCloud(string apiKey)
    {
        var parts = apiKey.Split('|');
        if (parts.Length != 2)
        {
            throw new ArgumentException("apiKey is invalid");
        }

        _spiceClient.AppId = parts[0];
        _spiceClient.ApiKey = apiKey;

        _spiceClient.FlightAddress = SpiceDefaultConfigCloud.FlightAddress;
        return this;
    }

    /// <summary>
    /// Sets the client's max retries count. 
    /// </summary>
    /// <param name="maxRetries">Max retries for request</param>
    /// <returns>The current instance of <see cref="SpiceClientBuilder"/> for method chaining.</returns>
    public SpiceClientBuilder WithMaxRetries(int maxRetries)
    {
        _spiceClient.MaxRetries = maxRetries;
        return this;
    }

    /// <summary>
    /// Initiates <see cref="SpiceClient" /> with provided parameters.
    /// </summary>
    /// <returns>The current instance of <see cref="SpiceClient"/> for method chaining.</returns>
    /// <exception cref="Spice.Errors.SpiceException">Spice exception</exception>
    /// <exception cref="Grpc.Core.RpcException">gRPC exception</exception>
    public SpiceClient Build()
    {
        _spiceClient.Init();
        return _spiceClient;
    }
}