using System.Net;
using Demo.Configs;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind the RedisConfig section from appsettings.json to IOptions
builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("Redis"));
var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfig>();

// Register StackExchange.Redis as a distributed cache using the bound options
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{redisConfig.Host}:{redisConfig.Port},password={redisConfig.Password},defaultDatabase={redisConfig.Database}";
});

// Register the RedLock factory as a singleton in the service container
var redisConnections = new[]
{
    new RedLockEndPoint
    {
        EndPoint = new DnsEndPoint(redisConfig!.Host!, redisConfig.Port),
        Password = redisConfig.Password,
        RedisDatabase = redisConfig.Database
    }
};
builder.Services.AddSingleton<IDistributedLockFactory>(sp =>
{
    return RedLockFactory.Create(redisConnections);
});

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.MapGet("/", () =>
{
    return "Demo Redis lock";
})
.WithOpenApi();

app.Run();
