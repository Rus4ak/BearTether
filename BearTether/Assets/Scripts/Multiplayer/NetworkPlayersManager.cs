using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerMultiplayer
{
    public GameObject player;
    public Rigidbody2D playerRb;
    
    public PlayerMultiplayer(GameObject player, Rigidbody2D playerRb)
    {
        this.player = player;
        this.playerRb = playerRb;
    }
}

public class NetworkPlayersManager : NetworkBehaviour
{
    [NonSerialized] public List<PlayerMultiplayer> players = new List<PlayerMultiplayer>();

    public static NetworkPlayersManager Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        SceneManager.activeSceneChanged += CheckScene;
    }

    private void CheckScene(Scene currentScene, Scene nextScene)
    {
        if (currentScene.name == "MainMenu")
            Destroy(gameObject);
    }   
}
