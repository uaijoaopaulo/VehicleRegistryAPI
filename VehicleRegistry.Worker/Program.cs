using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using MongoDB.Driver;
using System.Net;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Manager;
using VehicleRegistry.InfraStructure.AWS;
using VehicleRegistry.InfraStructure.Mongo.Repository;
using VehicleRegistry.Manager;
using VehicleRegistry.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

/////////////////////////INFRASTRUCTURE////////////////////////////
builder.Services.AddHttpClient();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddDefaultAWSOptions(new AWSOptions());
builder.Services.AddTransient<IAmazonSQSConnector, AmazonSQSConnector>();
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));
builder.Services.AddTransient(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("VehicleRegistry");
});

//////////////////////////MANAGER/////////////////////////////
builder.Services.AddTransient<IVehicleFilesManager, VehicleFilesManager>();

/////////////////////////REPOSITORY////////////////////////////
builder.Services.AddTransient<IVehicleFilesRepository, VehicleFilesRepository>();

//////////////////////////WORKER/////////////////////////////
builder.Services.AddHostedService<FileUploadedWorkerService>();

var host = builder.Build();
host.Run();
