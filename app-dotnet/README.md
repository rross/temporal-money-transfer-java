# money-transfer-dotnet

A money transfer demo worker written using the Temporal .NET SDK, which is compatable with the Java UI.


## Run the Java UI
```bash
cd ..\app-java
./gradlew -q execute -PmainClass=io.temporal.samples.moneytransfer.web.WebServer --console=plain
```


## Run Worker Locally
```bash
cd src
dotnet run
```

## Edit cloudenvsetup.bash for Temporal Cloud environment variables
```bash
cd ../..
# edit cloudenvsetup.sh
# change enviornment variables to match your configuration
export TEMPORAL_NAMESPACE=<namespace>.<accountId>
export TEMPORAL_ADDRESS=<namespace>.<accountId>.tmprl.cloud:7233
export TEMPORAL_TLS_CERT=/path/to/cert
export TEMPORAL_TLS_KEY=/path/to/key
# save the file
```

```bash
# run the worker 
source ../cloudenvsetup.sh
cd src
dotnet run
```

## Add Search Attribute to local for Advanced Visibility
```bash
temporal operator search-attribute create --name="Step" --type="Keyword"
```

## Add Search Attribute Temporal Cloud for Advanced Visibility 
```bash
# set your temporal environment
temporal env set dev.namespace <namespace>.<accountId>
temporal env set dev.address <namespace>.<accountId>.tmprl.cloud:7233
temporal env set dev.tls-cert-path /path/to/cert
temporal env set dev.tls-key-path /path/to/key 
# add the search attribute
temporal operator search-attribute create --name="Step" --type="Keyword" --env dev
```

