using UnityEngine;

public class PlayerCamera : ASingleBehaviour<PlayerCamera>
{
    private const int RayMaxDistance = 100;
    [SerializeField] Camera cameraComponent;
    [SerializeField] LayerMask clickableLayer;

    public void ActivateCamera()
    {
        cameraComponent.gameObject.SetActive(true);
    }

    public bool LineTraceFromCameraView(out RaycastHit hit,Vector3 pointerLocation)
    {
        Ray rayToScreen = cameraComponent.ScreenPointToRay(pointerLocation);
        return Physics.Raycast(rayToScreen, out hit, RayMaxDistance, clickableLayer,QueryTriggerInteraction.Collide);
    }



}
