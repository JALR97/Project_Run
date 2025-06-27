using System;
using UnityEngine;
using UnityEngine.Serialization;

//Subscribing to Events:
/*
    private void Awake() {
        GameManager.OnGameStateChange += StateChange;
    }

    private void OnDestroy() {
        GameManager.OnGameStateChange -= StateChange;
    }

    private void StateChange(GameManager.GameState newState) {
        //Do something depending on the newState
        ;
    }
*/

public class GameManager : MonoBehaviour {
    ///Singleton class to manage the game on a high level
    public static GameManager Instance;
    public GameState gameState;
    public PlayerState playerState;
    
    public static event Action<GameState> OnGameStateChange; 
    public enum GameState {
        MainMenu,
        Game,
        Paused,
        GameOver
    }
    
    public static event Action<PlayerState> OnPlayerStateChange; 
    public enum PlayerState {
        Inactive,
        Running,
        LookingAround,
        Autopilot
    }
    //**    ---Components---    **//


    //**    ---Variables---    **//
    //  [[ balance control ]] 
    
    //  [[ internal work ]] 
    
    //**    ---Properties---    **//
    

    //**    ---Functions---    **//
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SwitchGameState(GameState.MainMenu);
    }  
    
    public void SwitchGameState(GameState newState) {
        gameState = newState;
        switch (newState) {
            case GameState.MainMenu:
                break;
            case GameState.Game:
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
        }
        OnGameStateChange?.Invoke(newState);
    }

    public void SwitchPlayerState(PlayerState newState)
    {
        playerState = newState;
        switch (newState)
        {
            case PlayerState.Inactive:
                break;
            case PlayerState.LookingAround:
                break;
            case PlayerState.Running:
                break;
            case PlayerState.Autopilot:
                break;
        }

        OnPlayerStateChange?.Invoke(newState);
    }
}