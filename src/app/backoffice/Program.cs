using Conventions.BackOffice;
using Conventions.BackOffice.Models;
using Conventions.Services;
using Conventions.Services.TypedClients;
using Conventions.Storage;
using Conventions.Storage.Mongo;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using System.Text.Json;
using Conventions.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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

builder.Services.Configure<MongoConventionStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Conventions:CollectionName");
});
builder.Services.Configure<MongoUserStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Users:CollectionName");
});
builder.Services.Configure<MongoEventStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Events:CollectionName");
});
builder.Services.Configure<MongoVenueStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Venues:CollectionName");
});
builder.Services.Configure<MongoEventReservationStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:Reservations:CollectionName");
});
builder.Services.Configure<MongoConventionEventAssociationStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:ConventionEvents:CollectionName");
});
builder.Services.Configure<MongoConventionVenueAssociationStoreOptions>(options => {
    options.ConnectionString = builder.Configuration.GetValue<string>("Storage:Mongo:ConnectionString");
    options.DatabaseName = builder.Configuration.GetValue<string>("Storage:Mongo:DatabaseName");
    options.CollectionName = builder.Configuration.GetValue<string>("Storage:Mongo:ConventionVenues:CollectionName");
}); 
builder.Services.Configure<SignoutControllerOptions>(options =>
{
    options.Authority = "https://localhost:7200";
    options.RedirectUrl = "https://localhost:7100";
});
builder.Services.Configure<BackOfficeServerTokenClientOptions>(options =>
{
    options.Authority = builder.Configuration.GetValue<string>("Identity:Base");
    options.ClientId = builder.Configuration.GetValue<string>("Identity:Clients:s2s:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("Identity:Clients:s2s:ClientSecret");
});

builder.Services.AddSingleton<IConventionStore, MongoConventionStore>();
builder.Services.AddSingleton<IUserStore, MongoUserStore>();
builder.Services.AddSingleton<IVenueStore, MongoVenueStore>();
builder.Services.AddSingleton<IEventStore, MongoEventStore>();
builder.Services.AddSingleton<IEventRegistrationStore, MongoEventReservationStore>();
builder.Services.AddSingleton<IConventionEventAssociationStore, MongoConventionEventAssociationStore>();
builder.Services.AddSingleton<IConventionVenueAssociationStore, MongoConventionVenueAssociationStore>();

builder.Services.AddScoped<IConventionService, ConventionService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<BlazorAccessTokenProvider>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<UserClient>(options =>
    {
        options.BaseAddress = new Uri("https://localhost:7001");
    });
builder.Services.AddHttpClient<ConventionClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7001");
});
builder.Services.AddHttpClient<VenueClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7001");
});
builder.Services.AddHttpClient<EventClient>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7001");
});
builder.Services.AddHttpClient<BackOfficeServerTokenClient>();
builder.Services.AddHttpClient<ProfileClient>(options => {
    options.BaseAddress = new Uri("https://localhost:7001");
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();


builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("cookies", options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = builder.Configuration.GetValue<string>("Identity:Base");
                    options.RequireHttpsMetadata = false;

                    options.ClientId = builder.Configuration.GetValue<string>("Identity:Clients:oidc:ClientId");
                    options.ClientSecret = builder.Configuration.GetValue<string>("Identity:Clients:oidc:ClientSecret");
                    options.ResponseType = "code";
                    
                    options.UsePkce = true;
                    options.SaveTokens = true;
                    
                    options.CallbackPath = "/signin-oidc";
                    options.SignedOutCallbackPath = "/signout-oidc";

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("roles");

                    options.ClaimActions.MapJsonKey(JwtClaimTypes.Role, "role", "role");
                    options.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;
                    options.TokenValidationParameters.IssuerSigningKey = x509SecurityKey;
                    options.TokenValidationParameters.ValidAudience = builder.Configuration.GetValue<string>("Identity:Clients:oidc:ClientId");
                    options.TokenValidationParameters.ValidateAudience = true;

                    options.Events.OnTokenValidated = (context) =>
                    {
                        return Task.CompletedTask;
                    };
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
    options.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
    options.AddPolicy(Policies.IsTalker, Policies.IsTalkerPolicy());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();
