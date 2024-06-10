using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerConnector : MonoBehaviourPunCallbacks
{
    public Action OnMasterConnectionEtablished;
    private void Start()
    {
        Application.targetFrameRate = 30;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        OnMasterConnectionEtablished();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SceneManager.LoadScene("Lobby");
    }

    public void LaunchLobby()
    {
        GetGameLaunchCount();
    }

    private async void JoinLobby(int launchCount )
    {
        Logger.Log($"LAUNCH COUNT: {launchCount}",4);
        await System.Threading.Tasks.Task.Delay(4500);
        PhotonNetwork.JoinLobby();
    }

    private void GetGameLaunchCount()
    {
    
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey { Id = AccountManager.GetEntityKey.Id,Type = AccountManager.GetEntityKey.Type},
            FunctionName = "Counter",
            GeneratePlayStreamEvent = false // PlayStream event generation

        }, resultCallback => 
        { 
            try
            {
                string launchCount_str = resultCallback.FunctionResult.ToString();
                int launchCount = Convert.ToInt32(launchCount_str);
                JoinLobby(launchCount);

            }
            catch (Exception ex)
            {
                Debug.LogError($" {ex.Message} @ {resultCallback.Error.Message}");
            }
        }
        , errorCallback => { Debug.LogError($" {errorCallback.ErrorMessage}[{errorCallback.ErrorDetails}] ({errorCallback.Error}"); });
    }

}
