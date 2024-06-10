using Photon.Pun;
using UnityEngine;

public class PunCallbackHandler : MonoBehaviourPunCallbacks
{
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"ROOM CREATION ERROR {message} / {returnCode}");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("Game");
    }
}
