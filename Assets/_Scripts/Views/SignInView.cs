public class SignInView : ASignView
{
    protected override void OnButtonClicked()
    {
        SignViewController.GetInstance.SetCanvasGroupState(canvasGroup,true,false);
        AccountManager.LogIn(InputField_ID.text,InputField_PW.text,SignViewController.GetInstance.OnLogIn);
    }

}
