using System;
using UnityEngine;

public class Proto_StartPoint : MonoBehaviour {
    private float _t;
    void Update()
    {
        _t += Time.deltaTime * hoverSpeed;
        transform.Translate(Vector3.up * (Mathf.Sin(_t) * hoverAmplitude * Time.deltaTime));
    }

    public float hoverSpeed;
    public float hoverAmplitude;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<PlayerController>().SwitchState(PlayerController.PlayerState.Running);
            //fade Animation
            Destroy(gameObject);
        }
    }
}
