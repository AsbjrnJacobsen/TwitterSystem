using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Vault;
using Vault.Client;
using Vault.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<TimelineService.Services.TimelineService>();
builder.Services.AddDbContext<ASDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountsDBConnection"));
});
builder.Services.AddDbContext<PSDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostsDBConnection"));
});

// ===== Generate JWT Token for access to service =====
var jwtKey = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(RandomNumberGenerator.GetBytes(38)));
var securityKey = new SymmetricSecurityKey(jwtKey);
var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var securityToken = new JwtSecurityToken(
    "Twitter-System",
    "Twitter-System",
    null,
    expires: DateTime.MaxValue,
    signingCredentials: credentials);
var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
// ===== Generate JWT Token END =====

// ===== JWT Authentication middleware =====
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Twitter-System",
        ValidAudience = "Twitter-System",
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
    };
});
// ===== JWT Authentication middleware END =====

// ========= HASHICORP VAULT ========= 
// Vault access to get JWT Tokens for service access
string address = Environment.GetEnvironmentVariable("VAULT_URL")!;
VaultConfiguration config = new VaultConfiguration(address);
VaultClient vaultClient = new VaultClient(config);
vaultClient.SetToken(Environment.GetEnvironmentVariable("VAULT_TOKEN"));
bool wroteTokenSuccess = false;
try
{
    var secretData = new Dictionary<string, string>{{"TimelineServiceToken", token}};
    var kvRequestData = new KvV2WriteRequest(secretData);
    vaultClient.Secrets.KvV2Write("TimelineServiceToken", kvRequestData);
    wroteTokenSuccess = true;
}
catch (VaultApiException e) { Console.WriteLine("Failed to read secret with message {0}", e.Message); }
if (!wroteTokenSuccess)
    throw new Exception("Failed to write secret to vault.");
// ========= HASHICORP VAULT END =========

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();