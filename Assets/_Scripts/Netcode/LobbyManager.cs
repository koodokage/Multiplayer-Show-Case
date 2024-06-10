using Photon.Pun;
using PlayFab;
using PlayFab.MultiplayerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(LobbyManager))]
public class LobbyManagerHandler : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LobbyManager manager = (LobbyManager)target;
        if (GUILayout.Button("DeleteAllLobby"))
        {
            manager.DeleteAll();
        }

        if (GUILayout.Button("DeleteAllLobby"))
        {
            manager.JoinLobby(manager.testlobbyID);
        }
    }
}

#endif

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public int testlobbyID = 0;
    List<LobbySummary> lobbies;
    string connectionID = string.Empty;
    string lobbyID = string.Empty;
    bool isJoined;

    IEnumerator Start()
    {
        isJoined = false;
        lobbies = new List<LobbySummary>();

        WaitForSeconds forSeconds = new WaitForSeconds(5);

        while (isJoined == false)
        {
            PullLobbies();
            yield return forSeconds;
        }
    }


    private void PullLobbies()
    {
        lobbies.Clear();
        PlayFabMultiplayerAPI.FindLobbies(new FindLobbiesRequest(),
            resultCallback => { lobbies = resultCallback.Lobbies; PrintLobbies(); },
            errorCallback => { Debug.LogError($"LOBBY PULL FAILED {errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}"); });
    }

    public void CreateLobby()
    {
        PlayFabMultiplayerAPI.CreateLobby(
        new CreateLobbyRequest()
        {
            Owner = AccountManager.GetEntityKey,
            OwnerMigrationPolicy = OwnerMigrationPolicy.None,
            AccessPolicy = AccessPolicy.Public,
            MaxPlayers = 2,
            UseConnections = false,
            SearchData = new Dictionary<string, string> { { "string_key1", "WOOD RACE" } },
            Members = new List<Member>
            {
                { new Member()
                        { MemberEntity = AccountManager.GetEntityKey
                          , MemberData = new Dictionary<string, string>{ {"NAME",AccountManager.GetPlayFabDisplayName} }
                        }
                }
            }

        }
        , resultCallback =>
         {
             connectionID = resultCallback.ConnectionString;
             lobbyID = resultCallback.LobbyId;
             Logger.Log($"LOBBY CREATED ID: {lobbyID} /  CONNID: {connectionID}");
         }
        , errorCallback =>
         {
             Debug.LogError($" { errorCallback.Error} { errorCallback.ErrorMessage} / { errorCallback.ErrorDetails} { errorCallback.GenerateErrorReport()}");
             Logger.Log($"LOBBY CREATION FAILED {errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}/ \n{errorCallback.GenerateErrorReport()}", 2);
         });

    }

    private void LeaveLobby()
    {
        if (string.IsNullOrEmpty(lobbyID))
            return;

        PlayFabMultiplayerAPI.LeaveLobby(
                new LeaveLobbyRequest() { LobbyId = lobbyID }
                , resultCallback => { }
                , errorCallback => { }
            );
    }

    private void DeleteLobby(string id)
    {
        PlayFabMultiplayerAPI.DeleteLobby(
                new DeleteLobbyRequest() { LobbyId = id }
                , resultCallback => { Debug.Log($"DELETED {id}"); }
                , errorCallback => { Debug.LogError($"ERROR ON DELETE LOBBY {errorCallback.ErrorMessage}/{id}"); }
            );
    }

    public void JoinLobby()
    {
        var lobby = lobbies[0];
        Debug.LogWarning($" TRY JOIN : [{lobby.ConnectionString}],[{lobby.Owner.Id}] [{lobby.CurrentPlayers}] LID [{lobby.LobbyId}] ");
        PlayFabMultiplayerAPI.JoinLobby(
            new JoinLobbyRequest() { ConnectionString = lobby.ConnectionString }
            , resultCallback =>
            {
                isJoined = true;
                PlayFabMultiplayerAPI.GetLobby(
                          new GetLobbyRequest() { LobbyId = lobby.LobbyId }
                          , resultCallback =>
                          {
                              var LIST = resultCallback.Lobby.Members;
                              foreach (var p in LIST)
                              {
                                  Debug.Log(p.MemberData["NAME"]);
                              }
                          }
                          , errorCallback =>
                          {
                              Debug.LogError($"LOBBY NOT FOUND {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails} / {errorCallback.Error}");
                          });
            }
            , errorCallback => { Debug.LogError($"LOBBY JOIN FAIL {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails} / [{errorCallback.Error}]"); }
            );
    }

    public void JoinLobby(int id)
    {
        var lobby = lobbies[id];
        Debug.LogWarning($" TRY JOIN : [{lobby.ConnectionString}],[{lobby.Owner.Id}] [{lobby.CurrentPlayers}] LID [{lobby.LobbyId}] ");
        PlayFabMultiplayerAPI.JoinLobby(
            new JoinLobbyRequest() { ConnectionString = lobby.ConnectionString, MemberEntity = AccountManager.GetEntityKey, MemberData = new Dictionary<string, string> { { "NAME", AccountManager.GetPlayFabDisplayName } } }
            , resultCallback =>
            {
                isJoined = true;
                PlayFabMultiplayerAPI.GetLobby(
                          new GetLobbyRequest() { LobbyId = lobby.LobbyId }
                          , resultCallback =>
                          {
                              var LIST = resultCallback.Lobby.Members;
                              foreach (var p in LIST)
                              {
                                  Debug.Log(p.MemberData["NAME"]);
                              }
                          }
                          , errorCallback =>
                          {
                              Debug.LogError($"LOBBY NOT FOUND {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails} / {errorCallback.Error}");
                          });
            }
            , errorCallback => { Debug.LogError($"LOBBY JOIN FAIL {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails} / [{errorCallback.Error}]"); }
            );
    }

    private void PrintLobbies()
    {
        Debug.Log($"LOBBY COUNT: [{lobbies.Count}]");
        foreach (var lobby in lobbies)
        {
            Debug.Log($"[{lobby.ConnectionString}],[{lobby.Owner.Id}] [{lobby.CurrentPlayers}] LID [{lobby.LobbyId}] ");
            if (lobby.SearchData != null)
            {
                Debug.LogWarning($"LOBBY NAME : {lobby.SearchData["string_key1"]}");
            }
        }

    }


    internal void DeleteAll()
    {
        for (int i = 0; i < lobbies.Count; i++)
        {
            DeleteLobby(lobbies[i].LobbyId);
        }
    }

    private void OnApplicationQuit()
    {
        LeaveLobby();
    }
}
