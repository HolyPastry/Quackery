using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AppButton : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Button _button;
    [SerializeField] private AppData _appData;
    public AppData AppData => _appData;

    void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }
    void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        AppServices.OpenApp?.Invoke(_appData, transform.position);
    }
}
