using System;
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
    private bool _appOpened;

    void OnEnable()
    {
        AppServices.OpenApp = OpenApp;
        AppServices.CloseApp = CloseApp;
        AppServices.RegisterAppScreen = RegisterAppScreen;
        AppServices.UnregisterAppScreen = UnregisterAppScreen;
        AppServices.IsAppSelected = (appData) => _currentApp != null && _currentApp.AppData == appData;
    }


    void OnDisable()
    {
        AppServices.OpenApp = delegate { };
        AppServices.CloseApp = delegate { };
        AppServices.RegisterAppScreen = delegate { };
        AppServices.UnregisterAppScreen = delegate { };
        AppServices.IsAppSelected = delegate { return false; };
    }

    private void RegisterAppScreen(AppScreen screen)
    {
        if (_loadedApps.ContainsKey(screen.AppData))
        {
            Debug.LogWarning($"AppScreen for {screen.AppData} is already registered.");
            return;
        }
        _loadedApps.Add(screen.AppData, screen);
    }

    private void UnregisterAppScreen(AppScreen screen)
    {
        _loadedApps.Remove(screen.AppData);
    }

    private void CloseApp()
    {
        if (_currentApp == null)
            return;

        _currentApp.Animatable.transform.DOScale(Vector3.zero, _closeAppDuration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _appOpened = false;
                _currentApp.Hide();
                _currentApp = null;
            });
    }

    private void OpenApp(AppData data, Vector2 position)
    {
        if (_appOpened)
            return;
        _appOpened = true;
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
            Debug.LogError($"AppScreen for {data} is not loaded.");
            yield break;
        }
        appScreen.Hide();
        appScreen.Animatable.localScale = Vector3.zero;
        appScreen.Animatable.position = position;
        appScreen.Show();
        appScreen.Animatable.DOScale(Vector3.one, _openAppDuration).SetEase(Ease.OutBack);
        appScreen.Animatable.DOLocalMove(Vector3.zero, _openAppDuration).SetEase(Ease.OutBack);

        _currentApp = appScreen;
    }
}
