namespace Spice.config;

internal static class SpiceDefaultConfigLocal
{
    public static string HttpAddress { get; } = Environment.GetEnvironmentVariable("SPICE_LOCAL_HTTP_URL") ?? "http://localhost:3000";
    public static string FlightAddress { get; } = Environment.GetEnvironmentVariable("SPICE_LOCAL_FLIGHT_URL") ?? "grpc://localhost:50051";
    public static string FirecacheAddress { get; } = Environment.GetEnvironmentVariable("SPICE_FIRECACHE_URL") ?? "firecache.spiceai.io:443";
}