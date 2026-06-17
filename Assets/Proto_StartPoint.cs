using System;
using TMPro;
using UnityEngine;

public class Proto_StartPoint : MonoBehaviour {
    private float _t;
    private float timer;
    private bool started;
    [SerializeField] private TMP_Text metaText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private AudioClip chime;
    void Update()
    {
        _t += Time.deltaTime * hoverSpeed;
        transform.Translate(Vector3.up * (Mathf.Sin(_t) * hoverAmplitude * Time.deltaTime));
        if (started) {
            timer += Time.deltaTime;
            timerText.text = FormatTimer(timer);
        }
    }

    public float hoverSpeed;
    public float hoverAmplitude;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (!started) {
                other.GetComponent<PlayerController>().SwitchState(PlayerController.PlayerState.Running);
                metaText.text = "FINISH!";
                started = true;
                AudioSource.PlayClipAtPoint(chime, Camera.main.transform.position);
            }else {
                started = false;
                AudioSource.PlayClipAtPoint(chime, Camera.main.transform.position);
                other.GetComponent<PlayerController>().SwitchState(PlayerController.PlayerState.Walking);
                //fade Animation
                timer = 0f;
                gameObject.SetActive(false);
            }
            
        }
    }
    
    string FormatTimer(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int wholeSeconds = Mathf.FloorToInt(seconds % 60f);
        int centiseconds = Mathf.FloorToInt((seconds * 100f) % 100f);

        return $"{minutes:00}:{wholeSeconds:00}:{centiseconds:00}";
    }
}
