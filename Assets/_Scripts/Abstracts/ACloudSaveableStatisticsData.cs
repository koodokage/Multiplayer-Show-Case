using PlayFab.ClientModels;
using System.Collections.Generic;

public enum PlayerStats { WOODAMOUNT }

public abstract class ACloudSaveableStatisticsData
{
    protected Dictionary<PlayerStats, StatisticValue> statisticsMap;

    public abstract List<StatisticUpdate> GetStatisticSaves();
    public abstract void SyncStatisticSaves(List<StatisticValue> statistics);
}
