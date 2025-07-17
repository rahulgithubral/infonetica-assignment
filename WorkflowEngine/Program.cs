using WorkflowEngine.Models;

var builder = WebApplication.CreateBuilder(args);
var app = WebApplication.Create();

// In-memory storage
var workflows = new Dictionary<string, WorkflowDefinition>();
var instances = new Dictionary<string, WorkflowInstance>();

// 1) Create Workflow Definition
app.MapPost("/workflows", (WorkflowDefinition definition) =>
{
    if (workflows.ContainsKey(definition.Id))
        return Results.BadRequest("Duplicate workflow ID.");

    if (definition.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Exactly one initial state required.");

    workflows[definition.Id] = definition;
    return Results.Ok(definition);
});

// 2) Get Workflow Definition
app.MapGet("/workflows/{id}", (string id) =>
{
    return workflows.ContainsKey(id)
        ? Results.Ok(workflows[id])
        : Results.NotFound("Workflow not found.");
});

// 3) Start Workflow Instance
app.MapPost("/instances", (string workflowId) =>
{
    if (!workflows.ContainsKey(workflowId))
        return Results.BadRequest("Workflow not found.");

    var wf = workflows[workflowId];
    var initialState = wf.States.First(s => s.IsInitial);

    var instance = new WorkflowInstance
    {
        DefinitionId = workflowId,
        CurrentState = initialState.Id
    };

    instances[instance.Id] = instance;
    return Results.Ok(instance);
});

// 4) Execute Action on Instance
app.MapPost("/instances/{id}/actions", (string id, string actionId) =>
{
    if (!instances.ContainsKey(id))
        return Results.BadRequest("Instance not found.");

    var instance = instances[id];
    var wf = workflows[instance.DefinitionId];

    var action = wf.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action == null || !action.Enabled)
        return Results.BadRequest("Invalid or disabled action.");

    if (!action.FromStates.Contains(instance.CurrentState))
        return Results.BadRequest("Action not allowed from current state.");

    var targetState = wf.States.FirstOrDefault(s => s.Id == action.ToState);
    if (targetState == null || !targetState.Enabled)
        return Results.BadRequest("Target state invalid or disabled.");

    if (wf.States.First(s => s.Id == instance.CurrentState).IsFinal)
        return Results.BadRequest("Cannot act on a final state.");

    instance.CurrentState = targetState.Id;
    instance.History.Add((action.Id, DateTime.UtcNow));

    return Results.Ok(instance);
});

// 5) Get Current State & History of Instance
app.MapGet("/instances/{id}", (string id) =>
{
    return instances.ContainsKey(id)
        ? Results.Ok(instances[id])
        : Results.NotFound("Instance not found.");
});

app.Run();
