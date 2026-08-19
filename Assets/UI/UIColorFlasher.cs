using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class UIColorFlasher : MonoBehaviour {
    [SerializeField] private Image barFill;
    [SerializeField] private float flashSpeed = 0.2f;
    [SerializeField] private float flashStrength = 0.2f;
    private Color baseColor;
    private Tween flashTween;

    private bool isFlashing;
    
    public void StartFlashing() {
        if (isFlashing) {
            return;
        }
        isFlashing = true;
        
        baseColor = barFill.color; 
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        
        flashTween = Tween.Custom(
            duration: flashSpeed,
            startValue: 0,
            endValue:1,
            onValueChange: value => barFill.color = Color.HSVToRGB(h, s, v + value * flashStrength),
            cycles: -1,
            cycleMode: CycleMode.Yoyo);
    }

    public void StopFlashing() {
        if (!isFlashing) {
            return;
        }
        isFlashing = false;
        flashTween.Stop();
        
        barFill.color = baseColor;
    }
}
