namespace Core.Tracing;

public enum OperationType
{
    EventPublish = 1,
    HandleEvent,
    ActionExecution,
    Other = 99
}