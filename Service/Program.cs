using System.Text.Json.Serialization;
using Bootstrapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Service;
using Service.Adapters;
using Service.Authentication;
using Service.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<AppointmentDtoValidator>());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking Service", Version = "v1" });

    // Define the BasicAuth scheme
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Input your username and password to access this API"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            []
        }
    });
});

builder.Services.AddAuthorization().AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", _ => { });

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment()) await builder.Services.InitializeDb(builder.Configuration);

app.Run();