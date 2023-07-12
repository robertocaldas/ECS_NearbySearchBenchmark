public interface ILogger
{
    void Log(int id, string message);
    void Watch(int id, string key, string message);
}