using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using TMPro;
using UnityEngine;

public class PlayGamesManager : MonoBehaviour
{
    [SerializeField] private GameObject _connectButton;
    [SerializeField] private GameObject _connectedText;
    [SerializeField] private GameObject _errorText;
    [SerializeField] private Transform _settingsCanvas;

    private bool _isInstantianErrorText = false;

    public static bool isAuthenticated = false;

    private void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void SignIn()
    {
        _isInstantianErrorText = true;
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            isAuthenticated = true;
            _connectButton.SetActive(false);
            _connectedText.SetActive(true);
            LoadFromCloud("progress");
            LoadFromCloud("options");
        }
        else
        {
            if (_isInstantianErrorText)
            {
                TextMeshProUGUI errorText = Instantiate(_errorText, _settingsCanvas).GetComponent<TextMeshProUGUI>();
                errorText.text = "Something wrong\nTry again";
            }
            _isInstantianErrorText = false;

            isAuthenticated = false;
            _connectedText.SetActive(false);
            _connectButton.SetActive(true);
        }
    }

    public static void SaveToCloud(byte[] data, string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                    savedGameClient.CommitUpdate(game, update, data, (status2, meta) =>
                    {
                        if (status2 == SavedGameRequestStatus.Success)
                        {
                            Debug.Log("data saved");
                        }
                    });
                }
            });
    }

    public static void LoadFromCloud(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    savedGameClient.ReadBinaryData(game, (status2, data) =>
                    {
                        if (status2 == SavedGameRequestStatus.Success)
                        {
                            Debug.Log("Data loaded");
                            if (filename == "progress")
                                Progress.Instance.DeserializeAndLoad(data);
                            else if (filename == "options")
                                Options.Instance.LoadFromCLoud(data);
                        }
                        else
                        {
                            Debug.LogError("Failed to read save: " + status2);
                        }
                    });
                }
                else
                {
                    Debug.LogError("Failed to open save for reading: " + status);
                }
            });
    }
}
