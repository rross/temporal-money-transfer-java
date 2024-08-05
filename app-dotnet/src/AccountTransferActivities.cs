using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace MoneyTransfer;

public record EchoInput(String val);
public record EchoOutput(String result);

public class AccountTransferActivities
{
    [Activity]
    public bool Validate(ExecutionScenario scenario) 
    {
        ActivityExecutionContext.Current.Logger.LogInformation($"\nAPI /validate. Scenario {scenario}\n");

        return (scenario == ExecutionScenario.HUMAN_IN_LOOP);
    }
     
    [Activity]
    public async Task<string> WithdrawAsync(float amountDollars, ExecutionScenario scenario) 
    {
        ActivityExecutionContext.Current.Logger.LogInformation(
            $"\nAPI /withdraw amount = {amountDollars}");

        if (scenario == ExecutionScenario.API_DOWNTIME)
        {
            var info = ActivityExecutionContext.Current.Info;
            ActivityExecutionContext.Current.Logger.LogInformation(
                "\n**** Simulating API Downtime\n");
            if (info.Attempt < 5)
            {
                ActivityExecutionContext.Current.Logger.LogInformation(
                    "\n*** Activity Attempt: # " + info.Attempt + "***\n");
                var delaySeconds = 7;
                ActivityExecutionContext.Current.Logger.LogInformation(
                    "\n/API/simulateDelay Seconds" + delaySeconds + "\n");
                var response = await SimulateDelay(delaySeconds);
            }
        }
        
        return "SUCCESS";
    }    

    [Activity]
    public ChargeResponse Deposit(String idempotencyKey, float amountDollars, ExecutionScenario scenario)  
    {
        ActivityExecutionContext.Current.Logger.LogInformation($"\nAPI /deposit amount = {amountDollars}");

        if (scenario == ExecutionScenario.INVALID_ACCOUNT)
        {
            throw new InvalidAccountException("Invalid Account");
        }

        return new ChargeResponse("example-charge-id");
    }

    [Activity]    
    public bool UndoWithdraw(float amountDollars)
    {
        ActivityExecutionContext.Current.Logger.LogInformation(
            $"\nAPI /undoWithdraw amount = {amountDollars}");

        return true;
    }

    private static async Task<string> SimulateDelay(int seconds) 
    {        
        var url = ServerInfo.WebServerURL;
        var urlPath ="/simulateDelay?s=" + seconds;
        ActivityExecutionContext.Current.Logger.LogInformation(            
            $"\n/API/simulateDelay URL: {url} path: {urlPath}");

        using var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url),
        };
        var response = await httpClient.GetStringAsync(urlPath);
        return response;
    } 
}
