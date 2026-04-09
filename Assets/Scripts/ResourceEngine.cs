using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResourceEngine : MonoBehaviour
{
//-----------------//Data structures//-----------------//
//enums
    public enum SlopesCat {
        SteepDown,
        LightDown,
        Flat,
        LightUp,
        SteepUp
    }
    public enum SpeedCategory {
        WALKING,
        JOGGING,
        RUNNING,
        SPRINTING
    }

//structs


//-----------------//Components//-----------------//
//Internal Components


//Prefabs


//External References
    [Header("External")]
    [SerializeField] private PlayerController playerController;

    [SerializeField] private RectTransform needleUI, targetUI;
    [SerializeField] private Slider staminaBar, volitionBar;
    
//-----------------//Variables//-----------------//
//Process variables - private
    
    //temporarily public for testing:
    public float _targetSpeed = 2f;
    public float _acceleration = 0.2f;
    public float _deceleration = -0.5f;

    public SlopesCat currentSlope;
    //testing

    
    private float _volition;
    private float _muscleStrain;
    private float _stamina;
    private float _temperature;
    private int _stability;

    private bool _running;
    private bool _boosting;
    private bool _canBoost = true;

//Balance variables - serialized 
    [Header("Boost")]
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float boostCooldown = 7f;
    [SerializeField] private float boostConsumption = 20f;
    [SerializeField] private float speedBoostFactor = 1.5f;
    
    [Header("Resources")]
    [SerializeField] private float staminaUseRate = 1f;
    [SerializeField] private float volitionRegen = 0.3f;
    [SerializeField] private float coolingFactor = 0.2f;
    
    [Header("Slope")]
    [SerializeField] private float slopeFlatThreshold = 5f;
    [SerializeField] private float SlopeSteepThreshold = 15f;
    public float _slope; //tempPublic
    public float _slopeDirection; //tempPublic
    
    [Header("Speed")]
    [SerializeField] private float _targetSpeedChangeRate = 0.5f;
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private float _minSpeed = 1.3f;
    
    [Header("UI")]
    [SerializeField] private float UIbarTickTime = 0.2f;
    [SerializeField] private float UIbarFlashSpeed = 0.1f;
    [SerializeField] private float UIbarFlashForce = 0.2f;
    
    [Header("Thresholds")]
    [SerializeField] private float joggingSpeedTH = 0.2f;
    [SerializeField] private float runningSpeedTH = 0.1f;
    [SerializeField] private float sprintingSpeedTH = 0.2f;
    private SpeedCategory _speedCategory = SpeedCategory.WALKING;
    
    //Public properties - private set "Name { get; private set; }"
    //public float _realSpeed { get; private set; }
    public float _realSpeed;

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
        DetermineSlope();
        //Debug.Log(currentSlope);
        Debug.Log(_speedCategory);
        
        if (_running) {
            DetermineSpeedCategory();
            StaminaTick();
            VolitionTick();
            if (!Mathf.Approximately(_targetSpeed, _realSpeed)) {
                _realSpeed += ((_targetSpeed - _realSpeed) >= 0f ? _acceleration : _deceleration) * Time.deltaTime;
            }

            if (_boosting) {
                _realSpeed = _targetSpeed;
            }
            UIUpdate();
            //Debug.Log($"slope: {_slope}, slopeDir: {_slopeDirection}");
        }
    }
    
    //Inner process - private

    private void DetermineSlope() {
        if (_slope < slopeFlatThreshold) 
            currentSlope = SlopesCat.Flat;
        else
            if (_slope > SlopeSteepThreshold) {
                currentSlope = _slopeDirection > 0 ? SlopesCat.SteepUp : SlopesCat.SteepDown;
            }
            else currentSlope = _slopeDirection > 0 ? SlopesCat.LightUp : SlopesCat.LightDown;
    }
    private void DetermineSpeedCategory() {
        if (_running) {
            if (_realSpeed <= joggingSpeedTH) {
                _speedCategory = SpeedCategory.WALKING;
            }else if (_realSpeed <= runningSpeedTH) {
                _speedCategory = SpeedCategory.JOGGING;
            }else if (_realSpeed <= sprintingSpeedTH) {
                _speedCategory = SpeedCategory.RUNNING;
            }else
                _speedCategory = SpeedCategory.SPRINTING;
        }
    }
    private void UIUpdate() {
        SpeedometerTickUI();
        StaminaUITick();
        VolitionUITick();
    }

    private void StaminaTick() {
        if (_boosting) 
            return;

        float adjustedRate = staminaUseRate;
        switch (_speedCategory) {
            case SpeedCategory.WALKING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        adjustedRate *= 2;
                        break;
                    case SlopesCat.LightDown:
                        adjustedRate *= 2;
                        break;
                    case SlopesCat.Flat:
                        adjustedRate *= 3;
                        break;
                    case SlopesCat.LightUp:
                        adjustedRate *= 0;
                        break;
                    case SlopesCat.SteepUp:
                        adjustedRate *= -1;
                        break;
                }
                break;
            case SpeedCategory.JOGGING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        adjustedRate *= 0;
                        break;
                    case SlopesCat.LightDown:
                        adjustedRate *= 1;
                        break;
                    case SlopesCat.Flat:
                        adjustedRate *= -1;
                        break;
                    case SlopesCat.LightUp:
                        adjustedRate *= -2;
                        break;
                    case SlopesCat.SteepUp:
                        adjustedRate *= -3;
                        break;
                }
                break;
            case SpeedCategory.RUNNING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        adjustedRate *= -1;
                        break;
                    case SlopesCat.LightDown:
                        adjustedRate *= -1;
                        break;
                    case SlopesCat.Flat:
                        adjustedRate *= -2;
                        break;
                    case SlopesCat.LightUp:
                        adjustedRate *= -2;
                        break;
                    case SlopesCat.SteepUp:
                        adjustedRate *= -3;
                        break;
                }
                break;
            case SpeedCategory.SPRINTING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        adjustedRate *= -2;
                        break;
                    case SlopesCat.LightDown:
                        adjustedRate *= -1;
                        break;
                    case SlopesCat.Flat:
                        adjustedRate *= -3;
                        break;
                    case SlopesCat.LightUp:
                        adjustedRate *= -3;
                        break;
                    case SlopesCat.SteepUp:
                        adjustedRate *= -4;//might be too much
                        break;
                }
                break;
        }
        //Do we still involve the actual speed number?
        //_stamina += _realSpeed * adjustedRate * Time.deltaTime;
        _stamina += adjustedRate * Time.deltaTime;
    }
    private void VolitionUITick() {
        volitionBar.value = _volition;
    }
    private void StabilityTick() {
        
    }
    private void TemperatureTick() {
        _temperature -= Time.deltaTime * coolingFactor;
        _temperature = Mathf.Clamp(_temperature, 37.0f, 40.0f);
    }
    private void StaminaUITick() {
        staminaBar.value = _stamina;
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
        Image staminaFill = staminaBar.transform.GetChild(1).GetChild(0).GetComponent<Image>(); //Could give problems later
        
        Animator.Instance.Animate(
            UIbarTickTime,
            t => { _volition = Mathf.Lerp(startVal, endVal, t); },
            Animator.EaseOutCubic);
        
        Color imgC = staminaFill.color; //All this for the bar flashing
        Color.RGBToHSV(imgC, out float h, out float s, out float v);
        Animator.Instance.Animate(
            boostDuration,
            t => {
                staminaFill.color = Color.HSVToRGB(h, s, v + Mathf.Cos(t * UIbarFlashSpeed) * UIbarFlashForce);
            } ,null,
            () => {
                _boosting = false; 
                staminaFill.color = imgC;
                Debug.Log("not boosting anymore");
            }); 
        
        Animator.Instance.Animate(
            boostCooldown,
            t => { } ,null,
            () => { _canBoost = true; Debug.Log("can boost again");}); 
        
    }
    
}
