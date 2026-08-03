using System;
using UnityEngine;

public class ExhaustionLevel : MonoBehaviour {

    //components
    [SerializeField] private ResourceEngine _engine;
    [SerializeField] private BreathController _breathController;
    
    //Balance
    [SerializeField] private float baseBonus;
    [SerializeField] private int pointMultiplier;
    
    //Audio
    [SerializeField] private AudioClip _lightExSound;
    [SerializeField] private AudioClip _heavyExSound;
    
    //Exhaustion levels just go from 0 to 2. None, light and heavy.
    private int _currentExhaustion;
    public int CurrentLevel => _currentExhaustion;
    [SerializeField] private int maxExhaustionLevel = 2;
    
    private float _gracePeriodUntil = 0f;
    [SerializeField] private float gracePeriodLenght = 10f;
    
    [SerializeField] private int sprintsToExhaustion = 3;
    private int _exSprintCount;

    [SerializeField] private int DEBUGstartingEx;
    private void Start() {
        IncreaseExhaustion(DEBUGstartingEx);
    }

    public void ExtendedSprint() {
        //Grace period check
        if (_gracePeriodUntil != 0f) {
            if (Time.time < _gracePeriodUntil) {
                return;
            }
            _gracePeriodUntil = 0f;
        }

        _exSprintCount++;
        if (_exSprintCount >= sprintsToExhaustion) {
            IncreaseExhaustion();
            _exSprintCount = 0;
        }
    }

    public void BrokenStaminaLimit(int threshold) {
        IncreaseExhaustion(threshold);
        _exSprintCount = 0;
    }

    private void IncreaseExhaustion(int amount = 1) {
        if (_currentExhaustion == maxExhaustionLevel || amount == 0) {
            return;
        }else if (amount == 2) {
            _currentExhaustion = maxExhaustionLevel;
        }else
            _currentExhaustion++;
        
        _breathController.ExhaustionLevelUpdate(_currentExhaustion);
        PlaySoundCue();
    }

    private void LowerExhaustion(int amount = 1) {
        //For now, you have to do the breath control twice when at level 2 exhaustion. could explore just once in the
        //future but with a longer streak of "breaths"
        if (_currentExhaustion == 0) {
            Debug.LogError("Shouldn't happen. Trying to lower exhaustion past 0");
            return;
        }
        _currentExhaustion -= amount;
        _breathController.ExhaustionLevelUpdate(_currentExhaustion);
        PlaySoundCue();
    }

    public void BreathControl(int execution) {
        //Reduce exhaustion and give stamina regen based on the execution value
        var staminaBonus = baseBonus + execution * pointMultiplier;
        _engine.StaminaBonus(staminaBonus);
        LowerExhaustion();
    }

    private void PlaySoundCue() {
        //depending on the level of exhaustion the breathing sound should be harsher
        AudioClip clip;
        switch (_currentExhaustion) {
            case 1:
                clip = _lightExSound;
                break;
            case 2:
                clip = _heavyExSound;
                break;
            default:
                return;
        }
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
