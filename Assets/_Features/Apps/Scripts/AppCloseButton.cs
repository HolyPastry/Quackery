using KBCore.Refs;
using Quackery;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AppCloseButton : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Button _button;
    [SerializeField, Parent] private App _app;

    void OnEnable()
    {
        // _button.gameObject.SetActive(true);
        _button.onClick.AddListener(OnClick);
    }
    void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        ///  _button.gameObject.SetActive(false);
        _app.Close();
    }
}
