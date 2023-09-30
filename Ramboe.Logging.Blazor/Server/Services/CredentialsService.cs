using System.Text.Json;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Services;

public class CredentialsService
{
    private readonly string _filePath = "credentials.json";// Change this path as needed

    public Credentials Validate(LoginModel credentials)
    {
        var credentialsFromFile = GetCredentials();

        var userNameCorrect = credentials.Email.ToLower() == credentialsFromFile.Email.ToLower();
        var passwordCorrect = credentials.Password.ToLower() == credentialsFromFile.Password.ToLower();

        if (userNameCorrect && passwordCorrect)
        {
            return credentialsFromFile;
        }

        throw new Exception("username and or password wrong");
    }

    public Credentials GetCredentials()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("missing file 'credentials.json'");
        }

        var json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<Credentials>(json) ?? throw new Exception("'credentials.json' is empty");
    }

    public void UpdateCredentials(Credentials updatedCredentials)
    {
        var json = JsonSerializer.Serialize(updatedCredentials);
        File.WriteAllText(_filePath, json);
    }
}
