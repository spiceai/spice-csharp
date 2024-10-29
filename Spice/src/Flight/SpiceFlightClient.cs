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

using System.Net.Http.Headers;
using Apache.Arrow.Flight;
using Apache.Arrow.Flight.Client;
using Grpc.Core;
using Grpc.Net.Client;
using Polly;
using Polly.Retry;
using Spice.Auth;
using Spice.Config;
using Spice.Errors;

namespace Spice.Flight;

internal class SpiceFlightClient
{
    private readonly FlightClient _flightClient;
    private readonly AsyncRetryPolicy _retryPolicy;

    private static GrpcChannelOptions GetGrpcChannelOptions(string? appId, string? apiKey)
    {
        var options = new GrpcChannelOptions();

        if (appId == null || apiKey == null) return options;


        options.Credentials = ChannelCredentials.SecureSsl;
        options.HttpClient = new HttpClient
        {
            DefaultRequestHeaders =
            {
                Authorization = AuthHeaderBuilder.BasicAuth(appId, apiKey)
            }
        };
        
        options.HttpClient.DefaultRequestHeaders.Add("X-Spice-User-Agent", UserAgent.agent());

        return options;
    }

    private static Metadata.Entry? GetAuthToken(Metadata responseHeaders, Metadata trailers)
    {
        return responseHeaders.Get("authorization") ?? trailers.Get("authorization");
    }

    internal SpiceFlightClient(string address, int maxRetries, string? appId, string? apiKey)
    {
        _retryPolicy = Policy.Handle<RpcException>(ex =>
                ex.Status.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Aborted
                    or StatusCode.Internal or StatusCode.Unknown)
            .WaitAndRetryAsync(retryCount: maxRetries,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt * 1.5),
                onRetry: (_, timespan, retryAttempt, _) =>
                {
                    Console.WriteLine(
                        $"Request failed. Waiting {timespan} before next retry. Retry attempt {retryAttempt}");
                });

        var options = GetGrpcChannelOptions(appId, apiKey);

        _flightClient = new FlightClient(GrpcChannel.ForAddress(address, options));

        if (appId == null || apiKey == null)
        {
            return;
        }

        var stream = _flightClient.Handshake();

        stream.ResponseHeadersAsync.Wait();

        var token = GetAuthToken(stream.ResponseHeadersAsync.Result, stream.GetTrailers());
        if (token == null || options.HttpClient == null) throw new SpiceException(SpiceStatus.FailedToAuthenticate, "Failed to authenticate");

        options.HttpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token.Value);
    }

    internal async Task<FlightClientRecordBatchStreamReader> Query(string sql)
    {
        if (string.IsNullOrEmpty(sql))
        {
            throw new ArgumentException("No SQL provided");
        }

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var descriptor = FlightDescriptor.CreateCommandDescriptor(sql);
            var flightInfo = await _flightClient.GetInfo(descriptor);

            var endpoint = flightInfo.Endpoints.FirstOrDefault();
            if (endpoint == null) throw new Exception("Failed to get endpoint");

            var stream = _flightClient.GetStream(endpoint.Ticket);
            return stream.ResponseStream;
        });
    }
}