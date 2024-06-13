namespace Spice.Config;

internal static class SpiceDefaultConfigLocal
{
    public static readonly string FlightAddress = Environment.GetEnvironmentVariable("SPICE_LOCAL_FLIGHT_URL") ?? "grpc://localhost:50051";
}