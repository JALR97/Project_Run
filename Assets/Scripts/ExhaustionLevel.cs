using UnityEngine;

public class ExhaustionLevel : MonoBehaviour {

    //Exhaustion levels just go from 0 to 2. None, light and heavy.
    private int _currentExhaustion;
    public int CurrentLevel => _currentExhaustion;
    [SerializeField] private int maxExhaustionLevel = 2;
    
    private float _gracePeriodUntil = 0f;
    [SerializeField] private float gracePeriodLenght = 10f;
    
    [SerializeField] private int sprintsToExhaustion = 3;
    private int _exSpritCount;
    
    public void ExtendedSprint() {
        //Grace period check
        if (_gracePeriodUntil != 0f) {
            if (Time.time < _gracePeriodUntil) {
                return;
            }
            _gracePeriodUntil = 0f;
        }

        _exSpritCount++;
        if (_exSpritCount >= sprintsToExhaustion) {
            IncreaseExhaustion();
            _exSpritCount = 0;
        }
    }

    private void IncreaseExhaustion(int amount = 1) {
        if (_currentExhaustion == maxExhaustionLevel) {
            return;
        }else if (amount == 2) {
            _currentExhaustion = maxExhaustionLevel;
        }else
            _currentExhaustion++;
        
        PlaySoundCue();
    }
    
    public void BrokenStaminaLimit(int threshold) {
        
        IncreaseExhaustion(threshold);
        _exSpritCount = 0;
    }

    public void BreathControl(int execution) {
        //Reduce exhaustion and give stamina regen based on the execution value
    }

    private void PlaySoundCue() {
        //depending on the level of exhaustion the breathing sound should be harsher
    }
}
