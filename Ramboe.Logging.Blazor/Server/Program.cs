using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ramboe.Logging.Blazor.Server.Data;
using Ramboe.Logging.Blazor.Server.Middleware;
using Ramboe.Logging.Blazor.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddTransient<CredentialsService>();
builder.Services.AddTransient<ErrorMessageMiddleware>();

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Logging");
builder.Services.AddNpgsqlDataSource(connectionString);
builder.Services.AddTransient<ILogRepository, DapperLogRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = false,
               ValidIssuer = "your-issuer",
               ValidAudience = "your-audience",
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("FF03CC39-583B-45A7-8A27-3B51632D3149"))
           };

           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context => {
                   if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                   {
                   }

                   return Task.CompletedTask;
               }
           };
       });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseMiddleware<ErrorMessageMiddleware>();

app.Run();
