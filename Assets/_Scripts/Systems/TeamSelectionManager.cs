using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(PhotonView))]
public class TeamSelectionManager : MonoBehaviour
{
    const int MAXPLAYERCOUNT = 2;
    int SELECTEDPLAYERCOUNT = 2;
    PhotonView _view;
    [SerializeField] Camera cameraComponent;
    [SerializeField] float cameraRotationSpeed = 5;

    [SerializeField] CanvasGroup teamPanel;
    [SerializeField] CanvasGroup resourcePanel;
    [Header("TEAM 1")]
    [SerializeField] Color ColorTeam1;
    [SerializeField] Button btn_Team1;
    [SerializeField] Warehouse Team1SpawnPoint;
    [Header("TEAM 2")]
    [SerializeField] Color ColorTeam2;
    [SerializeField] Button btn_Team2;
    [SerializeField] Warehouse Team2SpawnPoint;


    private void Awake()
    {
        SELECTEDPLAYERCOUNT = 0;
        _view = GetComponent<PhotonView>();

        ColorBlock colors = btn_Team1.colors;
        colors.normalColor = ColorTeam1;
        btn_Team1.colors = colors;
        btn_Team1.onClick.AddListener(() => OnSelectSide("1"));

        colors = btn_Team2.colors;
        colors.normalColor = ColorTeam2;
        btn_Team2.colors = colors;
        btn_Team2.onClick.AddListener(() => OnSelectSide("2"));
    }

    public void OnSelectSide(string side)
    {
        _view.RPC("RPC_SelectSide", RpcTarget.AllBuffered, side);
    }

    void Update()
    {
        float rotationAmount = cameraRotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);
    }

    [PunRPC]
    public void RPC_SelectSide(string side, PhotonMessageInfo info)
    {
        //Debug.Log($"[RPC] SELECT SIDE {side} [SENDER IS LOCAL {info.Sender.IsLocal}]");

        SELECTEDPLAYERCOUNT++;
        Logger.Log(SELECTEDPLAYERCOUNT.ToString(),0,2);

        Color color = Color.black;
        Warehouse warehouse = null;
        if (side == "1")
        {
            warehouse = Team1SpawnPoint;
            btn_Team1.interactable = false;
            color = ColorTeam1;
        }
        else if (side == "2")
        {
            warehouse = Team2SpawnPoint;
            btn_Team2.interactable = false;
            color = ColorTeam2;
        }

        
        // Initialize only local RPC sender things
        if (info.Sender.IsLocal)
        {
            Player.GetInstance.SpawnLocalPlayerController(warehouse,color);
            cameraComponent.gameObject.SetActive(false);
            teamPanel.interactable = false;
            enabled = false;
        }

        if(SELECTEDPLAYERCOUNT  >= MAXPLAYERCOUNT)
        {
            teamPanel.alpha = 0;
            teamPanel.interactable = false;
            resourcePanel.alpha = 1;
        }

    }


}
