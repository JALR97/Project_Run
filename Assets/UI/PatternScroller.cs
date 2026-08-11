using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PatternScroller : MonoBehaviour
{
    [Header("Image")][SerializeField] private RawImage pattern;
    [SerializeField] private Color decreaseColor = Color.red; 
    [SerializeField] private Color increaseColor = Color.green;
    
    [Header("Alpha")]
    [SerializeField] private float intensityAlpha1;
    [SerializeField] private float intensityAlpha2;
    [SerializeField] private float intensityAlpha3;
    
    [Header("Speed")]
    [SerializeField] private float scrollSpeed1;
    [SerializeField] private float scrollSpeed2;
    [SerializeField] private float scrollSpeed3;
    private float currentScrollSpeed;
    
    public static readonly Vector2 RIGHT = new Vector2(-1f, 0f);
    public static readonly Vector2 LEFT = new Vector2(1f, 0f);
    
    private Vector2 scrollDirection = LEFT;
    
    private bool isScrolling;

    private void Start() {
        Stop();
    }

    private void Update() {
        if (!isScrolling) {return;}
        
        Rect uv = pattern.uvRect;
        uv.position += currentScrollSpeed * Time.deltaTime * scrollDirection;
        pattern.uvRect = uv;
    }

    public void Scroll(Vector2 direction, int intensity) {
        scrollDirection = direction;
        SwitchColor(scrollDirection);
        SetIntensity(intensity);
        isScrolling = true;
    }

    public void Stop() {
        isScrolling = false;
        SwitchAlpha(0f);
    }

    private void SetIntensity(int intensity) {
        float newSpeed = 0f;
        float newAlpha = 0f;
        switch (intensity) {
            case 1:
                newAlpha = intensityAlpha1;
                newSpeed = scrollSpeed1;
                break;
            case 2:
                newAlpha = intensityAlpha2;
                newSpeed = scrollSpeed2;
                break;
            case 3:
                newAlpha = intensityAlpha3;
                newSpeed = scrollSpeed3;
                break;
        }
        SwitchSpeed(newSpeed);
        SwitchAlpha(newAlpha);
    }
    
    [SerializeField] private float tweenDuration = 0.3f;
    
    private Tween alphaTween;
    private void SwitchAlpha(float alpha) {
        alphaTween.Stop();
        alphaTween = Tween.Alpha(
            pattern, 
            alpha,
            tweenDuration,
            ease: Ease.OutCubic);
    }

    private Tween speedTween; 
    private void SwitchSpeed(float speed) {
        speedTween.Stop();
        speedTween = Tween.Custom(
            startValue:currentScrollSpeed, 
            endValue:speed,
            duration:tweenDuration,
            onValueChange: value => currentScrollSpeed = value,
            ease: Ease.OutCubic);
    }
    
    private void SwitchColor(Vector2 direction) {
        Color newColor;
        if (direction == LEFT) {
            newColor = decreaseColor;
        }
        else {
            newColor = increaseColor;
        }
        pattern.color = newColor;
    }
    
}
