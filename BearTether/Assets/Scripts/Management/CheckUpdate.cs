using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class CheckUpdate : MonoBehaviour
{
    [SerializeField] private GameObject _updateMenu;

    private AppUpdateManager _appUpdateManager;

    private void Start()
    {
        _appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }

    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                _updateMenu.SetActive(true);
            }
        }
    }

    public void StartUpdate()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.Luderus.BearTether");
    }
}
