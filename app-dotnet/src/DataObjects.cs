

using System.Text.Json.Serialization;

namespace MoneyTransfer;

public enum ExecutionScenario
{
    HAPPY_PATH,
    ADVANCED_VISIBILITY,
    HUMAN_IN_LOOP,
    API_DOWNTIME,
    BUG_IN_WORKFLOW,
    INVALID_ACCOUNT
}

// public record WorkflowParameterObj(int amountCents, string scenario);
public record WorkflowParameterObj
{
    [JsonPropertyName("amount")]
    public required int AmountCents { get; init; }
    [JsonPropertyName("scenario")]
    public required string Scenario { get; init;}
    [JsonIgnore]
    public ExecutionScenario ExecutionScenario => Enum.Parse<ExecutionScenario>(Scenario);
}

// Keep the Java conventions to interoperate with the Java UI
#pragma warning disable IDE1006 // Naming Styles
public record ChargeResponse(string chargeId);

public record StateObject(int approvalTime, int progressPercentage, string transferState, string workflowStatus, ChargeResponse chargeResult);
#pragma warning restore IDE1006 // Naming Styles
