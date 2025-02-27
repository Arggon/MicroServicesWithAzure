using Microsoft.EntityFrameworkCore;
using Polly;
using Wpm.Clinic.Api.Application;
using Wpm.Clinic.Api.DataAccess;
using Wpm.Clinic.Api.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ManagementService>();
builder.Services.AddScoped<ClinicApplicationService>();
builder.Services.AddDbContext<ClinicDbContext>( options => options.UseInMemoryDatabase("ClinicDb"));
builder.Services.AddHttpClient<ManagementService>(client =>
{
    var uri = builder.Configuration.GetValue<string>("Wpm:ManagementUri");
    client.BaseAddress = new Uri(uri);
}).AddResilienceHandler("management-pipeline", builder =>
{
    builder.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>()
    {
        BackoffType = DelayBackoffType.Exponential,
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1)
    });
});

var app = builder.Build();

app.EnsureClinicDbIsCreated();

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