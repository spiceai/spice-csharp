namespace Spice.config;

internal static class SpiceDefaultConfigCloud 
{
    public static string HttpAddress { get; } = Environment.GetEnvironmentVariable("SPICE_HTTP_URL") ?? "https://data.spiceai.io";
    public static string FlightAddress { get; } = Environment.GetEnvironmentVariable("SPICE_FLIGHT_URL") ?? "https://flight.spiceai.io:443";
    public static string FirecacheAddress { get; } = Environment.GetEnvironmentVariable("SPICE_FIRECACHE_URL") ?? "https://firecache.spiceai.io:443";
}