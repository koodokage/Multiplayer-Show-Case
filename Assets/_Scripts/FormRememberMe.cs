using UnityEngine;

public class FormRememberMe : MonoBehaviour
{
    [SerializeField] ASignView view;
    [SerializeField] string EDITOR_ID;
    [SerializeField] string EDITOR_PW;

    [SerializeField] string BUILD_ID;
    [SerializeField] string BUILD_PW;

    public void SignIn()
    {
#if UNITY_EDITOR
        view.AutoFill(EDITOR_ID, EDITOR_PW);
#else
        view.AutoFill(BUILD_ID,BUILD_PW);
#endif
    }
}
