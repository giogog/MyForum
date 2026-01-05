using API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Custom configurations
builder.Services.ConfigureCors();
builder.Services.ConfigureSqlServer(builder.Configuration);
builder.ConfigureJwtOptions();
builder.Services.GeneralConfiguration(builder.Configuration);
builder.Services.ConfigureAutomapper();
builder.Services.ConfigureRepositoryManager();
builder.Services.AddIdentityService(builder.Configuration);

builder.Services.ConfigureServiceManager();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddSwagger();

builder.Host.UseSerilog();

var app = builder.Build();
Log.Information("WebApi Starting Up");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
