using Amazon.S3;
using Amazon.SQS;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Http;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Log;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.Contracts.Interfaces.Mongo;
using VehicleRegistry.InfraStructure.AWS;
using VehicleRegistry.InfraStructure.Log;
using VehicleRegistry.InfraStructure.Mongo.Repository;
using VehicleRegistry.Manager;
using HttpClient = VehicleRegistry.InfraStructure.Http.HttpClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

/////////////////////////INFRASTRUCTURE////////////////////////////
builder.Services.AddHttpClient();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("ConnectionStrings:MongoDB")));

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("VehicleRegistry");
});

builder.Services.AddTransient<IAmazonSQSConnector, AmazonSQSConnector>();
builder.Services.AddTransient<IAmazonS3Connector, AmazonS3Connector>();
builder.Services.AddDefaultAWSOptions(new Amazon.Extensions.NETCore.Setup.AWSOptions());
builder.Services.AddSingleton<IHttpClient, HttpClient>();
builder.Services.AddSingleton<ILogSystemClient, LogSystemClient>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IVehiclesRepository, VehiclesRepository>();
builder.Services.AddScoped<IVehicleFilesRepository, VehicleFilesRepository>();

builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IVehiclesManager, VehiclesManager>();
builder.Services.AddScoped<IVehicleFilesManager, VehicleFilesManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
