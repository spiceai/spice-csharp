using System.Net.Http.Headers;
using Apache.Arrow;
using Apache.Arrow.Flight;
using Apache.Arrow.Flight.Client;
using Grpc.Core;
using Grpc.Net.Client;
using Spice.Extension;
using Spice.Auth;

namespace Spice.Flight;

internal class SpiceFlightClient
{
    private readonly FlightClient _flightClient;

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

        return options;
    }

    private static Metadata.Entry? GetAuthToken(Metadata responseHeaders, Metadata trailers)
    {
        return responseHeaders.Get("authorization") ?? trailers.Get("authorization");
    }

    internal SpiceFlightClient(string address, string? appId, string? apiKey)
    {
        var options = GetGrpcChannelOptions(appId, apiKey);

        _flightClient = new FlightClient(GrpcChannel.ForAddress(address, options));
        
        var stream = _flightClient.Handshake();

        stream.ResponseHeadersAsync.Wait();

        var token = GetAuthToken(stream.ResponseHeadersAsync.Result, stream.GetTrailers()); 
        if (token == null || options.HttpClient == null) throw new Exception("Failed to authenticate token");

        options.HttpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token.Value);
    }

    internal IEnumerable<RecordBatch> Query(string sql)
    {
        var descriptor = FlightDescriptor.CreateCommandDescriptor(sql);
        var request = _flightClient.GetInfo(descriptor);
        request.ResponseAsync.Wait();

        var flightInfo = request.ResponseAsync.Result;
        var endpoint = flightInfo.Endpoints.FirstOrDefault();
        if (endpoint == null) throw new Exception("Failed to get endpoint");

        var stream = _flightClient.GetStream(endpoint.Ticket);
        return stream.ResponseStream.ToBlockingEnumerable();
    }

    internal async IAsyncEnumerator<RecordBatch> QueryAsync(string sql)
    {
        var descriptor = FlightDescriptor.CreateCommandDescriptor(sql);

        var flightInfo = await _flightClient.GetInfo(descriptor);
        var endpoint = flightInfo.Endpoints.FirstOrDefault();
        if (endpoint == null) throw new Exception("Failed to get endpoint");

        var stream = _flightClient.GetStream(endpoint.Ticket);
        await foreach (var batch in stream.ResponseStream) yield return batch;
    }
}