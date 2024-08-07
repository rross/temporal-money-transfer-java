#!/bin/bash
source ../cloudenvsetup.sh
ENCRYPT_PAYLOADS=$1 ./gradlew -q execute -PmainClass=io.temporal.samples.moneytransfer.AccountTransferWorker --console=plain

