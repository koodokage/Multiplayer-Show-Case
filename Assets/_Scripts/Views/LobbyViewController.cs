using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyViewController : ASingleBehaviour<LobbyViewController>
{
    [SerializeField] Button BTN_CreateMatchTicket;
    [SerializeField] Button BTN_CancelMatchTicket;
    [SerializeField] TextMeshProUGUI TMP_UserName;

    private void Start()
    {
        BTN_CreateMatchTicket.interactable = false;
        MatchmakerSystem.CancelToken(OnCancelationSuccess);
        BTN_CreateMatchTicket.onClick.AddListener(OnPressed_CreateTicketButton);
        BTN_CancelMatchTicket.onClick.AddListener(OnPressed_CancelTicketButton);
        TMP_UserName.text = AccountManager.GetPlayFabDisplayName;
    }

    private void OnCancelationSuccess()
    {
        BTN_CreateMatchTicket.interactable = true;
    }

    private void OnPressed_CreateTicketButton()
    {
        BTN_CreateMatchTicket.interactable = false;
        MatchmakerSystem.CreateMatchTicket();
    }

    private void OnPressed_CancelTicketButton()
    {
        Logger.Clean();
        MatchmakerSystem.CancelToken(OnCancelationSuccess);
    }

}

