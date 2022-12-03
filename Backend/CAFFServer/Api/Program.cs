using Api.Modules;
using Api.Services;
using Application.Extensions;
using Application.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dal;
using Domain.Entities.User;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Events;
using System.IdentityModel.Tokens.Jwt;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication
    .CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.ConfigureApplicationLayer(builder.Configuration);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new RepositoryModule());
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<WebshopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<WebshopUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<WebshopDbContext>();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryPersistedGrants()
    .AddInMemoryIdentityResources(builder.Configuration.GetSection("IdentityServer:IdentityResources"))
    .AddInMemoryApiResources(builder.Configuration.GetSection("IdentityServer:ApiResources"))
    .AddInMemoryApiScopes(builder.Configuration.GetSection("IdentityServer:ApiScopes"))
    .AddInMemoryClients(builder.Configuration.GetSection("IdentityServer:Clients"))
    .AddAspNetIdentity<WebshopUser>()
    .AddProfileService<ProfileService>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Authentication:Authority");
        options.Audience = builder.Configuration.GetValue<string>("Authentication:Audience");
        options.RequireHttpsMetadata = false;
    }
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => policy.RequireAuthenticatedUser()
        .RequireClaim("role", "user", "admin")
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));

    options.AddPolicy("Admin", policy => policy.RequireAuthenticatedUser()
        .RequireClaim(JwtClaimTypes.Role, "admin")
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
});

builder.Services.AddScoped<IIdentityService, IdentityService>();


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Webshop API";
    config.Description = "CIFF/CAFF webshop api";
    config.DocumentName = "Webshop";

    config.AddSecurity("OAuth2", new OpenApiSecurityScheme
    {
        OpenIdConnectUrl =
            $"{builder.Configuration.GetValue<string>("Authentication:Authority")}/.well-known/openid-configuration",
        Scheme = "Bearer",
        Type = OpenApiSecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl =
                    $"{builder.Configuration.GetValue<string>("Authentication:Authority")}/connect/authorize",
                TokenUrl = $"{builder.Configuration.GetValue<string>("Authentication:Authority")}/connect/token",
                Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenId" },
                                { "full-access", "full-access" }
                            }
            }
        }
    });

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("OAuth2"));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<WebshopDbContext>();
    dataContext.Database.Migrate();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseOpenApi();
app.UseSwaggerUi3(config =>
{
    config.OAuth2Client = new OAuth2ClientSettings
    {
        ClientId = "webshop_swagger",
        ClientSecret = null,
        UsePkceWithAuthorizationCodeGrant = true,
        ScopeSeparator = " ",
        Realm = null,
        AppName = "Webshop Swagger Client"
    };
});

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
