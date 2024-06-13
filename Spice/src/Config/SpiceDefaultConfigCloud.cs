namespace Spice.Config;

internal static class SpiceDefaultConfigCloud
{
    public static readonly string FlightAddress =
        Environment.GetEnvironmentVariable("SPICE_FLIGHT_URL") ?? "https://flight.spiceai.io:443";
}