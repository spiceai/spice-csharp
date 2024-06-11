using Spice.config;

namespace Spice;

public class SpiceClientBuilder
{
    private readonly SpiceClient _spiceClient = new();

    public SpiceClientBuilder WithApiKey(string apiKey)
    {
        var parts = apiKey.Split('|');
        if (parts.Length != 2)
        {
            throw new ArgumentException("apiKey is invalid");
        }
        _spiceClient.AppId = parts[0];
        _spiceClient.ApiKey = apiKey;
        return this;
    }

    public SpiceClientBuilder WithHttpAddress(string httpAddress)
    {
        _spiceClient.HttpAddress = httpAddress;
        return this;
    }

    public SpiceClientBuilder WithFlightAddress(string flightAddress)
    {
        _spiceClient.FlightAddress = flightAddress;
        return this;
    }

    public SpiceClientBuilder WithFirecacheAddress(string firecacheAddress)
    {
        _spiceClient.FirecacheAddress = firecacheAddress;
        return this;
    }

    public SpiceClientBuilder WithSpiceCloud()
    {
        _spiceClient.HttpAddress = SpiceDefaultConfigCloud.HttpAddress;
        _spiceClient.FlightAddress = SpiceDefaultConfigCloud.FlightAddress;
        _spiceClient.FirecacheAddress = SpiceDefaultConfigCloud.FirecacheAddress;
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