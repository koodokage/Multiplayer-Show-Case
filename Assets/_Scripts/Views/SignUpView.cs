using UnityEngine;

public class SignUpView : ASignView
{
    protected override void OnButtonClicked()
    {
        SignViewController.GetInstance.SetCanvasGroupState(canvasGroup, true, false);
        AccountManager.CreateAccount(InputField_ID.text,InputField_PW.text,SignViewController.GetInstance.OnLogIn);
    }
}
