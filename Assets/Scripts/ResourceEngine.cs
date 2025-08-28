using System;
using UnityEngine;

public class ResourceEngine : MonoBehaviour
{
//-----------------//Data structures//-----------------//
//enums


//structs


//-----------------//Components//-----------------//
//Internal Components


//Prefabs


//External References
    [SerializeField] private PlayerController playerController;
    
//-----------------//Variables//-----------------//
//Process variables - private
    private float _targetSpeed = 3f;
    
    private float _volition;
    private float _muscleStrain;
    private float _stamina;
    private float _temperature;
    private int _stability;

    private bool _running;

//Balance variables - serialized 
    [SerializeField] private float volitionRegen = 1f;
    [SerializeField] private float coolingFactor = 0.2f;

//Public properties - private set "Name { get; private set; }"
    public float _realSpeed { get; private set; }

//-----------------//Functions//-----------------//
//Built-in
    private void Start() {
        _realSpeed = 2f;
        _volition = 100f;
        _muscleStrain = 0f;
        _stamina = 100f;
        _temperature = 37f;
        _stability = 5;
    }

    private void Update() {
        if (_running) {
            TemperatureTick();
            StaminaTick();
            StrainTick();
            StabilityTick();
            VolitionTick();
        }
    }

    private void StabilityTick() {
        
    }

    private void TemperatureTick() {
        _temperature -= Time.deltaTime * coolingFactor;
        _temperature = Mathf.Clamp(_temperature, 37.0f, 40.0f);
    }

    private void StaminaTick() {
        
    }

    private void StrainTick() {
        
    }

    private void VolitionTick() {
        volitionRegen += Time.deltaTime * volitionRegen;
    }

    //Inner process - private


//External interaction - public
    public void StartRun() {
        _running = true;
    }
}
