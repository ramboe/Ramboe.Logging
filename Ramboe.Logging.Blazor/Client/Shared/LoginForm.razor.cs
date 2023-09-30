using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ramboe.Logging.Blazor.Client.Data;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Shared;

public partial class LoginForm
{
    #region injections
    [Inject] public HttpLoginClient HttpLoginClient { get; set; }

    [Inject] public ILocalStorageService LocalStorageService { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }

    [Inject] ISnackbar Snackbar { get; set; }
    #endregion

    public LoginTokenModel LoginTokenModel { get; set; }

    public LoginModel LoginModel { get; set; } = new LoginModel();

    public string Token { get; set; }

    MudForm form;

    async Task SaveToLocalStorage()
    {
        async Task execute()
        {
            LoginTokenModel = await HttpLoginClient.Login(LoginModel.Email, LoginModel.Password);

            Token = LoginTokenModel.Token;

            await save(Token);

            Snackbar.Add("submitted");
            Snackbar.Add(Token);

            return;

            async Task save(string tokenFromLogin)
            {
                await LocalStorageService.SetItemAsStringAsync(Constants.STORAGE_TOKENKEY, tokenFromLogin);

                await Task.Delay(1000);

                //reload page now that we are authorized
                NavigationManager.NavigateTo(NavigationManager.Uri, true);
            }
        }

        await TryAsync(execute);
    }
}
