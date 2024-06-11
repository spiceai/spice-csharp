using System.Net.Http.Headers;
using System.Text;
using Apache.Arrow;
using Apache.Arrow.Flight;
using Apache.Arrow.Flight.Client;
using Grpc.Core;
using Grpc.Net.Client;

namespace Spice.flight;

internal class SpiceFlightClient
{
    private readonly FlightClient _flightClient;

    internal SpiceFlightClient(string address, string? appId, string? apiKey)
    {
        GrpcChannelOptions options;
        if (appId != null && apiKey != null)
            options = new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl,
                HttpClient = new HttpClient
                {
                    DefaultRequestHeaders =
                    {
                        Authorization = AuthenticationHeaderValue.Parse("Basic " +
                                                                        Convert.ToBase64String(Encoding
                                                                            .UTF8
                                                                            .GetBytes($"{appId}:{apiKey}")))
                    }
                }
            };
        else
            options = new GrpcChannelOptions();

        _flightClient = new FlightClient(GrpcChannel.ForAddress(address, options));
        var stream = _flightClient.Handshake();

        stream.ResponseHeadersAsync.Wait();
        var md = stream.ResponseHeadersAsync.Result;
        var token = md.Get("authorization");
        if (token == null || options.HttpClient == null) throw new Exception("Failed to authenticate token");

        options.HttpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token.Value);
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

    internal IEnumerable<RecordBatch> Query(string sql)
    {
        var descriptor = FlightDescriptor.CreateCommandDescriptor(sql);
        var request = _flightClient.GetInfo(descriptor);
        request.ResponseAsync.Wait();

        var flightInfo = request.ResponseAsync.Result;
        var endpoint = flightInfo.Endpoints.FirstOrDefault();
        if (endpoint == null) throw new Exception("Failed to get endpoint");

        var stream = _flightClient.GetStream(endpoint.Ticket);
        return stream.ResponseStream.ReadAllAsync().ToBlockingEnumerable();
    }
}