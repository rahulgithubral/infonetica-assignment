namespace WorkflowEngine.Models;

public class WorkflowDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public List<State> States { get; set; } = new();
    public List<Action> Actions { get; set; } = new();
}
