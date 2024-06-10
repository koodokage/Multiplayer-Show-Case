using PlayFab;
using PlayFab.MultiplayerModels;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class MatchmakerSystem
{
    const int MillisecondsDelay_Polling = 6000;
    const string QueueName = "woodrace";
    static string TicketID;
    static string MatchID;
    static bool MatchPollState;

    public static string GetTicketID { get => TicketID; }
    public static string GetMatchID { get => MatchID; }
    public static string GetQueueName { get => QueueName; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Start()
    {
        TicketID = null;
        MatchID = null;
    }

    public static void CreateMatchTicket()
    {
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
           new CreateMatchmakingTicketRequest()
           {
               Creator = new MatchmakingPlayer
               {
                   Entity = AccountManager.GetEntityKey
                   ,
                   Attributes = new MatchmakingPlayerAttributes
                   {
                       DataObject = new
                       {
                           level = 2
                       },
                   },
               }
               ,
               GiveUpAfterSeconds = 120
               ,
               QueueName = "woodrace"
           }
           , resultCallback => { TicketID = resultCallback.TicketId; Logger.Log($"Create Ticket: {resultCallback.TicketId}"); MatchPollState = true; StartPollMatchStateAsync(); }
           , errorCallback => { Logger.Log($"Create Ticket: {errorCallback.Error} / {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails}", 2); }
           );


    }

    public static async void StartPollMatchStateAsync()
    {

        while (MatchPollState)
        {
            if (Application.isPlaying == false)
            {
                break;
            }

            await Task.Delay(MillisecondsDelay_Polling);

            if(MatchPollState == false)
            {
                break;
            }

            if (string.IsNullOrEmpty(MatchID))
            {
                PlayFabMultiplayerAPI.GetMatchmakingTicket(
                    new GetMatchmakingTicketRequest() { TicketId = TicketID, QueueName = "woodrace" }
                    , resultCallback => { MatchID = resultCallback.MatchId; Logger.Log($"{MatchID} - {resultCallback.Status} - {resultCallback.Members.Count}"); }
                    , errorCallback => { Logger.Log($"Get Ticket: {errorCallback.Error}/{errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}", 2); }
                    );
            }
            else
            {
                PlayFabMultiplayerAPI.GetMatch(
                    new GetMatchRequest() { MatchId = GetMatchID, QueueName = GetQueueName}
                    , resultCallback => { 
                        Logger.Log($"{MatchID} - LID: {resultCallback.ArrangementString}");
                        PhotonRoomSystem.CreateOrJoin(resultCallback.MatchId);
                        MatchPollState = false;
                    }
                    , errorCallback => {
                        Logger.Log($"Get Match: {errorCallback.Error}/{errorCallback.ErrorMessage}/{errorCallback.ErrorDetails}", 2); }
                    );
            }
        }


    }


    public static void CancelToken(Action OnCancelationSuccess = null)
    {
        MatchPollState = false;
        PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayer
        (new PlayFab.MultiplayerModels.CancelAllMatchmakingTicketsForPlayerRequest() { Entity = AccountManager.GetEntityKey, QueueName = MatchmakerSystem.GetQueueName }
        , resultCallback => { Logger.Log("CLEAN UP LIVE TICKETS", 0, 1);  MatchID = string.Empty; TicketID = string.Empty; OnCancelationSuccess?.Invoke();}
        , errorCallback => { Logger.Log($"ERROR: {errorCallback.ErrorMessage} / {errorCallback.ErrorDetails} PLEASE RESTART NETWORK CONNECTION", 2); }
        );
    }

}
