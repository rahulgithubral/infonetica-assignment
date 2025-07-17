namespace WorkflowEngine.Models;

public class Action
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public List<string> FromStates { get; set; } = new();
    public string ToState { get; set; }
}
