using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Common.MassTransit;
using Play.Catalog.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddMongo()
                .AddMongoRepository<Item>("Items")
                .AddMassTransitWithRabbit("Items");

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
    app.UseCors(b => 
    {
        b.WithOrigins(builder.Configuration["AllowedOrigin"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
