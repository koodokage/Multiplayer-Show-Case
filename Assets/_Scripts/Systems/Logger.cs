using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class Logger
{
    private const string Tag = "LoggerView";
    private const string PrefabResourcePath = "Systems/LoggerCanvas";
    static LoggerView _loggerView;
    static CancellationTokenSource _cancellationTokenSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static async void Start()
    {
        GameObject[] loggerViews = GameObject.FindGameObjectsWithTag(Tag);
        // Clean Up
        while (loggerViews.Length > 0)
        {
            await Task.Yield();
#if UNITY_EDITOR
            GameObject.DestroyImmediate(loggerViews[0]);
#else
            GameObject.Destroy(loggerViews[0]);
#endif
        }
    }

    /// <summary>
    ///  Display a log with spawning a canvas in scene
    /// </summary>
    /// <param name="log">display log</param>
    /// <param name="logLevel">0 = Success, 1 = Warning, 2 = Error</param>
    /// <param name="autoCleanInSecond">automatic clean up display after second</param>
    public static void Log(string log = "", int logLevel = 0, int autoCleanInSecond = -1)
    {
        // async operation checking
        if (Application.isPlaying == false)
            return;

        if (_loggerView == null)
        {
            GetLoggerView();
        }


        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }

        _loggerView.DisplayLog(log, logLevel);

        if (autoCleanInSecond > 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            AutoCleanUp(autoCleanInSecond, _cancellationTokenSource);
        }

    }

    public static async  void AutoCleanUp(int second, CancellationTokenSource cancellationToken)
    {
        float miliSec = Time.time;
        float timer = Time.time;
        while (timer - miliSec < second)
        {
            await Task.Yield();
            timer = Time.time;
            if (cancellationToken.IsCancellationRequested || Application.isPlaying == false)
            {
                return;
            }
        }

        Clean();
    }


    public static void Clean()
    {
        if (_loggerView != null)
        {
            _loggerView.ResetLog();
        }
    }


    private static void GetLoggerView()
    {
        var loggerCanvas = GameObject.FindGameObjectWithTag(Tag);
        if (loggerCanvas == null)
        {
            var prefab = Resources.Load(PrefabResourcePath);
            GameObject instance = GameObject.Instantiate(prefab) as GameObject;
            _loggerView = instance.GetComponent<LoggerView>();
        }
        else
        {
            _loggerView = loggerCanvas.GetComponent<LoggerView>();
        }
    }
}
