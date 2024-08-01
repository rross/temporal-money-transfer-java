# money-transfer-dotnet

A money transfer demo worker written using the Temporal .NET SDK. 

## Run Worker Locally
```bash
dotnet run
```

## Run Worker using Temporal Cloud
```bash
# set up environment variables
export TEMPORAL_NAMESPACE=<namespace>.<accountId>
export TEMPORAL_ADDRESS=<namespace>.<accountId>.tmprl.cloud:7233
export TEMPORAL_TLS_CERT=/path/to/cert
export TEMPORAL_TLS_KEY=/path/to/key
# run the worker
dotnet run
```