public struct BaseStateTransitionData
{
    public IState FromThatStateActive;
    public IState DestinationState;
    public bool IsGlobal;

    public BaseStateTransitionData(IState fromThatStateActive, IState destinationState, bool isGlobal)
    {
        FromThatStateActive = fromThatStateActive;
        DestinationState = destinationState;
        IsGlobal = isGlobal;
    }
}
