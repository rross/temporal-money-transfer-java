/*
 *  Copyright (c) 2020 Temporal Technologies, Inc. All Rights Reserved
 *
 *  Copyright 2012-2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 *  Modifications copyright (C) 2017 Uber Technologies, Inc.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"). You may not
 *  use this file except in compliance with the License. A copy of the License is
 *  located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 *  or in the "license" file accompanying this file. This file is distributed on
 *  an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 *  express or implied. See the License for the specific language governing
 *  permissions and limitations under the License.
 */

package io.temporal.samples.moneytransfer.dataclasses;

public class WorkflowStatusObj {
  private String workflowId;
  private String workflowStatus;
  private String url;

  // getters and setters
  public String getWorkflowId() {
    return workflowId;
  }

  public String getWorkflowStatus() {
    return workflowStatus;
  }

  public String getUrl() {
    return url;
  }

  public void setWorkflowId(String workflowId) {
    this.workflowId = workflowId;
  }

  public void setWorkflowStatus(String workflowStatus) {
    this.workflowStatus = workflowStatus;
  }

  public void setUrl(String url) {
    this.url = url;
  }
}
