using Photon.Pun;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[CustomEditor(typeof(Player))]
public class PlayerHandler : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Player manager = (Player)target;
        if (GUILayout.Button("Save Wood Data"))
        {
            Player.GetInstance.SaveStatisticData(PlayerStats.WOODAMOUNT,5);
        }
    }
}
#endif

public sealed class Player : ASafeSingleBehaviour<Player>
{
    const string SpawnableSceneLabel = "Game";
    private const int LocalWarehouseLayer = 10;

    bool _spawnAuth;
    PlayerStatisticsData _playerData;
    public PlayerStatisticsData GetPlayerData { get => _playerData; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        // Security and Initialization
        _spawnAuth = false;
        _playerData = new PlayerStatisticsData();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private int GenerateSecureID()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            return BitConverter.ToInt32(randomBytes, 0);
        }
    }

    public void SpawnLocalPlayerController(Warehouse teamWarehouse,Color teamColor)
    {
        // spawn only in Game Scene
        if (_spawnAuth == false)
            return;

        transform.position = teamWarehouse.transform.position;
        GameObject controller = PlayerFactory.GetInstance.Produce(this);
        PlayerController connectedController = controller.GetComponent<PlayerController>();
        connectedController.CreatePawns(teamColor);
        teamWarehouse.gameObject.layer = LocalWarehouseLayer;
    }

   

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _spawnAuth = arg0.name == SpawnableSceneLabel;
        if (_spawnAuth)
        {
            // Security for Game scene : check other typeof<Player> class instance for fully security [securing the Player class will automatically secure all game process]
            CheckAndDestroyOtherInstances();
        }
    }

    public Task LoadStatisticData()
    {
        var tcs = new TaskCompletionSource<bool>();

        // WEB REQUEST
        CloudSaveSystem.LoadStatisticData((values) =>
        {
            try
            {
                OnDataLoaded(values);
                tcs.SetResult(true);
                //Debug.Log("TRY LOAD DATA SUCCESS");
            }
            catch (Exception ex)
            {
                tcs.SetException(ex); 
                Debug.LogError($"TRY LOAD DATA ERROR {ex.Message}");
            }
        });

        return tcs.Task; // Return the Task associated with the TaskCompletionSource
    }

    private void OnDataLoaded(List<StatisticValue> dictionary)
    {
        _playerData.SyncStatisticSaves(dictionary);
    }

    public void SaveStatisticData(PlayerStats stat, int amount)
    {
        Logger.Log("SAVING...",0,1);
        _playerData.Add(stat, amount);
        CloudSaveSystem.SaveStatisticData(_playerData);
    }


}
