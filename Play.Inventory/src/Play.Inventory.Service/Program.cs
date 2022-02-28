using Polly;
using Polly.Timeout;

using Microsoft.Extensions.Options;

using Play.Common;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IRepository<InventoryItem>>(service => 
{
    var options = service.GetService<IOptions<MongoDbSettings>>();
    if (options == null) throw new ArgumentNullException(nameof(options));
    return new MongoRepository<InventoryItem>(options, "InventoryItem");
});

Random jitterer = new Random();
builder.Services.AddHttpClient<CatalogClient>(client => 
{
    client.BaseAddress = new Uri("https://localhost:7109");
})
.AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        5, 
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
        onRetry: (outcome, timespan, retryAttempt) => 
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>() ?
                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
            }
    )
)
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
