using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Shkodran_Hasani_Zgjedhjet_API.Data;
using Shkodran_Hasani_Zgjedhjet_API.Models.Elastic;
using Shkodran_Hasani_Zgjedhjet_API.Services;
using Shkodran_Hasani_Zgjedhjet_API.Services.Implementations;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IZgjedhjetService, ZgjedhjetService>();
builder.Services.AddScoped<ICsvImportService, CsvImportService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LifeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LifeDatabase")));

builder.Services.AddSingleton(sp =>
{
    var settings = new ElasticsearchClientSettings(
        new Uri("http://localhost:9200"))
        .DefaultIndex("zgjedhje-index");
    return new ElasticsearchClient(settings);
});

builder.Services.AddScoped<IElasticService<ZgjedhjeElasticDocument>>(sp =>
{
    var client = sp.GetRequiredService<ElasticsearchClient>();
    return new ElasticService<ZgjedhjeElasticDocument>(client, "zgjedhje-index");
});

var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<IRedisService, RedisService>();

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
