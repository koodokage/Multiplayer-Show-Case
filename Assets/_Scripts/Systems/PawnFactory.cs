using Photon.Pun;
using UnityEngine;

public class PawnFactory : AEntityFactory<GameObject, PlayerController>
{
    protected override void Awake()
    {
        base.Awake();
        prefabResourcePath = "NPawn";
        pool_Entity = new System.Collections.Generic.List<GameObject>(5);
    }

    public override GameObject Produce(PlayerController controller)
    {
        GameObject entity = null;
        if (pool_Entity.Count > 0)
        {
            entity = pool_Entity[0];
            entity.transform.position = controller.transform.position;
        }
        else
        {
            entity = PhotonNetwork.Instantiate(prefabResourcePath, controller.transform.position, Quaternion.identity);
        }

        entity.SetActive(true);

        return entity;
    }

    public override GameObject[] Produce(PlayerController controller, int amount)
    {
        GameObject[] entities = new GameObject[amount];
        GameObject entity = null;
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnLocation  = controller.transform.position + (controller.transform.right * i);
            if (pool_Entity.Count > 0)
            {
                entity = pool_Entity[0];
                entity.transform.position = spawnLocation;
            }
            else
            {
                entity = PhotonNetwork.Instantiate(prefabResourcePath, spawnLocation, Quaternion.identity);
            }

            entity.SetActive(true);
            entities[i] = entity;
        }

        return entities;
    }

    public override void Release(GameObject entity)
    {
        entity.SetActive(false);
        pool_Entity.Add(entity);
    }

}
