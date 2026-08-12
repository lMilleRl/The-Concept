namespace TextBox
{
    public interface IProgressiveTarget
    {
        ProgressiveTargetId Id { get; }
        void SetProgress(float progress);
    }
}
