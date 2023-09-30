namespace Ramboe.Logging.Blazor.Shared;

public class LogModel
{
    public string Id { get; set; }

    public string level { get; set; }

    public DateTime raise_date { get; set; }

    public string service { get; set; }

    public string http_path { get; set; }

    public string http_verb { get; set; }

    public string http_body { get; set; }

    public string http_queryparams { get; set; }

    public string message_short { get; set; }

    public string location_in_stacktrace { get; set; }

    public string stack_trace { get; set; }
}
