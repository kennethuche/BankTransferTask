using BankTransferTask.Authorization;
using BankTransferTask.Core.Data;
using BankTransferTask.Core.Services;
using BankTransferTask.Core.Services.Bank;
using BankTransferTask.Core.Services.Paystack;
using BankTransferTask.Helpers;
using BankTransferTask.Middlewares;
using BankTransferTask.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.InitServices();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:3000");
                      });
});
builder.Services.Configure<PaystackConfig>(configuration.GetSection(nameof(PaystackConfig)));
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigurePaystackHttpClient(configuration);
builder.Services.AddScoped<IBankService, BankService>();
//builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IPaystackService, PaystackService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{ Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
        },
        new string[]{}
        }
    });

});
builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(ServicesBootstrapper).Assembly);
builder.Services.AddDbContextFactory<AppDbContext>(x =>
{
    x.UseInMemoryDatabase("BankDb");
}, ServiceLifetime.Scoped);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.AutoMigrateDatabase();
app.UseAuthorization();

app.MapControllers();
app.UseCors(myAllowSpecificOrigins);
app.Run();
