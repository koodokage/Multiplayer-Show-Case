using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Pawn _selectedPawn;
    PhotonView _photonView;
    [SerializeField] Color controllerTeamColor;
    [SerializeField] PhotonView view;
    [SerializeField] int groundLayer;
    [SerializeField] int pawnLayer;
    List<int> _pawnIdList;

    public Color GetTeamColor { get => controllerTeamColor;}

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // connect only the local
        if (view.IsMine == false)
        {
            return;
        }

    }

    public void CreatePawns(Color teamColor)
    {
        _pawnIdList = new List<int>();
        controllerTeamColor = teamColor;
        transform.name = "[LOCAL CLIENT]" + transform.name;
        PlayerCamera.GetInstance.ActivateCamera();

        GameObject[] pawns = PawnFactory.GetInstance.Produce(this, 3);
        foreach (var pawnInstance in pawns)
        {
            Pawn pawn = pawnInstance.GetComponent<Pawn>();
            pawn.ActivateLocalPawn(pawnLayer);
            _pawnIdList.Add(pawn.GetComponent<PhotonView>().ViewID);
        }

        ChangeMaterial(_pawnIdList.ToArray(),teamColor);
    }

    private void ChangeMaterial(int[]ids ,Color teamColor)
    {
        _photonView.RPC("RPC_SycnPawns", RpcTarget.AllBuffered, ids, teamColor.r, teamColor.g, teamColor.b, teamColor.a);
    }

    [PunRPC]
    public void RPC_SycnPawns(int[] pawnIds, float r, float g, float b, float a)
    {
        Color teamColor = new Color(r, g, b, a);
        var pawns = FindObjectsOfType<Pawn>();
        foreach (var pawn in pawns)
        {
            var view = pawn.GetComponent<PhotonView>();
            for (int i = 0; i < pawnIds.Length; i++)
            {
                if (pawnIds[i] == (view.ViewID))
                {
                    pawn.meshRenderer.material.color = teamColor;
                    break;
                }

            }
        }
    }

   

    private void Update()
    {
        // update only the local client
        if (view.IsMine == false)
        {
            return;
        }

        // Tap received
        if (Input.GetMouseButton(0))
        {
            if (PlayerCamera.GetInstance.LineTraceFromCameraView(out RaycastHit hit, Input.mousePosition))
            {
                Transform hitTransform = hit.transform;
                if (hitTransform.gameObject.layer == pawnLayer)
                {
                    hitTransform.TryGetComponent(out _selectedPawn);
                    return;
                }
            }

            _selectedPawn = null;
        }


        // Drag released
        if (Input.GetMouseButton(1))
        {
            if (PlayerCamera.GetInstance.LineTraceFromCameraView(out RaycastHit hit, Input.mousePosition))
            {
                Transform hitTransform = hit.transform;
                if (hitTransform.gameObject.layer == groundLayer)
                {
                    if (_selectedPawn != null)
                    {
                        _selectedPawn.MoveCommand(hit.point);
                    }
                }
            }
        }
    }

}
