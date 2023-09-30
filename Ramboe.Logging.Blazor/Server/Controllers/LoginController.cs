using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ramboe.Logging.Blazor.Server.Services;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    readonly CredentialsService _credentials;

    public LoginController(CredentialsService credentials)
    {
        _credentials = credentials;
    }

    [Authorize] [HttpGet(nameof(GetCurrentUser))] public Credentials GetCurrentUser()
    {
        return _credentials.GetCredentials();
    }

    [Authorize] [HttpPatch(nameof(UpdateCredentialFile))] public void UpdateCredentialFile(Credentials model)
    {
        _credentials.UpdateCredentials(model);
    }

    [HttpPost] public LoginTokenModel Login(LoginModel model)
    {
        var user = _credentials.Validate(model);

        // Generate a JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("FF03CC39-583B-45A7-8A27-3B51632D3149");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)

                // new Claim(ClaimTypes.Role, user.Role)
            }),

            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var loginTokenResponse = new LoginTokenModel
        {
            Token = tokenString,
            Type = LoginTokenModel.LoginResponseTypes.LoginSuccessful
        };

        return loginTokenResponse;
    }
}
