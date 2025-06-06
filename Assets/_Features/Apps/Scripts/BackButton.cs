using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Button _button;

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
        AppServices.CloseApp?.Invoke();
    }
}
