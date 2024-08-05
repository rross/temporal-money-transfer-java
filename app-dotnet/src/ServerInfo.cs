namespace MoneyTransfer;
public static class ServerInfo
{
    private static String getEnvVarWithDefault(String envName, String defaultValue) 
    {
        String? value = Environment.GetEnvironmentVariable(envName);
        if (String.IsNullOrEmpty(value)) 
        {
            return defaultValue;
        }
        return value; 
    }

    public static string WebServerURL
    {
        get 
        {
            return getEnvVarWithDefault("TEMPORAL_JAVA_WEB_SERVER_URL", "http://localhost:7070");
        }
    }

    public static string taskQueue
    {
        get 
        {
            return getEnvVarWithDefault("TEMPORAL_MONEYTRANSFER_TASKQUEUE", "MoneyTransferJava");
        }
    }

    public static int WorkflowSleepDuration
    {
        get
        {
            return Int32.Parse(getEnvVarWithDefault("TEMPORAL_MONEYTRANSFER_SLEEP","5"));
        }
    }
}