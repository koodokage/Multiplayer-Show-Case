using Photon.Pun;
using UnityEngine;

public class PlayerFactory : AEntityFactory<GameObject, Player>
{
    protected override void Awake()
    {
        base.Awake();
        prefabResourcePath = "Player";
    }

    public override GameObject Produce(Player player)
    {
        return PhotonNetwork.Instantiate(prefabResourcePath, player.transform.position, Quaternion.identity);
    }
    public override GameObject[] Produce(Player player, int amount)
    {
        GameObject[] gameObjects = new GameObject[1];
        var networkInstance = PhotonNetwork.Instantiate(prefabResourcePath, player.transform.position, Quaternion.identity);
        gameObjects[0] = networkInstance;
        return gameObjects;
    }

    public override void Release(GameObject entity)
    {
        entity.SetActive(false);
    }

}
