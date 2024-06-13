using Spice.Config;

namespace Spice;

public class SpiceClientBuilder
{
    private readonly SpiceClient _spiceClient = new();

    public SpiceClientBuilder WithFlightAddress(string flightAddress)
    {
        _spiceClient.FlightAddress = flightAddress;
        return this;
    }

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

    public SpiceClientBuilder WithMaxRetries(int maxRetries)
    {
        _spiceClient.MaxRetries = maxRetries;
        return this;
    }

    public SpiceClient Build()
    {
        _spiceClient.Init();
        return _spiceClient;
    }
    
}