using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Slider staminaBar, volitionBar;
    
//-----------------//Variables//-----------------//
//Process variables - private
    //temporarily public for testing:
    public float _targetSpeed = 2f;
    public float _targetSpeedChangeRate = 0.5f;
    public float _acceleration = 0.3f;
    
    public float _maxSpeed = 3.5f;
    public float _minSpeed = 1.3f;
    
    public float boostTimer;
    //testing
    
    private float _slope;
    
    private float _volition;
    private float _muscleStrain;
    private float _stamina;
    private float _temperature;
    private int _stability;

    private bool _running;
    private bool _boosting;
    private bool _canBoost = true;

//Balance variables - serialized 
    [SerializeField] private float volitionRegen = 2f;
    [SerializeField] private float boostCooldown = 7f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float boostConsumption = 20f;
    
    [SerializeField] private float coolingFactor = 0.2f;
    [SerializeField] private float staminaUseRate = 1f;
    [SerializeField] private float _slopeDirection;
    
    [SerializeField] private float UIbarTickTime = 0.2f;
    
    

    //Public properties - private set "Name { get; private set; }"
    public float _realSpeed { get; private set; }

//-----------------//Functions//-----------------//
//Built-in
    private void Start() {
        _realSpeed = _targetSpeed;
        SpeedometerTickUI();
        _volition = 100f;
        _stamina = 100f;
        
        staminaBar.maxValue = _stamina;
        volitionBar.maxValue = _volition;
        /*
         Future implementation
        _temperature = 37f;
        _stability = 5;
        _muscleStrain = 0f; 
         */
    }

    private void Update() {
        //Debug.Log(_realSpeed);
        if (_boosting) {
            _acceleration = 1f;
        }
        else {
            _acceleration = 0.4f;
        }
        
        if (_running) {
            StaminaTick();
            VolitionTick();
            if (!Mathf.Approximately(_targetSpeed, _realSpeed)) {
                int upOrDown = (_targetSpeed - _realSpeed) >= 0f ? 1 : -1;
                _realSpeed += _acceleration * Time.deltaTime * upOrDown;
            }
            UIUpdate();
            //Debug.Log($"slope: {_slope}, slopeDir: {_slopeDirection}");
        }
    }

    private void UIUpdate() {
        SpeedometerTickUI();
        StaminaUITick();
        VolitionUITick();
    }

    private void VolitionUITick() {
        volitionBar.value = _volition;
    }

    private void StaminaUITick() {
        staminaBar.value = _stamina;
    }

    private void StabilityTick() {
        
    }

    private void TemperatureTick() {
        _temperature -= Time.deltaTime * coolingFactor;
        _temperature = Mathf.Clamp(_temperature, 37.0f, 40.0f);
    }

    private void StaminaTick() {
        _stamina -= _realSpeed * staminaUseRate * Time.deltaTime;
    }

    private void StrainTick() {
        
    }

    private void VolitionTick() {
        _volition += Time.deltaTime * volitionRegen;
    }
    
    private void SpeedometerTickUI() {
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
    public void UpdateSlope(float angle, float direction) {
        _slope = angle;
        _slopeDirection = direction;
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

    public void Nitro() {
        if (!_canBoost) 
            return; 
        
        Debug.Log("Nitro");
        _boosting = true;
        _canBoost = false;
        float startVal = _volition;
        float endVal = _volition - boostConsumption;
        
        Animator.Instance.Animate(
            UIbarTickTime,
            t => { _volition = Mathf.Lerp(startVal, endVal, t); },
            Animator.EaseOutCubic);  
        
        Animator.Instance.Animate(
            boostDuration,
            t => { } ,null,
            () => { _boosting = false; Debug.Log("not boosting anymore");}); 
        
        Animator.Instance.Animate(
            boostCooldown,
            t => { } ,null,
            () => { _canBoost = true; Debug.Log("can boost again");}); 
        
    }
    
}
