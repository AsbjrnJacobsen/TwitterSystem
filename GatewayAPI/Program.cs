using GatewayAPI;
using Vault;
using Vault.Client;
using Vault.Model;

// Sleep for 5 seconds to make sure that services have registered their access tokens in the Vault
Thread.Sleep(5000);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Retry layer for http requests to services.
builder.Services.AddTransient<RetryPollyLayer>();

// ========= HASHICORP VAULT ========= 
// Vault access to get JWT Tokens for service access
string address = Environment.GetEnvironmentVariable("VAULT_URL")!;
VaultConfiguration config = new VaultConfiguration(address);
VaultClient vaultClient = new VaultClient(config);
vaultClient.SetToken(Environment.GetEnvironmentVariable("VAULT_TOKEN"));

ServiceAccessTokens tokens = new();
bool getTokensSucess = false;
try
{
    // Get tokens
    VaultResponse<KvV2ReadResponse> ast = vaultClient.Secrets.KvV2Read("AccountServiceToken");
    VaultResponse<KvV2ReadResponse> pst = vaultClient.Secrets.KvV2Read("PostServiceToken");
    VaultResponse<KvV2ReadResponse> tst = vaultClient.Secrets.KvV2Read("TimelineServiceToken");
    
    // Write to tokens access object
    tokens.AccountServiceToken = ast.Data.Data.ToString()!;
    tokens.PostServiceToken = ast.Data.Data.ToString()!;
    tokens.TimelineServiceToken = ast.Data.Data.ToString()!;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();