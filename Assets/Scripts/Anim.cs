using UnityEngine;
using System;
using System.Collections;

public class Anim : MonoBehaviour
{
    //-----------------//Use Example//-----------------//
    /*     
     Animator.Instance.Animate(
            duration,
            t => { transform.localScale = Vector3.Lerp(StartingSize, TargetSize, t); },
            Animator.EaseOutCubic,
            () => {                
                Destroy(gameObject); 
            });     
     */
    //-----------------//SINGLETON//-----------------//
    public static Anim Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Coroutine Animate(
        float duration,
        Action<float> onUpdate,
        Func<float, float> ease = null,
        Action onComplete = null)
    {
        return StartCoroutine(AnimateRoutine(duration, onUpdate, ease ?? EaseLinear, onComplete));
    }
    //----------------------------------//
    
    private IEnumerator AnimateRoutine(
        float duration,
        Action<float> onUpdate,
        Func<float, float> ease,
        Action onComplete)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            onUpdate(ease(t));
            yield return null;
        }

        onUpdate?.Invoke(1f);
        onComplete?.Invoke();
    }

    public static float EaseLinear(float t) => t;
    public static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);
    public static float EaseInSine(float t) => 1 - Mathf.Cos((t * Mathf.PI) / 2);
    public static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    public static float EaseInCubic(float t) => t * t * t;

}