using Microsoft.Extensions.Logging;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace MoneyTransfer;

[Workflow("moneyTransferWorkflow")]
public class TransferWorkflow 
{
    private readonly static SearchAttributeKey<string> stepAttributKey = SearchAttributeKey.CreateKeyword("Step");

    ActivityOptions options = new () 
    {
        StartToCloseTimeout = TimeSpan.FromSeconds(5),
        RetryPolicy = new() {
            NonRetryableErrorTypes = [ nameof(InvalidAccountException), ],
        }
    };

    [WorkflowRun]
    public async Task<ChargeResponse> RunAsync(WorkflowParameterObj parameters)
    {
        Workflow.Logger.LogInformation($"Running workflow with scenario: {parameters.ExecutionScenario}");

        transferState = "starting";
        progressPercentage = 25;

        await Workflow.DelayAsync(TimeSpan.FromSeconds(ServerInfo.WorkflowSleepDuration));

        progressPercentage = 50;
        transferState = "running";

        // The validate activity will return false if approval is required
        var needsApproval = await Workflow.ExecuteActivityAsync(
            (AccountTransferActivities act) => act.Validate(parameters.ExecutionScenario), options);
        if (needsApproval)
        {
            Workflow.Logger.LogInformation(
                "\n\nWaiting on 'approveTransfer' Signal or Update for workflow ID: " +
                Workflow.Info.WorkflowId + "\n\n");
            
            transferState = "waiting";
            var received = await Workflow.WaitConditionAsync(() => approved, TimeSpan.FromSeconds(approvalTime));
            if (!received)
            {
                Workflow.Logger.LogInformation(
                    "Approval not received within the " + 
                    approvalTime + 
                    " -second time window: Failing the workflow.");

                throw new ApplicationFailureException("Approval not recieved wihin " + approvalTime + " seconds");
            }
        }

        // These variables are reflected in the UI
        progressPercentage = 60;
        transferState = "running";

        if (parameters.ExecutionScenario == ExecutionScenario.ADVANCED_VISIBILITY)
        {
            // TODO - Advanced Visiblity
            // Workflow.UpsertTypedSearchAttributes();
            var searchAttributes = Workflow.TypedSearchAttributes;
            Workflow.Logger.LogInformation($"Search attributes is {searchAttributes}");
            Workflow.Logger.LogInformation($"There are {searchAttributes.Count} search attributes.");
            var foundSearchAttribute = searchAttributes.ContainsKey("Step");
            Workflow.Logger.LogInformation($"Search attribute found? {foundSearchAttribute}");
            var untypedValues = searchAttributes.UntypedValues;
            Workflow.Logger.LogInformation($"Untyped Values has {untypedValues.Count} entities");
            //var foundUntypedSearchAttribute = untypedValues.ContainsKey("Step");
            //Workflow.Logger.LogInformation($"untyped contains value? {untypedValues.Count} values");
            Workflow.UpsertTypedSearchAttributes(stepAttributKey.ValueSet("Withdraw"));
            // Pause for dramatic effect
            await Workflow.DelayAsync(TimeSpan.FromSeconds(5));            
        }

        // Withdraw activity
        await Workflow.ExecuteActivityAsync((AccountTransferActivities act) => act.WithdrawAsync(parameters.AmountCents, parameters.ExecutionScenario), options);

        // Pause for dramatic effect
        await Workflow.DelayAsync(TimeSpan.FromSeconds(2));

        // Simulate bug in workflow
        if (parameters.ExecutionScenario == ExecutionScenario.BUG_IN_WORKFLOW)
        {
            // Throw an error to simulate a bug in the workflow
            // uncomment the following line and restart workers to fix the bug
            Workflow.Logger.LogInformation("\nSimulating workflow task failure.\n");
            throw new InvalidOperationException("Simulating workflow bug!");
        }

        if (parameters.ExecutionScenario == ExecutionScenario.ADVANCED_VISIBILITY)
        {
            Workflow.UpsertTypedSearchAttributes(stepAttributKey.ValueSet("Deposit")); 
        }

        try 
        {
            var idempotencyKey = Workflow.Random.Next().ToString();
            chargeResult = await Workflow.ExecuteActivityAsync(
                (AccountTransferActivities act) => 
                    act.Deposit(idempotencyKey, parameters.AmountCents, parameters.ExecutionScenario), options);
        }
        catch (ActivityFailureException exception)
        {
            Workflow.Logger.LogInformation("\n\nDeposit failed unrecoverably, reverting withdraw\n\n");
            // Undo activity (rollback)
            await Workflow.ExecuteActivityAsync(
                (AccountTransferActivities act) => 
                    act.UndoWithdraw(parameters.AmountCents), options);

            // Return failure message
            throw new ApplicationFailureException(exception.Message);
        }

        // These variables are reflected in the UI
        progressPercentage = 80;
        await Workflow.DelayAsync(TimeSpan.FromSeconds(6));
        progressPercentage = 100;
        transferState = "finished";
        
        return chargeResult;
    }

    [WorkflowSignal]
    public async Task ApproveTransfer()
    {
        Workflow.Logger.LogInformation("\nApprove Signal Received\n");
        if (transferState == "waiting")
        {
            approved = true;
        }
        else
        {
            Workflow.Logger.LogInformation($"\nSignal not applied: Transfer is not waiting for approval. {transferState}\n");
        }
    }
    
    [WorkflowQuery("transferStatus")]
    public StateObject TransferStatus
    {
        get
        {
            return new StateObject(approvalTime, progressPercentage, transferState, string.Empty, chargeResult);
        }
    }

    [WorkflowUpdate]
    public Task<string> ApproveTransferUpdate()
    {
        Workflow.Logger.LogInformation("\n\nApprove Update Validated: Approving Transfer\n\n");
        approved = true;
        return Task.FromResult("successfully approved transfer");
    }

    // [WorkflowUpdateValidator]
    // public void approveTransferUpdateValidator() 
    // {
    //     Workflow.Logger.LogInformation("\n\nApprove Update Validated: Approving Transfer\n\n");
    //     if (approved) 
    //     {
    //         throw new InvalidOperationException("Validation Failed: Transfer already approved");
    //     }
    //     if (transferState != "waiting")
    //     {
    //         throw new InvalidOperationException("Validation Failed: Transfer doesn't require approval");
    //     }
    // }

    // These variables are reflected in the UI
    private int progressPercentage = 10;
    private string transferState = "starting";
    // Time to allow for transfer approval
    private int approvalTime = 30;
    private bool approved = false;
    private ChargeResponse chargeResult = new(string.Empty);
}