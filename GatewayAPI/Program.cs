using GatewayAPI;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vault;
using Vault.Client;
using Vault.Model;
using Serilog;
using Serilog.Events;

// Sleep for 5 seconds to make sure that services have registered their access tokens in the Vault
Thread.Sleep(15000);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add serilog logger instance for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341/")
    .CreateLogger();

builder.Host.UseSerilog();

// ========= HASHICORP VAULT ========= 
// Vault access to get JWT Tokens for service access
string address = Environment.GetEnvironmentVariable("VAULT_URL")!;

VaultConfiguration config = new VaultConfiguration(address, null, null, null, null);

VaultClient vaultClient = new VaultClient(config);
vaultClient.SetToken(Environment.GetEnvironmentVariable("VAULT_TOKEN"));

ServiceAccessTokens tokens = new();
bool getTokensSucess = false;
try
{
    // Get tokens
    VaultResponse<KvV2ReadResponse> ast = vaultClient.Secrets.KvV2Read("AccountServiceToken", "secret");
    Thread.Sleep(500);
    VaultResponse<KvV2ReadResponse> pst = vaultClient.Secrets.KvV2Read("PostServiceToken", "secret");
    Thread.Sleep(500);
    VaultResponse<KvV2ReadResponse> tst = vaultClient.Secrets.KvV2Read("TimelineServiceToken", "secret");
    
    // Write to tokens access object
    //tokens.AccountServiceToken = ((KeyValuePair<string, string>)ast.Data.Data).Value;
    tokens.AccountServiceToken = ((JObject)ast.Data.Data)["AccountServiceToken"]!.ToString();
    //tokens.PostServiceToken = ((KeyValuePair<string, string>)pst.Data.Data).Value;
    tokens.PostServiceToken = ((JObject)pst.Data.Data)["PostServiceToken"]!.ToString();
    //tokens.TimelineServiceToken = ((KeyValuePair<string, string>)tst.Data.Data).Value;
    tokens.TimelineServiceToken = ((JObject)tst.Data.Data)["TimelineServiceToken"]!.ToString();
    
    getTokensSucess = true;
}
catch (VaultApiException e) { Console.WriteLine("Failed to read secret with message {0}", e.Message); }
// Throw if we fail to get tokens
if (!getTokensSucess) {
    throw new Exception($"Failed to get account service token");
}
// Make tokens available as dependency injectable instance
builder.Services.AddSingleton(tokens);
// ========= HASHICORP VAULT END =========

// Retry layer for http requests to services.
builder.Services.AddTransient<RetryPollyLayer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();