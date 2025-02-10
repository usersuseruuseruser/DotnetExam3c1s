using System.Reflection;
using System.Text;
using api.AppLogic;
using api.DataAccess;
using api.HostedServices;
using api.Repositories;
using api.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.Configure<PostgresOptions>(builder.Configuration.GetSection("PostgresOptions"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<DbContextMigration>();
builder.Services.AddScoped<IUserRatingsRepository, UserRatingsRepository>();
builder.Services.AddScoped<IMongoDatabase>((sp) =>
{
    var connectionString = sp.GetRequiredService<IOptions<MongoOptions>>().Value.ConnectionString;
    
    var mongoClient = new MongoClient(connectionString);
    return mongoClient.GetDatabase("main");
});
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Key"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorizationBuilder();
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddDbContext<AppDbContext>((sp, c) =>
{
    var connectionString = sp.GetRequiredService<IOptions<PostgresOptions>>().Value.DefaultConnection;

    c.UseNpgsql(connectionString);
});
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();

