# Infonetica – Configurable Workflow Engine

This is a minimal state-machine API built in .NET 8 / C# as part of the Infonetica Software Engineer Intern assignment.



#  Features

- Define workflows (states + actions)
- Start workflow instances
- Execute actions to move between states with validation
- Inspect current state and history of instances
- In-memory storage (no database)

  #  Quick Start

#  Run the Project

```bash
dotnet run
 Example API Calls
    Create a Workflow

    powershell
    curl -Uri http://localhost:5151/workflows -Method POST -Body '{
    "name": "Leave Process",
    "states": [
        { "id": "draft", "name": "Draft", "isInitial": true, "isFinal": false, "enabled": true },
        { "id": "submitted", "name": "Submitted", "isInitial": false, "isFinal": false, "enabled": true },
        { "id": "approved", "name": "Approved", "isInitial": false, "isFinal": true, "enabled": true }
    ],
    "actions": [
        { "id": "submit", "name": "Submit", "enabled": true, "fromStates": ["draft"], "toState": "submitted" },
        { "id": "approve", "name": "Approve", "enabled": true, "fromStates": ["submitted"], "toState": "approved" }
    ]
    }' -ContentType "application/json"

    Start an Instance

    powershell
    curl -Uri "http://localhost:5151/instances?workflowId=<WORKFLOW-ID>" -Method POST

    Execute an Action

    powershell
    curl -Uri "http://localhost:5151/instances/<INSTANCE-ID>/actions?actionId=submit" -Method POST

    Check Current State

    powershell
    curl http://localhost:5151/instances/<INSTANCE-ID>



    Assumptions & Notes
    
    In-memory only (no persistence after restart)
    Exactly one initial state required per workflow
    Actions blocked if:
    Disabled
    Wrong source state
    Acting on final state

