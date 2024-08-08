# money-transfer

Demonstrates a Money Transfer Demo for [Temporal](https://temporal.io) using the [Java](https://docs.temporal.io/develop/java) and [.NET](https://docs.temporal.io/develop/dotnet/) SDKs

Based on Steve Androulakis' Awesome [Money Transfer Demo](https://github.com/steveandroulakis/temporal-money-transfer-java)
this demo creates a .NET Worker Version that is compatible with the Java UI Web Application.

## Prerequisites
1. [Temporal CLI](https://docs.temporal.io/cli)
2. [Java SDK](https://openjdk.org/install/)
3. [.NET SDK](https://dotnet.microsoft.com/en-us/download)

## Run locally
To run everything on your own laptop, follow these instructions

### Start Temporal CLI
Start the temporal server locally:

```bash
temporal server start-dev
```

### Open up the Temporal UI
In a browser, navigate to [http://localhost:8233](http://localhost:8233)

### Start the Java Web UI
Open a new terminal window and run the following commands:

```bash
cd app-java
./startlocalwebui.sh
```
### Open the UI in another browser tab
In a browser, navigate to [http://localhost:7070/](http://localhost:7070/)

### Start the .NET Worker
Open a new terminal window and run the following commands:

```bash
cd app-dotnet/src
dotnet run
```

## Run On Temporal Cloud
To run everything on Temporal Cloud, follow these instructions

### Open a browser to Temporal Cloud
Open a browser and navigate to [Temporal Cloud](https://cloud.temporal.io). Go to your namespace and then workflows.

### Set up environment variables
In the root folder of this repository, edit the cloudenvsetup.sh to reflect the information needed for your account. 
It should look something like this:

```
#!/bin/bash
export TEMPORAL_ADDRESS=<namespace>.<accountId>.tmprl.cloud:7233
export TEMPORAL_NAMESPACE=<namespace>.<accountId>
export TEMPORAL_CERT_PATH="/path/to/cert"
export TEMPORAL_KEY_PATH="/path/to/key"
```

After you have made changes and saved it, you can continue. 

### Start the Java UI
```bash
cd app-java
./startcloudwebui.sh
```

Note that the startcloudwebui.sh file uses the environment variables in the cloudenvsetup.sh file. 

### Open the UI in another browser tab
In a browser, navigate to [http://localhost:7070/](http://localhost:7070/)

### Start the .NET Worker
Open a new terminal window and run the following commands:

```bash
cd app-dotnet/src
source ../cloudenvsetup.sh
dotnet run
```