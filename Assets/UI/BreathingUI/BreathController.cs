using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BreathController : MonoBehaviour
{
    public enum EvalCategory {
        PERFECT,
        GOOD,
        MISS
    }
    
    [SerializeField] float _innerRadius = 25f;
    [SerializeField] float _outerRadius = 50f;
    [SerializeField] float _thickness = 10f;
    [SerializeField] Color _strokeCol = Color.red;
    
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
    float t_progressAlongRange;

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
    }

    private void OnDisable() {
        lungsButton.UnregisterCallback<PointerDownEvent>(clickDown);
        lungsButton.UnregisterCallback<PointerUpEvent>(clickUp);
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

    private void Miss() {
        inactive = true;
        isIncreasing = false;
        _currentRadius = _minRange;
    }

    private void Hit(EvalCategory category) {
        //Some FX
        //Some tracking of good hits plus effects on resources
        isIncreasing = !isIncreasing;
        Debug.Log($"Hit: {category}");
    }
    
    private void Evaluate() {
        if (isIncreasing) {
            if (_currentRadius >= _outerRadius - _perfectOffset &&  _currentRadius <= _outerRadius + _perfectOffset ) {
                Hit(EvalCategory.PERFECT);
            }else if (_currentRadius >= _outerRadius - _goodOffset && _currentRadius <= _outerRadius + _goodOffset) {
                Hit(EvalCategory.GOOD);
            }
            else
                Miss();
        }
        else {
            if (_currentRadius >= _innerRadius - _perfectOffset &&  _currentRadius <= _innerRadius + _perfectOffset ) {
                Hit(EvalCategory.PERFECT);
            }else if (_currentRadius >= _innerRadius - _goodOffset && _currentRadius <= _innerRadius + _goodOffset) {
                Hit(EvalCategory.GOOD);
            }else
                Miss();
        }
    }
    
    private void clickDown(PointerDownEvent e) {
        if (!inactive) 
            Evaluate();
        else {
            inactive = false;
            isIncreasing = true;
        }
    }
    
    private void clickUp(PointerUpEvent e) {
        if (!inactive) Evaluate();
    }
}
