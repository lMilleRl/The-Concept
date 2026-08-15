namespace TextBox
{
    public interface IDebugWriter
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
