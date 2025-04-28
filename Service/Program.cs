using System.Text.Json.Serialization;
using Bootstrapper;
using FluentValidation.AspNetCore;
using Scalar.AspNetCore;
using Service;
using Service.Adapters;
using Service.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<AppointmentDtoValidator>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register components
builder.Services.AddSingleton<ScheduleAdapter>();
builder.Services.AddSingleton<ScheduleStartDateValidator>();
builder.Services.RegisterPersistence(builder.Configuration);
builder.Services.RegisterApplication();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Booking Service";
    options.Theme = ScalarTheme.Moon;
});

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment()) await builder.Services.InitializeDb(builder.Configuration);

app.Run();