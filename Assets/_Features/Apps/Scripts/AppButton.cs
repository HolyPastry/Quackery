using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AppButton : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Button _button;
    [SerializeField, Child] private TextMeshProUGUI _label;
    [SerializeField] private AppData _appData;
    public AppData AppData => _appData;

    void Awake()
    {
        _label.text = _appData.MasterText;
    }

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
