using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ASignView : MonoBehaviour
{

    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected TMP_InputField InputField_ID;
    [SerializeField] protected TMP_InputField InputField_PW;
    [SerializeField] protected Button BTN_Action;

    public CanvasGroup GetCanvasGroup { get => canvasGroup;}

    protected virtual void Awake()
    {
        BTN_Action.onClick.AddListener(OnClick_Button);
    }

    public void AutoFill(string id,string pw)
    {
        InputField_ID.text = id;
        InputField_PW.text = pw;
        OnClick_Button();
    }


    private void  OnClick_Button()
    {
        string ID = InputField_ID.text;
        if(SignViewController.GetInstance.InputFieldCheck("EMAIL", ID,3,20) == false)
        {
            InputField_ID.text = string.Empty;
            return;
        }

        string PW = InputField_PW.text;
        if (SignViewController.GetInstance.InputFieldCheck("PASSWORD", PW, 6,12) == false)
        {
            InputField_PW.text = string.Empty;
            return;
        }

        OnButtonClicked();
    }

    protected abstract void OnButtonClicked();
}
