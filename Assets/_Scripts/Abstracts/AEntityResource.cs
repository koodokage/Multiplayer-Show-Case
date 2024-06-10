using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class AEntityResource : MonoBehaviour
{
    protected PhotonView photonView;
    protected PlayerStats playerStats;
    public PlayerStats GetStat { get => playerStats;}

    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void RPC_DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }

    public void DeactivateResource()
    {
        photonView.RPC("RPC_DeactivateGameObject", RpcTarget.AllBuffered);
    }


}


