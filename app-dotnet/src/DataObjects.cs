namespace MoneyTransfer;

public enum ExecutionScenario
{
    HAPPY_PATH,
    ADVANCED_VISIBILITY,
    HUMAN_IN_LOOP,
    API_DOWNTIME,
    BUG_IN_WORKLOW,
    INVALID_ACCOUNT
}

public record WorkflowParameterObj
{
    public required int AmountCents { get; init; }
    public string Scenario { get; init;} = string.Empty;
    public ExecutionScenario ExecutionScenario
    { 
        get
        {
            return Enum.Parse<ExecutionScenario>(Scenario);
        }
    }
}

public record ChargeResponse(string chargeId);

public record StateObject(int approvalTime, int progressPercentage, string transferState, string workflowStatus, ChargeResponse chargeResult);
