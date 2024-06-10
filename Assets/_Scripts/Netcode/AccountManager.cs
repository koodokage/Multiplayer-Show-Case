using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;

public static class AccountManager
{
    static string playfab_ID;
    static string playfab_DisplayName;
    static EntityKey playfab_entityKey;

    public static EntityKey GetEntityKey { get => playfab_entityKey; }
    public static string GetPlayFabID { get => playfab_ID; }
    public static string GetPlayFabDisplayName { get => playfab_DisplayName; }

    public static void CreateAccount(string email, string pw, Action<bool> callback)
    {
        Logger.Clean();
        string[] splitPart = email.Split('@');
        playfab_DisplayName = splitPart[0];
        string userName = splitPart[0] + pw;
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest()
            {
                Email = email,
                Username = userName,
                Password = pw,
                DisplayName = playfab_DisplayName,
                RequireBothUsernameAndEmail = false
            },
          resultCallback =>
          {
              var entityId = resultCallback.EntityToken.Entity.Id;
              var entityType = resultCallback.EntityToken.Entity.Type;
              playfab_ID = resultCallback.PlayFabId;
              OnAccountLogIn(entityId, entityType, callback);
          },
            errorCallback =>
            {
                string err = $"{errorCallback.Error} (Retry with another mail and pw)";
                Logger.Log(err, 2);
                callback?.Invoke(false);
            });

    }

    public static void LogIn(string email, string pw, Action<bool> callback)
    {
        Logger.Clean();
        string[] splitPart = email.Split('@');
        string userName = splitPart[0] + pw;
        PlayFabClientAPI.LoginWithPlayFab(
            new LoginWithPlayFabRequest()
            {
                Username = userName,
                Password = pw,
            },
            resultLoginCallback =>
            {
                var entityId = resultLoginCallback.EntityToken.Entity.Id;
                var entityType = resultLoginCallback.EntityToken.Entity.Type;
                playfab_ID = resultLoginCallback.PlayFabId;
                // Get Safe Profile
                PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest() { PlayFabId = playfab_ID }
                , resultProfileCallback =>
                {
                    playfab_DisplayName = resultProfileCallback.PlayerProfile.DisplayName;
                    OnAccountLogIn(entityId, entityType, callback);
                }
                , errorProfileCallback =>
                {
                    string err = $" GET PROFILE : {errorProfileCallback.ErrorMessage}[{errorProfileCallback.ErrorDetails}] ({errorProfileCallback.Error})";
                    Logger.Log(err, 2);
                    callback?.Invoke(false);

                });
            },
            errorLoginCallback =>
            {
                string err = $"{errorLoginCallback.ErrorMessage}[{errorLoginCallback.ErrorDetails}] ({errorLoginCallback.Error})";
                Logger.Log(err, 2);
                callback?.Invoke(false);

            });
    }


    private static async void OnAccountLogIn(string EntityId, string EntityType, Action<bool> callback)
    {
        playfab_entityKey = new EntityKey() { Id = EntityId, Type = EntityType };
        await Player.GetInstance.LoadStatisticData();
        callback?.Invoke(true);
        Logger.Log($"SUCCESS {playfab_ID} / {playfab_DisplayName}");
    }





}
