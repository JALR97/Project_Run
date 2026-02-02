using System;
using Unity.VisualScripting;
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

    [SerializeField] private RectTransform needleUI, targetUI;
    
//-----------------//Variables//-----------------//
//Process variables - private
    //temporarily public for testing:
    public float _targetSpeed = 2f;
    public float _targetSpeedChangeRate = 0.5f;
    public float _acceleration = 0.3f;
    
    public float _maxSpeed = 3.5f;
    public float _minSpeed = 1.3f;
    //testing
    
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
        _realSpeed = _targetSpeed;
        SpeedometerTick();
        _volition = 100f;
        _stamina = 100f;
        
        /*
         Future implementation
        _temperature = 37f;
        _stability = 5;
        _muscleStrain = 0f; 
         */
    }

    private void Update() {
        //Debug.Log(_realSpeed);
        if (_running) {
            StaminaTick();
            VolitionTick();
            if (!Mathf.Approximately(_targetSpeed, _realSpeed)) {
                int upOrDown = (_targetSpeed - _realSpeed) >= 0f ? 1 : -1;
                _realSpeed += _acceleration * Time.deltaTime * upOrDown;
            }
            SpeedometerTick();
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
    
    private void SpeedometerTick() {
        float speedRatio = (_realSpeed - _minSpeed) / (_maxSpeed - _minSpeed);
        float targetRatio = (_targetSpeed - _minSpeed) / (_maxSpeed - _minSpeed);
        //Debug.Log($"ratio = {_realSpeed} - {_minSpeed} / {_maxSpeed} - {_minSpeed} == {speedRatio}");
        float rotationNeedle = speedRatio * (-160f) + 80f;
        float rotationTarget = targetRatio * (-160f) + 80f;
        
        needleUI.localEulerAngles = new Vector3(0f, 0f, rotationNeedle);
        targetUI.localEulerAngles = new Vector3(0f, 0f, rotationTarget);
    }
    
    //Inner process - private


//External interaction - public
    public void StartRun() {
        _running = true;
    }

    public void Accelerate(int intensity) {
        switch (intensity) {
            case 0: //Slow increase - hold
                _targetSpeed = Mathf.Clamp(_targetSpeed + _targetSpeedChangeRate * Time.deltaTime, _minSpeed, _maxSpeed);
                break;
            case 1: //Small jump - double tap
                
                break;
            case 2: //Big strides - continuous tapping
                
                break;           
        }
    }
    
    public void Decelerate(int intensity) {
        switch (intensity) {
            case 0: //Slow decrease - hold
                _targetSpeed = Mathf.Clamp(_targetSpeed - _targetSpeedChangeRate * Time.deltaTime, _minSpeed, _maxSpeed);
                break;
            case 1: //Small dip - double tap
                
                break;
            case 2: //Big strides - continuous tapping
                
                break;           
        }
    }
    
    
}
