using System;
using Unity.Cinemachine;
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
    
    public static event Action<GameState> OnGameStateChange; 
    public enum GameState {
        MainMenu,
        Game,
        Paused,
        GameOver
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
        //Debug:
        RenderSettings.fog = true;
        attentionFollow = attentionCam.GetComponent<CinemachineOrbitalFollow>();
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
    
    //Camera switching
    public enum Cameras {
        walking,
        rail,
        attention
    }
    [SerializeField] private GameObject walkingCam;
    [SerializeField] private GameObject railCam;
    [SerializeField] private GameObject attentionCam;
    private CinemachineOrbitalFollow attentionFollow;
    public void SwitchCam (Cameras camID){
        walkingCam.SetActive(false);
        railCam.SetActive(false);
        attentionCam.SetActive(false);
        
        switch (camID) {
            case Cameras.walking:
                walkingCam.SetActive(true);
                break;
            case Cameras.rail:
                railCam.SetActive(true);
                break;
            case Cameras.attention:
                attentionFollow.HorizontalAxis.Value = 0f;
                attentionFollow.VerticalAxis.Value = 17.5f;
                attentionCam.SetActive(true);
                break;
        }
    }
}