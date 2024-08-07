#!/bin/bash
source ../cloudenvsetup.sh
ENCRYPT_PAYLOADS=$2 ./gradlew -q execute -PmainClass=io.temporal.samples.moneytransfer.TransferApprover -Parg=$1
