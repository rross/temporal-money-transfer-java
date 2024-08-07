# where TRANSFER-XXX-### is the workflowId
./gradlew -q execute -PmainClass=io.temporal.samples.moneytransfer.TransferApprover -Parg=$1
