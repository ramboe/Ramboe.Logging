namespace WebService1.TypedHttpClients;

public class HttpWebService2Client
{
    readonly HttpClient _client;

    public HttpWebService2Client(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetAsync()
    {
        return await _client.GetStringAsync("");
    }
}
