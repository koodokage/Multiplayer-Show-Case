using TMPro;
using UnityEngine;

public class LoggerView : MonoBehaviour
{
    [SerializeField ]TextMeshProUGUI TMP_DisplayLog;
    [SerializeField ]CanvasGroup canvasGroup;

    public void ResetLog()
    {
        canvasGroup.alpha = 0;
        TMP_DisplayLog.text = string.Empty;
        TMP_DisplayLog.color = Color.white;
    }

    /// <summary>
    /// Display log in UI also invoke a callback
    /// </summary>
    /// <param name="playFabId"></param>
    /// <param name="logLevel"> 0 = Success, 1 = Warning, 2 = Error</param>
    internal void DisplayLog(string log = "", int logLevel = 0)
    {
        TMP_DisplayLog.text = log;
        switch (logLevel)
        {
            case 0:
                TMP_DisplayLog.color = Color.white;
                break;
            case 1:
                TMP_DisplayLog.color = Color.yellow;
                break;
            case 2:
                TMP_DisplayLog.color = Color.red;
                break;
        }

        canvasGroup.alpha = 1;
    }
}
