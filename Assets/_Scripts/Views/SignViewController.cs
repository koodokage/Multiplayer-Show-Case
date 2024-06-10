using System;
using UnityEngine;

public class SignViewController : ASingleBehaviour<SignViewController>
{
    [SerializeField] CanvasGroup signPanel;
    [SerializeField] ASignView signInPanel;

 

    [SerializeField] ASignView signUpPanel;
    [SerializeField] FormRememberMe rememberMe;
    [SerializeField] ServerConnector serverConnector;


    protected override void Awake()
    {
        base.Awake();
        serverConnector.OnMasterConnectionEtablished = OpenSignInPanel;
        SetCanvasGroupState(signPanel, false, false);
        SetCanvasGroupState(signInPanel.GetCanvasGroup, false, false);
        SetCanvasGroupState(signUpPanel.GetCanvasGroup, false, false);
    }

    public void OpenSignInPanel()
    {
        SetCanvasGroupState(signPanel, true, true);
        SetCanvasGroupState(signUpPanel.GetCanvasGroup, false, false);
        SetCanvasGroupState(signInPanel.GetCanvasGroup, true, true);
    }

    public async void OnLogIn(bool authState)
    {
        await System.Threading.Tasks.Task.Delay(550);
        if (authState)
        {
            serverConnector.LaunchLobby();
        }
        else
        {
            SetCanvasGroupState(signInPanel.GetCanvasGroup, true, true);
            SetCanvasGroupState(signUpPanel.GetCanvasGroup, false, false);
        }
    }

    public void OpenSignUpPanel()
    {
        SetCanvasGroupState(signPanel, true,true);
        SetCanvasGroupState(signInPanel.GetCanvasGroup, false,false);
        SetCanvasGroupState(signUpPanel.GetCanvasGroup, true,true);
    }

    public bool InputFieldCheck(string fieldLabel, string text, int minLength, int maxLenght)
    {
        if (string.IsNullOrEmpty(text))
        {
            string info = $"[{fieldLabel}] Cannot be Empty !";
            Logger.Log(info, 1);
            return false;
        }

        if (text.Length < minLength)
        {
            string info = $"[{fieldLabel}] Must be Greater Than {minLength} Char !";
            Logger.Log(info, 1);
            return false;
        }

        if (text.Length > maxLenght)
        {
            string info = $"[{fieldLabel}] Must be Less Than {maxLenght} Char !";
            Logger.Log(info, 1);
            return false;
        }
        return true;
    }

    internal void SetCanvasGroupState(CanvasGroup canvasGroup, bool stateVisibility,bool stateInteractable)
    {
        canvasGroup.alpha = System.Convert.ToInt32(stateVisibility);
        canvasGroup.interactable = stateInteractable;
        canvasGroup.blocksRaycasts = stateInteractable;
    }





}



