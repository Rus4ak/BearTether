using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine;

public class PlayGamesManager : MonoBehaviour
{
    [SerializeField] private GameObject _connectButton;
    [SerializeField] private GameObject _connectedText;

    public static bool isAuthenticated = false;

    private void Awake()
    {
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            isAuthenticated = true;
            _connectButton.SetActive(false);
            _connectedText.SetActive(true);
            Progress.Instance.LoadFromCloud();
            Options.Instance.LoadFromCLoud();
        }
        else
        {
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

    public static byte[] LoadFromCloud(string filename)
    {
        byte[] loadedData = null;

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
                            loadedData = data;
                        }
                    });
                }
            });

        return loadedData;
    }
}
