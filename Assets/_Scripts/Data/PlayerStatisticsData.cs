using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatisticsData : ACloudSaveableStatisticsData
{
    public event Action OnDataChange;
    public PlayerStatisticsData()
    {
        statisticsMap = new Dictionary<PlayerStats, StatisticValue>();
    }

    public void Add(PlayerStats statisticName, int amount = 1)
    {
        StatisticValue statistic;
        if (statisticsMap.TryGetValue(statisticName, out statistic))
        {
            statistic.Value+= amount;
        }
        else
        {
            statistic = new StatisticValue { StatisticName = statisticName.ToString(), Value = amount };
            statisticsMap.Add(statisticName, statistic);
        }


        OnDataChange?.Invoke();
    }

    public bool TryGet(out int value, PlayerStats statisticName)
    {
        bool state = false;
        value = -1;
        StatisticValue statistic;
        if (statisticsMap.TryGetValue(statisticName, out statistic))
        {
            value = statistic.Value;
            state = true;
        }

        return state;
    }

    public override List<StatisticUpdate> GetStatisticSaves()
    {
        List<StatisticUpdate> result = new List<StatisticUpdate>();
        foreach (var key in statisticsMap.Keys)
        {
            StatisticValue stat = statisticsMap[key];
            result.Add(new StatisticUpdate() { StatisticName = stat.StatisticName, Value = stat.Value });
        }

        return result;
    }

    public override void SyncStatisticSaves(List<StatisticValue> statistics)
    {
        foreach (var stat in statistics)
        {
            if (Enum.TryParse(stat.StatisticName, out PlayerStats key))
            {
                if (statisticsMap.TryGetValue(key, out StatisticValue currentStatistic))
                {
                    currentStatistic.Value = stat.Value;
                }
                else
                {
                    statisticsMap.Add(key, stat);
                }
            }
            else
            {
                Debug.Log($"Invalid Statistic Key {stat.StatisticName}");
            }

        }
    }

}
