using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;

public class BreathController : MonoBehaviour
{
    public enum EvalCategory {
        PERFECT,
        GOOD,
        MISS
    }
    
    [SerializeField] ExhaustionLevel exhaustion;
    
    [SerializeField] float _innerRadius = 25f;
    [SerializeField] float _outerRadius = 50f;
    [SerializeField] float _thickness = 10f;
    [SerializeField] Color _strokeCol = Color.red;
    [SerializeField] bool _hideInner = true;

    [Header("UI Colors")] 
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color lightExColor;
    [SerializeField] private Color heavyExColor;
    [SerializeField] private Color goodBreathColor;
    [SerializeField] private Color perfectBreathColor;
    
    [SerializeField] float _minRange = 40f;
    [SerializeField] float _maxRange = 60f;
    [SerializeField] float _currentRadius;
    [SerializeField] float radiusChangeSpeed = 1f;
    [SerializeField] AnimationCurve radiusSpeedCurve;
    [SerializeField] float _perfectOffset = 1f;
    [SerializeField] float _goodOffset = 5f;
    bool isIncreasing = true;
    bool ringsVisible = false;
    bool inactive = true;
    bool greyedOut;
    float t_progressAlongRange;
    private int hitsCount;
    private int hitsScore;

    VisualElement lungsButton;
    
    private void OnEnable() {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        root.Q<Lungs>().dataSource = this;
        root.Q<BreathIndicator>().dataSource = this;
        lungsButton = root.Q<VisualElement>("LungsButton");
        lungsButton.RegisterCallback<PointerDownEvent>(clickDown);
        lungsButton.RegisterCallback<PointerUpEvent>(clickUp);
        _currentRadius = _minRange;
        ExhaustionLevelUpdate(exhaustion.CurrentLevel);
    }

    private void OnDisable() {
        lungsButton.UnregisterCallback<PointerDownEvent>(clickDown);
        lungsButton.UnregisterCallback<PointerUpEvent>(clickUp);
        Miss();
    }

    private void Update() {
        if (inactive) 
            return;
        
        t_progressAlongRange = (_currentRadius - _minRange) / (_maxRange - _minRange);
        var adjustedSpeed = radiusSpeedCurve.Evaluate(t_progressAlongRange) * radiusChangeSpeed;
        _currentRadius += (isIncreasing ? 1 : -1) * adjustedSpeed * Time.deltaTime;
        
        if (_currentRadius >= _maxRange || _currentRadius <= _minRange)
            Miss();
        
        if (_currentRadius <= _minRange) 
            isIncreasing = true;
        else if (_currentRadius >= _maxRange)
            isIncreasing = false;
    }

    private Tween _missTween;
    public void Crash(){Miss();}
    private void Miss() {
        hitsCount = 0;
        hitsScore = 0;
        inactive = true;
        isIncreasing = false;
        _currentRadius = _minRange;
        if (!_missTween.isAlive) {
            AnimateColor(emptyColor);
        }
        _hideInner = true;
    }
    
    private void Hit(EvalCategory category) {
        if (_hideInner) _hideInner = false;
        
        Color col;
        if (category == EvalCategory.PERFECT) {
            col = perfectBreathColor;
            hitsScore += 2;
        }
        else {
            col = goodBreathColor;
            hitsScore += 1;
        }
        isIncreasing = !isIncreasing;
        
        hitsCount++;
        HitsCountCheck();
        AnimateColor(col);
    }

    private void HitsCountCheck() {
        if (hitsCount >= 2) {
            exhaustion.BreathControl(hitsScore);
            hitsCount = 0;
            hitsScore = 0;

            if (exhaustion.CurrentLevel == 0) {
                inactive = true;
                isIncreasing = true;
                _currentRadius = _minRange;
                _hideInner = true;
            }
        }
    }
    
    private void Evaluate() {
        float evalRadius = isIncreasing ? _outerRadius : _innerRadius;
        
        if (_currentRadius >= evalRadius - _perfectOffset &&  _currentRadius <= evalRadius + _perfectOffset ) {
            Hit(EvalCategory.PERFECT);
        }else if (_currentRadius >= evalRadius - _goodOffset && _currentRadius <= evalRadius + _goodOffset) {
            Hit(EvalCategory.GOOD);
        }
        else
            Miss();
    }
    
    private void clickDown(PointerDownEvent e) {
        if (greyedOut) return;
        
        if (!inactive) Evaluate();
        else {
            inactive = false;
            isIncreasing = true;
        }
    }
    
    private void clickUp(PointerUpEvent e) {
        if (greyedOut) return;
        
        if (!inactive) Evaluate();
    }

    public void ExhaustionLevelUpdate(int currentLevel) {
        if (!gameObject.activeInHierarchy) { Debug.Log(enabled); return; }
        
        if (currentLevel == 0) {
            hitsScore = 0;
            greyedOut = true;
            _strokeCol = emptyColor;
            lungsButton.style.opacity = 0.5f;
        }
        else {
            greyedOut = false;
            hitsScore = 0;
            lungsButton.style.opacity = 1f;
            switch (currentLevel) {
                case 1:
                    _strokeCol = lightExColor;
                    break;
                case 2:
                    _strokeCol = heavyExColor;
                    break;
            }
        }
    }
    
    [SerializeField] private float animSpeed = 0.2f;
    private void AnimateColor(Color color) {
        _missTween = Tween.Custom(
            startValue: _strokeCol,
            endValue: color,
            duration: animSpeed,
            onValueChange: value =>
            {
                _strokeCol = value;
            },
            cycles: 2,
            cycleMode: CycleMode.Yoyo,
            ease: Ease.OutSine
        );
    }
}
