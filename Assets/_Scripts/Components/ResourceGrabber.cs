using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ResourceGrabber : MonoBehaviour
{
    int _securityID;
    Transform _grabbed;
    [SerializeField] Transform grabPoint;
    [SerializeField] int resourceLayer;
    [SerializeField] int warehouseLayer;
    bool isUpdateActive;

    private void Awake()
    {
        isUpdateActive = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (isUpdateActive == false)
            return;

        _grabbed.transform.position = grabPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject detected = other.gameObject;
        if (_grabbed != null)
        {
            if (detected.layer == warehouseLayer)
            {
                if (detected.TryGetComponent(out Warehouse warehouse))
                {

                    if (_grabbed.TryGetComponent(out AEntityResource entityResource))
                    {
                        _grabbed.transform.SetParent(null);
                        isUpdateActive = false;
                        warehouse.AddResources(entityResource);
                        _grabbed = null;
                    }

                }


            }

            return;
        }

        if (detected.layer == resourceLayer)
        {
            detected.layer = 0;
            _grabbed = detected.transform;
            isUpdateActive = true;
            return;
        }
    }

}
