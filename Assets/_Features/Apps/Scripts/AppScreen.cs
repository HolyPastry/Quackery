using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AppScreen : MonoBehaviour
{
    [SerializeField] private AppData _appData;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _animatable;
    [SerializeField] private Button _closeButton;
    public AppData AppData => _appData;

    public Transform Animatable => _animatable;

    IEnumerator Start()
    {
        yield return FlowServices.WaitUntilReady();

        AppServices.RegisterAppScreen(this);
    }
    void OnEnable()
    {
        _closeButton.onClick.AddListener(() =>
        {
            AppServices.CloseApp();
        });
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    void OnDestroy()
    {
        AppServices.UnregisterAppScreen(this);
    }

    public void Show()
    {
        _canvas.gameObject.SetActive(true);

    }

    public void Hide()
    {
        _canvas.gameObject.SetActive(false);
    }
}
