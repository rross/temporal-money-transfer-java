using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using Temporalio.Exceptions;
using Xunit;

using MoneyTransfer;

namespace MoneyTransferTests;

public class MoneyTransferTests
{
    [Fact]
    public async Task TestWorkflowParmaters()
    {
        var pHappyPath = new WorkflowParameterObj() {
            AmountCents = 0, 
            Scenario = "HAPPY_PATH"
        };
        Assert.Equal(ExecutionScenario.HAPPY_PATH, pHappyPath.ExecutionScenario);

        var pAdvancedVis = new WorkflowParameterObj()
        {
            AmountCents = 0, 
            Scenario = "ADVANCED_VISIBILITY"
        };
        Assert.Equal(ExecutionScenario.ADVANCED_VISIBILITY, pAdvancedVis.ExecutionScenario);

        var pHuman = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = "HUMAN_IN_LOOP"
        };
        Assert.Equal(ExecutionScenario.HUMAN_IN_LOOP, pHuman.ExecutionScenario);

        var pApiDown = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = "API_DOWNTIME"
        };
        Assert.Equal(ExecutionScenario.API_DOWNTIME, pApiDown.ExecutionScenario);

        var pBug = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = "BUG_IN_WORKLOW"
        };
        Assert.Equal(ExecutionScenario.BUG_IN_WORKLOW, pBug.ExecutionScenario);

        var pInvalidAccount = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = "INVALID_ACCOUNT"
        };
        Assert.Equal(ExecutionScenario.INVALID_ACCOUNT, pInvalidAccount.ExecutionScenario);

        var pError = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = string.Empty
        };
        Assert.Throws<ArgumentException> (() => pError.ExecutionScenario);

        pError = new WorkflowParameterObj()
        {
            AmountCents = 0,
            Scenario = "not a valid scenario"
        };
        Assert.Throws<ArgumentException> (() => pError.ExecutionScenario);
    }

    [Fact]
    public async Task RunAsync_MoneyTransfer_HappyPath()
    {
         await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
         var clientOptions = (TemporalClientOptions)env.Client.Options.Clone();
         var client = new TemporalClient(env.Client.Connection, clientOptions);
         var taskQueue = Guid.NewGuid().ToString();
         var workerOptions = new TemporalWorkerOptions(taskQueue).
            AddAllActivities(new AccountTransferActivities()).
            AddWorkflow<TransferWorkflow>();

        using var worker = new TemporalWorker(client, workerOptions);
        await worker.ExecuteAsync(async () =>
        {
            var amountCents = 1000;
            var parameters = new WorkflowParameterObj() 
            {
                AmountCents = amountCents, 
                Scenario = "HAPPY_PATH"
            };
            var handle = await client.StartWorkflowAsync(
                (TransferWorkflow wf) => wf.RunAsync(parameters),
                new(
                    id: "HappyPathID",
                    taskQueue: taskQueue));

            // Wait for the workflow to complete
            var result = await handle.GetResultAsync();
            var expected = new ChargeResponse("example-charge-id");
            Assert.Equal(expected, result);
        });
    }

    [Fact]
    public async Task RunAsync_MoneyTransfer_HumanInLoop_Approved()
    {
         // await using var env = await WorkflowEnvironment.StartLocalAsync();
         await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
         var clientOptions = (TemporalClientOptions)env.Client.Options.Clone();
         var client = new TemporalClient(env.Client.Connection, clientOptions);
         var taskQueue = Guid.NewGuid().ToString();
         var workerOptions = new TemporalWorkerOptions(taskQueue).
            AddAllActivities(new AccountTransferActivities()).
            AddWorkflow<TransferWorkflow>();

        using var worker = new TemporalWorker(client, workerOptions);
        await worker.ExecuteAsync(async () =>
        {
            var amountCents = 1000;
            var parameters = new WorkflowParameterObj() 
            {
                AmountCents = amountCents, 
                Scenario = "HUMAN_IN_LOOP"
            };
            var handle = await client.StartWorkflowAsync(
                (TransferWorkflow wf) => wf.RunAsync(parameters),
                new(
                    id: "HumanInLoopID",
                    taskQueue: taskQueue));
            
            // Skip time so we're waiting for a signal
            Thread.Sleep(TimeSpan.FromSeconds(1));

            // signal the workflow
            await handle.SignalAsync(wf => wf.ApproveTransfer());
            
            // Wait for the workflow to complete
            var result = await handle.GetResultAsync();
            var expected = new ChargeResponse("example-charge-id");
            Assert.Equal(expected, result);
        });
    }

    [Fact]
    public async Task RunAsync_MoneyTransfer_HumanInLoop_NotApproved()
    {
         // await using var env = await WorkflowEnvironment.StartLocalAsync();
         await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
         var clientOptions = (TemporalClientOptions)env.Client.Options.Clone();
         var client = new TemporalClient(env.Client.Connection, clientOptions);
         var taskQueue = Guid.NewGuid().ToString();
         var workerOptions = new TemporalWorkerOptions(taskQueue).
            AddAllActivities(new AccountTransferActivities()).
            AddWorkflow<TransferWorkflow>();

        using var worker = new TemporalWorker(client, workerOptions);
        await worker.ExecuteAsync(async () =>
        {
            var amountCents = 1000;
            var parameters = new WorkflowParameterObj() 
            {
                AmountCents = amountCents, 
                Scenario = "HUMAN_IN_LOOP"
            };
            var handle = await client.StartWorkflowAsync(
                (TransferWorkflow wf) => wf.RunAsync(parameters),
                new(
                    id: "HumanInLoopID",
                    taskQueue: taskQueue));
        
            // Wait for the workflow to complete
            // will fail because it wasn't approved
            Assert.ThrowsAsync<ApplicationFailureException> (async () => await handle.GetResultAsync());
        });
    }
}