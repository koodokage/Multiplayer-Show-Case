using UnityEngine;
using UnityEngine.AI;

public class Pawn : MonoBehaviour
{
    const float ReachDistance = .05f;
    Rigidbody _body;
    NavMeshAgent _trackedAgent;
    Vector3 _targetDestination;
    Vector3 _direction;
    [SerializeField] Transform colliderOwner;
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float rotationSpeed = 10;
    public Renderer meshRenderer;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    /// <summary>
    /// Setup the local pawn 
    /// </summary>
    /// <param name="localPawnLayerLayer"></param>
    public void ActivateLocalPawn(int localPawnLayerLayer)
    {
        transform.name = "[LOCAL CLIENT] PAWN";
        gameObject.layer = localPawnLayerLayer;
        colliderOwner.gameObject.layer = localPawnLayerLayer;
    }




    /// <summary>
    ///  Set movement destination and initialize the navigation
    /// </summary>
    /// <param name="point"></param>
    public void MoveCommand(Vector3 point)
    {
        if (_trackedAgent == null)
        {
            _trackedAgent = NavigationManager.GetInstance.GetNavMeshAgent(transform.position);
        }

        _targetDestination = point;
        _direction = (_targetDestination - transform.position).normalized;
        _trackedAgent.transform.position = transform.position + _direction * .1f;
        _trackedAgent.gameObject.SetActive(true);
        _trackedAgent.SetDestination(_targetDestination);

        MovementInitialization();
    }

    private void MovementInitialization()
    {
        enabled = true;
        _body.constraints &= ~RigidbodyConstraints.FreezeRotationY;
    }

    private void DestinationReached()
    {
        NavigationManager.GetInstance.Release(_trackedAgent);
        _body.constraints = RigidbodyConstraints.FreezeRotation;
        _trackedAgent = null;
        _body.velocity = Vector3.zero;
        enabled = false;
    }

    private void Update()
    {
        if (_trackedAgent != null)
        {
            // Update target and check destination
            Vector3 targetPosition = _trackedAgent.transform.position;
            _direction = targetPosition - transform.position;

            float distance = (_targetDestination - transform.position).sqrMagnitude;
            if (distance <= ReachDistance)
            {
                DestinationReached();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_trackedAgent != null)
        {
            // Handle movement
            if (_direction.sqrMagnitude > ReachDistance)
            {
                _body.velocity = _direction.normalized * moveSpeed;
            }
            else
            {
                _body.velocity = Vector3.zero;
            }

            // Handle rotation
            if (_direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_direction);
                Quaternion smoothedRotation = Quaternion.Slerp(_body.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                // Restrict rotation to y-axis only
                Vector3 eulerRotation = smoothedRotation.eulerAngles;
                eulerRotation.x = 0;
                eulerRotation.z = 0;
                smoothedRotation = Quaternion.Euler(eulerRotation);

                _body.MoveRotation(smoothedRotation);
            }
        }
    }
}
