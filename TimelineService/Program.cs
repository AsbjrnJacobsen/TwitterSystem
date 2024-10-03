using Microsoft.EntityFrameworkCore;
using TimelineService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<TimelineService.Services.TimelineService>();
builder.Services.AddDbContext<TSDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountsDBConnection"));
});
builder.Services.AddDbContext<TSDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostsDBConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

/* - Responsible for Timeline creating - READ ONLY
Methods:
Get Random Posts


*/