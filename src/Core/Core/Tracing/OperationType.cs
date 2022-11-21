namespace Core.Tracing;

public enum OperationType
{
    EventPublish = 1,
    HandledEvent,
    ActionExecution,
    Other = 99
}