using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class AppManager : MonoBehaviour
{
    [SerializeField] private float _openAppDuration = 0.2f;
    [SerializeField] private float _closeAppDuration = 0.2f;
    private readonly Dictionary<AppData, AppScreen> _loadedApps = new();
    private AppScreen _currentApp = null;
    void OnEnable()
    {
        AppServices.OpenApp = OpenApp;
        AppServices.CloseApp = CloseApp;
    }

    void OnDisable()
    {
        AppServices.OpenApp = delegate { };
        AppServices.CloseApp = delegate { };
    }

    private void CloseApp()
    {
        if (_currentApp == null)
            return;

        _currentApp.transform.DOScale(Vector3.zero, _closeAppDuration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _currentApp.gameObject.SetActive(false);
                _currentApp = null;
            });
    }

    private void OpenApp(AppData data, Vector2 position)
    {
        StartCoroutine(OpenAppCoroutine(data, position));

    }

    private IEnumerator OpenAppCoroutine(AppData data, Vector2 position)
    {
        if (_currentApp != null)
        {
            if (_currentApp.AppData == data)
                yield break;
            CloseApp();
            yield return new WaitForSeconds(_closeAppDuration);
        }

        if (!_loadedApps.TryGetValue(data, out var appScreen))
        {
            appScreen = Instantiate(data.AppScreenPrefab, transform);
            _loadedApps.Add(data, appScreen);
        }
        appScreen.gameObject.SetActive(false);
        appScreen.transform.localScale = Vector3.zero;
        appScreen.transform.position = position;
        appScreen.gameObject.SetActive(true);
        appScreen.transform.DOScale(Vector3.one, _openAppDuration).SetEase(Ease.OutBack);
        appScreen.transform.DOLocalMove(Vector3.zero, _openAppDuration).SetEase(Ease.OutBack);

        _currentApp = appScreen;
    }
}
