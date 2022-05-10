using Conventions.Models;
using Conventions.Services;
using Conventions.Storage;
using Conventions.Storage.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
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
var securityKey = new X509SecurityKey(signingCertificate);
#endregion

#region CONFIGURATION
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
#endregion

#region SERVICES
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
builder.Services.AddScoped<IAssociationService, AssociationService>();
#endregion

#region AUTHENTICATION
builder.Services.AddAuthentication()
    .AddJwtBearer("s2s", options =>
    {
        var issuer = builder.Configuration.GetValue<string>("identity:base");
        options.Authority = issuer;
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidAudience = "backoffice.s2s";
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.IssuerSigningKey = securityKey;
        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }
    })
    .AddJwtBearer("oidc", options =>
    {
        var issuer = builder.Configuration.GetValue<string>("identity:base");
        options.Authority = issuer;
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.IssuerSigningKey = securityKey;
        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("s2s", policy =>
    {
        policy.RequireAuthenticatedUser()
              .RequireClaim("aud", "backoffice.s2s")
              .AddAuthenticationSchemes("s2s");
    });

    options.AddPolicy("oidc", policy =>
    {
        policy.RequireAuthenticatedUser()
                .AddAuthenticationSchemes("oidc");
    });
});

#endregion

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conventions.API", Version = "v1" });

    var bearerSecurityDefinition = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "jwt",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    var securityRequirements = new OpenApiSecurityRequirement()
    {
        { bearerSecurityDefinition, Array.Empty<string>() }
    };

    c.AddSecurityDefinition("Bearer", bearerSecurityDefinition);
    c.AddSecurityRequirement(securityRequirements);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
} 
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApi v1"));
}

app.UseStaticFiles();

app.UseCors(config =>
{
    config.SetIsOriginAllowed(origin =>
    {
        var _corsAllowedOriginsSection = app.Configuration.GetSection("Cors:AllowedOrigins");
        var _corsAllowedOrigins = _corsAllowedOriginsSection.Get<string[]>();
        return _corsAllowedOrigins.Contains(origin);
    })
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
