using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class PhotonRoomSystem 
{
    private const string Tag = "PunCallback";
    private const string PrefabResourcePath = "Systems/PunCallback";


    public static void CreateOrJoin(string ArrangementString,int maxPlayers = 2)
    {
        RefreshCallbackReceiver();
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayers;
        PhotonNetwork.JoinOrCreateRoom(ArrangementString, options, TypedLobby.Default);
    }

    private static void RefreshCallbackReceiver()
    {
        var callbackObject = GameObject.FindGameObjectWithTag(Tag);
        if (callbackObject == null)
        {
            var prefab = Resources.Load(PrefabResourcePath);
            GameObject.Instantiate(prefab);
        }
    }

}
