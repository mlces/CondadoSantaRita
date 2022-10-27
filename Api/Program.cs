global using Api.Utilities;
global using Core.Entities;
global using Core.Models.Request;
global using Core.Models.Response;
global using Infrastructure;
using Api.Reports;
using Api.Tokens;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration.TokenIssuer,
        ValidAudience = Configuration.TokenAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.TokenSigningKey)),
        ClockSkew = TimeSpan.Zero
    }
);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

Configuration.ConfigureConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.ConnectionString));

builder.Services.AddTransient<TokenManager>();
builder.Services.AddTransient<PaymentReceiptsManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (Configuration.Swagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
