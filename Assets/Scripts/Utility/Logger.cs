using System;
using UnityEngine;
using CielaSpike;

public static class Logger
{
    public static void Log(System.Object obj, string s)
    {
        Debug.Log("["+obj.ToString()+"]" + s);
    }

    public static void LogAsync(string msg)
    {
        Debug.Log("[Async]" + msg);
    }

    public static void LogState(Task task)
    {
        Debug.Log("[State]" + task.State);
    }

    public static void LogSync(string msg)
    {
        Debug.Log("[Sync]" + msg);
    }
}