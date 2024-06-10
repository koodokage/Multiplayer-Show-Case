using Photon.Pun;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    public void AddResources(AEntityResource resource)
    {
        Player.GetInstance.SaveStatisticData(resource.GetStat, 1);
        resource.DeactivateResource();
    }

  


}


