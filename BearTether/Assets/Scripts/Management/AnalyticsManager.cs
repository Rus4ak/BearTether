using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private bool _isInitialized = false;

    public static AnalyticsManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }

    public void PressHardcore()
    {
        if (!_isInitialized)
            return;

        AnalyticsService.Instance.RecordEvent("press_hardcore");
        AnalyticsService.Instance.Flush();
    }

    public void PressBuyHardcore()
    {
        if (!_isInitialized)
            return;

        AnalyticsService.Instance.RecordEvent("press_buy_hardcore");
        AnalyticsService.Instance.Flush();
    }

    public void CompleteLevel(string mode, int completedLevel, int countStars, int countAttempts)
    {
        if (!_isInitialized)
            return;

        CustomEvent myEvent = new CustomEvent("completed_level")
        {
            {"game_mode", mode},
            {"level_index", completedLevel},
            {"count_stars", countStars},
            {"count_attempts", countAttempts}
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }
}
