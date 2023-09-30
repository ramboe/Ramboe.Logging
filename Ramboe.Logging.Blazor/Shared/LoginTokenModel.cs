namespace Ramboe.Logging.Blazor.Shared;

public class LoginTokenModel
{
    public string Token { get; set; }

    public LoginResponseTypes Type { get; set; }

    public enum LoginResponseTypes
    {
        LoginSuccessful,
        ActivationNeeded
    }
}
