#!/bin/bash
source ../cloudenvsetup.sh
ENCRYPT_PAYLOADS=$1 ./gradlew -q execute -PmainClass=io.temporal.samples.moneytransfer.web.WebServer --console=plain
echo "Navigate to http://localhost:7070/"
