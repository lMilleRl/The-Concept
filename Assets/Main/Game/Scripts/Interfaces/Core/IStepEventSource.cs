using System;

public interface IStepEventSource
{
    event Action<StepEvent> StepPerformed;
}
