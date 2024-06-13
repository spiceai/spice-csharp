using System.Net.Http.Headers;
using System.Text;

namespace Spice.Auth;

internal static class AuthHeaderBuilder
{
    public static AuthenticationHeaderValue BasicAuth(string appId, string apiKey)
    {
        return AuthenticationHeaderValue.Parse("Basic " +
                                        Convert.ToBase64String(Encoding
                                            .UTF8
                                            .GetBytes($"{appId}:{apiKey}")));
    }
}