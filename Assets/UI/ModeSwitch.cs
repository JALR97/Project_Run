using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModeSwitch : MonoBehaviour
{

    [SerializeField] private InputActionReference directionInputRef;
    [SerializeField] private PlayerController player;
    private InputAction _directionInput;

    private void Start() {
        _directionInput = directionInputRef.action;
    }

    private void Update() {
        var direction = _directionInput.ReadValue<Vector2>();
        
        if (_directionInput.WasPerformedThisFrame()) {
            //Switch mode depending on the direction
            if (direction.x != 0f) {
                //Control
                player.SwitchMode(PlayerController.RunningMode.Control);
            }else if (direction.y > 0f) {
                //Attention
                player.SwitchMode(PlayerController.RunningMode.Attention);
            }
            else {
                //Info
                player.SwitchMode(PlayerController.RunningMode.Information);
            }
            
            gameObject.SetActive(false);
        }
    }



    
}
