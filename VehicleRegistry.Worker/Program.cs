using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
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
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDefaultAWSOptions(new AWSOptions());
builder.Services.AddTransient<IAmazonSQSConnector, AmazonSQSConnector>();
builder.Services.AddTransient<IAmazonS3Connector, AmazonS3Connector>();
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(builder.Configuration.GetConnectionString("MongoDBConnection")));
builder.Services.AddTransient(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    var mongoUrl = new MongoUrl(connectionString);

    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoUrl.DatabaseName);
});

//////////////////////////MANAGER/////////////////////////////
builder.Services.AddTransient<IVehicleFilesManager, VehicleFilesManager>();

/////////////////////////REPOSITORY////////////////////////////
builder.Services.AddTransient<IVehicleFilesRepository, VehicleFilesRepository>();

//////////////////////////WORKER/////////////////////////////
builder.Services.AddHostedService<FileUploadedWorkerService>();

var host = builder.Build();
host.Run();
