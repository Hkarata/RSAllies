using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using RSAllies.Api.Data;
using RSAllies.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;


// Add services to the container.
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbConnection")));

builder.Logging.EnableRedaction();

builder.Services.AddRedaction(redact =>
{
    redact.SetRedactor<ErasingRedactor>(new DataClassificationSet(DataTaxonomy.SensitiveData));
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
});

builder.Services.AddCarter();

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

app.MapCarter();

app.Run();