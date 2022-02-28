using Microsoft.Extensions.Options;

using Play.Common;
using Play.Common.MongoDB;
using Play.Common.Settings;

using Play.Catalog.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IRepository<Item>>(service => 
{
    var options = service.GetService<IOptions<MongoDbSettings>>();
    if (options == null) throw new ArgumentNullException(nameof(options));
    return new MongoRepository<Item>(options, "Item");
});

builder.Services.AddControllers(options => 
{
    options.SuppressAsyncSuffixInActionNames = false;
});

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
