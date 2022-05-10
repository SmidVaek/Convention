using Conventions.Identity.IdentityServer;
using Conventions.Models;
using Conventions.Services;
using Conventions.Storage;
using Conventions.Storage.Mongo;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#region SIGNING CERTIFICATE
var certObjBytes = Convert.FromBase64String(builder.Configuration.GetValue<string>("Identity:signingCertificate"));
var certObj = JsonSerializer.Deserialize<Certificate>(certObjBytes, new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true
});
var certBytes = Convert.FromBase64String(certObj.Data);
var signingCertificate = new X509Certificate2(certBytes, certObj.Password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.EphemeralKeySet);
var x509SecurityKey = new X509SecurityKey(signingCertificate);
#endregion


// Add services to the container.
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();

builder.Services.Configure<MongoUserStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Users:CollectionName");
});
builder.Services.AddSingleton<IUserStore, MongoUserStore>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginReturnUrlParameter = "returnUrl";
                    options.UserInteraction.LoginUrl = "~/login";
                    options.UserInteraction.ConsentUrl = "~/consent";
                    options.UserInteraction.LogoutUrl = "~/logout";
                    options.UserInteraction.ErrorUrl = "~/error";
                })
                .AddProfileService<ProfileService>()
                // Should add proper signing cert rotation
                .AddSigningCredential(signingCertificate)
                // Should really get these from proper storage
                .AddInMemoryApiResources(MockConfig.GetApiResources())
                .AddInMemoryApiScopes(MockConfig.GetApiScopes())
                .AddInMemoryIdentityResources(MockConfig.GetIdentityResources())
                .AddInMemoryClients(MockConfig.GetClients());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
