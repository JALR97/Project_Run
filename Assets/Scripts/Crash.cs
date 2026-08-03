using System;
using Unity.Mathematics;
using UnityEngine;

public class Crash : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material baseMat;
    [SerializeField] Material flashMat;
    
    [SerializeField] float crashCooldown;
    [SerializeField] float graceTime;
    private float graceUntil;
    [SerializeField] float flashInterval;
    [SerializeField] AudioClip buzzer;
    private float timer;
    private bool crashed;
    

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Runner") && player.GetPlayerState() == PlayerController.PlayerState.Running) {
            if (Time.time <= graceUntil) {
                return;
            }
            crashed = true;
            player.SwitchState(PlayerController.PlayerState.Crashed);
            AudioSource.PlayClipAtPoint(buzzer, Camera.main.transform.position);
        }
    }

    private bool state;
    private void Update() {
        if (crashed) {
            timer += Time.deltaTime;
            state = Mathf.FloorToInt(timer / flashInterval) % 2 == 0;
            renderer.material = state ? flashMat : baseMat;
            if (timer >= crashCooldown) {
                crashed = false;
                timer = 0;
                player.SwitchState(PlayerController.PlayerState.Running);
                renderer.material = baseMat;
                graceUntil = Time.time + graceTime;
            }
        }
    }
}
