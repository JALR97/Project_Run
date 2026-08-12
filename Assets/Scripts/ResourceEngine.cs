using System;
using Unity.Cinemachine;
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
    private float multiplier = 1;
    
    //temporarily public for testing:
    private float _targetSpeed = 2f;
    private float _acceleration = 0.2f;
    [SerializeField] private readonly float _startingAcceleration = 0.2f;
    [SerializeField] private float channellingAcceleration = 0.5f;
    [SerializeField] private float exhaustedAcceleration = 0.1f;
    private float _deceleration = -0.5f;

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
    [SerializeField] private float staminaWaterBoost = 20f;
    [SerializeField] private float volitionRegen = 0.3f;
    [SerializeField] private float volitionUseRate = 1.5f;
    [SerializeField] private float volitionLandmarkBoost = 20f;
    //[SerializeField] private float coolingFactor = 0.2f;
    [SerializeField] private AudioClip chime;
    [SerializeField] private AudioClip chime2;
    
    [Header("Slope")]
    [SerializeField] private float slopeFlatThreshold = 5f;
    [SerializeField] private float SlopeSteepThreshold = 15f;
    public float _slope; //tempPublic
    public float _slopeDirection; //tempPublic
    
    [Header("Speed")]
    [SerializeField] private float _targetSpeedChangeRate = 0.5f;
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private float _minSpeed = 1.3f;
    [SerializeField] private float volitionMaxSpeed;
    
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
        
        staminaBar.maxValue = MaxStamina;
        volitionBar.maxValue = _volition;
        /*
         Future implementation
        _temperature = 37f;
        _stability = 5;
        _muscleStrain = 0f; 
         */
    }
    
    private void Awake() {
        Observer.OnLandmarkSeen += LandmarkBoost;
    }

    private void OnDestroy() {
        Observer.OnLandmarkSeen -= LandmarkBoost;
    }
    
    private void Update() {
        if (exhaustion.CurrentLevel != 0) {
            _acceleration = exhaustedAcceleration;
        }else if (playerController.channelingVolition) {
            _acceleration = channellingAcceleration;
        }
        else
            _acceleration = _startingAcceleration;
        
        DetermineSlope();
        
        if (_running) {
            DetermineSpeedCategory();
            StaminaTick();
            VolitionTick();
            if (_realSpeed > _maxSpeed && !playerController.channelingVolition) {
                CheckMaxSpeed();
            }
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
    private void LandmarkBoost() {
        _volition = Mathf.Clamp(_volition + volitionLandmarkBoost, 0f, volitionBar.maxValue);
        AudioSource.PlayClipAtPoint(chime, Camera.main.transform.position);
    }
    public void WaterBoost() {
        StaminaBonus(staminaWaterBoost);
        AudioSource.PlayClipAtPoint(chime, Camera.main.transform.position);
    }

    public void StaminaBonus(float amount) {
        _stamina = Mathf.Clamp(_stamina + amount, 0f, MaxStamina);
    }
    
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

    [Header("Stamina breaks")]
    [SerializeField] private int staminaBreak1 = 50;
    [SerializeField] private int staminaBreak2 = 20;
    [SerializeField] private ExhaustionLevel exhaustion;
    private float MaxStamina = 100f;

    [SerializeField] private float staminaLimitedModifier = 0.85f;
    [SerializeField] private float speedLimitedModifier = 0.80f;
    [Header("Hyped Modifier")] 
    [SerializeField] private float StaminaRegenVolitionMod = 1.5f;
    [SerializeField] private float StaminaUseVolitionMod = 0.8f;
    
    
    private void StaminaTick() {
        if (_boosting) 
            return;
        
        multiplier = 1;
        switch (_speedCategory) {
            case SpeedCategory.WALKING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        multiplier = 2;
                        break;
                    case SlopesCat.LightDown:
                        multiplier = 2;
                        break;
                    case SlopesCat.Flat:
                        multiplier = 3;
                        break;
                    case SlopesCat.LightUp:
                        multiplier = 1;
                        break;
                    case SlopesCat.SteepUp:
                        multiplier = 0;
                        break;
                }
                break;
            case SpeedCategory.JOGGING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        multiplier = 0;
                        break;
                    case SlopesCat.LightDown:
                        multiplier = 1;
                        break;
                    case SlopesCat.Flat:
                        multiplier = -1;
                        break;
                    case SlopesCat.LightUp:
                        multiplier = -2;
                        break;
                    case SlopesCat.SteepUp:
                        multiplier = -3;
                        break;
                }
                break;
            case SpeedCategory.RUNNING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        multiplier = -1;
                        break;
                    case SlopesCat.LightDown:
                        multiplier = -1;
                        break;
                    case SlopesCat.Flat:
                        multiplier = -2;
                        break;
                    case SlopesCat.LightUp:
                        multiplier = -2;
                        break;
                    case SlopesCat.SteepUp:
                        multiplier = -3;
                        break;
                }
                break;
            case SpeedCategory.SPRINTING:
                switch (currentSlope) {
                    case SlopesCat.SteepDown:
                        multiplier = -2;
                        break;
                    case SlopesCat.LightDown:
                        multiplier = -1;
                        break;
                    case SlopesCat.Flat:
                        multiplier = -3;
                        break;
                    case SlopesCat.LightUp:
                        multiplier = -3;
                        break;
                    case SlopesCat.SteepUp:
                        multiplier = -4;//might be too much
                        break;
                }
                break;
        }

        if (_speedCategory == SpeedCategory.WALKING && playerController.channelingVolition) {
            multiplier *= StaminaRegenVolitionMod;
        }else if (multiplier < 0f && playerController.channelingVolition) {
            multiplier *= StaminaUseVolitionMod;
        }
        
        float adjustedRate = staminaUseRate * multiplier;
        if (staminaBreak1 == -1 && adjustedRate > 0f) {
            adjustedRate *= staminaLimitedModifier;
        }
        
        _stamina = Mathf.Clamp(_stamina + adjustedRate * Time.deltaTime, 0f, MaxStamina);
        
        if (_stamina <= staminaBreak1) {
            exhaustion.BrokenStaminaLimit(1);
            staminaBar.fillRect.GetComponent<Image>().color = Color.yellow;
            MaxStamina = staminaBreak1;
            staminaBreak1 = -1;
            StaminaBonus(-5f);
            _targetSpeedChangeRate *= speedLimitedModifier;
            
        }else if (_stamina <= staminaBreak2) {
            exhaustion.BrokenStaminaLimit(2);
            staminaBar.fillRect.GetComponent<Image>().color = Color.red;
            MaxStamina = staminaBreak2;
            staminaBreak2 = -1;
            _acceleration *= speedLimitedModifier;
        }
    }
    
    private void VolitionUITick() {
        volitionBar.value = _volition;
    }
    /*private void TemperatureTick() {
        _temperature -= Time.deltaTime * coolingFactor;
        _temperature = Mathf.Clamp(_temperature, 37.0f, 40.0f);
    }*/
    
    [Header("UI Icons")]
    [SerializeField] private GameObject Up1;
    [SerializeField] private GameObject Up2;
    [SerializeField] private GameObject Up3;
    [SerializeField] private GameObject Down1;
    [SerializeField] private GameObject Down2;
    [SerializeField] private GameObject Down3;
    private GameObject previousIcon;
    private float previousMult = 1;
    
    [SerializeField] private PatternScroller barScroller;
    private void StaminaUITick() {
        staminaBar.value = _stamina;
        if (_boosting) {
            barScroller.Stop();
            return;
        }
        if (!Mathf.Approximately(previousMult, multiplier)) {
            if (multiplier == 0) {
                barScroller.Stop();
            }
            else if (multiplier > 0) {
                var clampedMultiplier = Mathf.Clamp(multiplier, 1f, 3f);
                barScroller.Scroll(PatternScroller.RIGHT, Mathf.CeilToInt(clampedMultiplier));
            }
            else {
                var clampedMultiplier = Mathf.Clamp(multiplier, -3f, -1f);
                barScroller.Scroll(PatternScroller.LEFT, Mathf.CeilToInt(Mathf.Abs(clampedMultiplier)));
            }
            previousMult = multiplier;
        }
    }
    private void VolitionTick() {
        _volition += Time.deltaTime * volitionRegen;
        if (playerController.channelingVolition) {
            _volition -= Time.deltaTime * volitionUseRate;
        }
        _volition = Mathf.Clamp(_volition, 0f, volitionBar.maxValue);
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
    public void ResetSpeed() {
        _realSpeed = _targetSpeed = _minSpeed;
    }
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
                if (playerController.channelingVolition && exhaustion.CurrentLevel == 0) {
                    _targetSpeed = Mathf.Clamp(_targetSpeed + _targetSpeedChangeRate * Time.deltaTime, _minSpeed, _maxSpeed);
                }else
                    _targetSpeed = Mathf.Clamp(_targetSpeed + _targetSpeedChangeRate * Time.deltaTime, _minSpeed, volitionMaxSpeed);
                break;
            case 1: //Small jump - double tap
                
                break;
            case 2: //Big strides - continuous tapping
                
                break;           
        }
    }
    
    private void CheckMaxSpeed() {
        _targetSpeed = Mathf.Clamp(_targetSpeed, _minSpeed, _maxSpeed);
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
        
        _targetSpeed = _realSpeed = _maxSpeed;
        
        Image staminaFill = staminaBar.transform.GetChild(2).GetChild(0).GetComponent<Image>(); //Could give problems later
        
        AudioSource.PlayClipAtPoint(chime2, Camera.main.transform.position);
        
        Color imgC = staminaFill.color; //All this for the bar flashing
        Color.RGBToHSV(imgC, out float h, out float s, out float v);
        Anim.Instance.Animate(
            boostDuration,
            t => {
                staminaFill.color = Color.HSVToRGB(h, s, v + Mathf.Cos(t * UIbarFlashSpeed) * UIbarFlashForce);
            } ,null,
            () => {
                _boosting = false; 
                staminaFill.color = imgC;
                //Debug.Log("not boosting anymore");
            }); 
        
        Anim.Instance.Animate(
            boostCooldown,
            t => { } ,null,
            () => { _canBoost = true; Debug.Log("can boost again");}); 
        
    }
    
}
