using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Temporalio.Worker;

using MoneyTransfer;

String getEnvVarWithDefault(String envName, String defaultValue) 
{
    String? value = Environment.GetEnvironmentVariable(envName);
    if (String.IsNullOrEmpty(value)) 
    {
        return defaultValue;
    }
    return value; 
}

var address = getEnvVarWithDefault("TEMPORAL_ADDRESS","127.0.0.1:7233");
var temporalNamespace = getEnvVarWithDefault("TEMPORAL_NAMESPACE","default");
var tlsCertPath = getEnvVarWithDefault("TEMPORAL_CERT_PATH","");
var tlsKeyPath = getEnvVarWithDefault("TEMPORAL_KEY_PATH","");
TlsOptions? tls = null;
if (!String.IsNullOrEmpty(tlsCertPath) && !String.IsNullOrEmpty(tlsKeyPath))
{
    Console.WriteLine("setting TLS");
    tls = new() {
        ClientCert = await File.ReadAllBytesAsync(tlsCertPath),
        ClientPrivateKey = await File.ReadAllBytesAsync(tlsKeyPath),
    };
}
Console.WriteLine($"Address is {address}");
var client = await TemporalClient.ConnectAsync(
    new(address)  
    { 
        Namespace = temporalNamespace,
        Tls = tls,
        LoggerFactory = LoggerFactory.Create(builder =>
        builder.
            AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
            SetMinimumLevel(LogLevel.Information)),
    });

using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
  tokenSource.Cancel();
  eventArgs.Cancel = true;    
};

var activities = new AccountTransferActivities();

using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions("MoneyTransferJava").
    AddAllActivities(activities).
    AddWorkflow<TransferWorkflow>());

// Run worker until cancelled
Console.WriteLine("Running worker...");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch( OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}


