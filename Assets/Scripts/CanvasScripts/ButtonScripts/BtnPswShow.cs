using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BtnPswShow : MonoBehaviour
{
    public TMP_InputField passwordField;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if (passwordField.contentType == TMP_InputField.ContentType.Password)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
        }

        passwordField.ForceLabelUpdate();
    }
}