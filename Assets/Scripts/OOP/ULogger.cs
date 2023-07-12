using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ULogger : MonoBehaviour, ILogger
{
    private UMain _ticker;

    private void Awake()
    {
        _ticker = FindObjectOfType<UMain>();
    }
    public void Log(int id, string message)
    {
        $"<id{id}>t{_ticker.Tick} {message}".Log();
    }

    public void Watch(int id, string key, string message)
    {
        // ConsoleProDebug.Watch($"<id{id}>:{key}", $"t{_ticker.Tick} {message}");
    }
}
