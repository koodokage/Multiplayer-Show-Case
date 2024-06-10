using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CloudSaveSystem
{
    public static void SaveStatisticData(ACloudSaveableStatisticsData cloudSaveableData)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
        {
             Statistics = cloudSaveableData.GetStatisticSaves()
        }
        , resultCallback => { Logger.Log("CLOUD SAVE GAME SUCCESS",0,1); }
        , errorCallback => { Debug.LogError($"CLOUD SAVE {errorCallback.Error}/{errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}"); }
        );
    }

    public static void LoadStatisticData(Action<List<StatisticValue>> onCallbackFire)
    {
        List<StatisticValue> statistics = null;
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(){}
         , resultCallback =>
         {
             statistics = resultCallback.Statistics;
             onCallbackFire.Invoke(statistics);
             Logger.Log("CLOUD LOAD GAME SUCCESS", 0, 1);
         }
         , errorCallback =>
         {
             Debug.LogError($"CLOUD LOAD {errorCallback.Error}/{errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}");
             onCallbackFire.Invoke(statistics);
         }
        );
    }
}
