using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using VehicleRegistry.Contracts.InfraStructure.Validators;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Database;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.InfraStructure.AWS;
using VehicleRegistry.InfraStructure.Database;
using VehicleRegistry.InfraStructure.Database.Repository;
using VehicleRegistry.InfraStructure.Mongo.Repository;
using VehicleRegistry.InfraStructure.Validators;
using VehicleRegistry.Manager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

/////////////////////////INFRASTRUCTURE////////////////////////////
builder.Services.AddHttpClient();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDefaultAWSOptions(new AWSOptions());
builder.Services.AddTransient<IAmazonS3Connector, AmazonS3Connector>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"), b =>
    {
        b.MigrationsAssembly("VehicleRegistry.InfraStructure");
    });
});
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(builder.Configuration.GetConnectionString("MongoDBConnection")));
builder.Services.AddScoped(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    var mongoUrl = new MongoUrl(connectionString);

    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoUrl.DatabaseName);
});
builder.Services.AddSingleton<ILicensePlateValidator, LicensePlateValidator>(); 
builder.Services.AddSingleton<IFileExtensionValidator, FileExtensionValidator>();

/////////////////////////REPOSITORY////////////////////////////
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IVehiclesRepository, VehiclesRepository>();
builder.Services.AddScoped<IVehicleFilesRepository, VehicleFilesRepository>();

/////////////////////////MANAGER////////////////////////////
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IVehiclesManager, VehiclesManager>();
builder.Services.AddScoped<IVehicleFilesManager, VehicleFilesManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VehicleRegistry", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token in the field below. Example: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var configuration = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }