using SurveyDeliverySystem.Business.Services.Email;
using SurveyDeliverySystem.Business.Services.Survey;
using SurveyDeliverySystem.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SurveyDeliverySystem.Business.Validation;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SurveyDeliverySystem.Models;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.Configure<EmailSettingsConfig>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SendGridSettingsConfig>(builder.Configuration.GetSection("SendGridSettings"));


// Register FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SurveyEmailInfoValidator>());

// Alternatively, you can register validators individually
builder.Services.AddScoped<IValidator<SurveyEmailInfo>, SurveyEmailInfoValidator>();

// Register SendGridEmailSender
builder.Services.AddSingleton<IEmailSender, SendGridEmailSender>();


builder.Services.AddScoped<ISurveyService, SurveyService>();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Use URL segments for versioning
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
