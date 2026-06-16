using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Observer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float radius = 0.35f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float observeTime = 2.5f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private InputActionReference clickAction;
    
    private GameObject CurrentTarget = null;
    private float timer;

    private bool lockedOn = false;
    
    public static event Action OnLandmarkSeen;
    void Update()
    {
        if (lockedOn) {
            //All the behavior when we are locked onto a landmark
            timer += Time.deltaTime;
            //Fade color of crosshair
            if (timer >= observeTime) {
                lockedOn = false;
                timer = 0f;
                OnLandmarkSeen?.Invoke();
                Debug.Log("seen");
                GameManager.Instance.SwitchCam(GameManager.Cameras.attention);
                CurrentTarget.GetComponent<Landmark>().Un_Highlight();
                CurrentTarget = null;
            }
            else {
                var distance = CurrentTarget.transform.position - transform.position;
                if (distance.magnitude > maxDistance || clickAction.action.WasPressedThisFrame()) {
                    CurrentTarget.GetComponent<Landmark>().Un_Highlight();
                    Debug.Log("not seen");
                    CurrentTarget = null;
                    lockedOn = false;
                    timer = 0f;
                    GameManager.Instance.SwitchCam(GameManager.Cameras.attention);
                }
            }
        }
        if (CurrentTarget && clickAction.action.WasPressedThisFrame()) {
            lockedOn = true; 
            GameManager.Instance.LockOnTarget(CurrentTarget.transform);
            GameManager.Instance.SwitchCam(GameManager.Cameras.lockedon);
            //All the code to assign the lock on and everything else
            timer = 0f;
        }
        
        if (!lockedOn) {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.SphereCast(ray, radius, out RaycastHit hit, maxDistance, targetLayer, QueryTriggerInteraction.Ignore))
            {
                if (!CurrentTarget) {
                    CurrentTarget = hit.collider.gameObject;
                    CurrentTarget.GetComponent<Landmark>().Highlight();
                }
                //There's a possible bug here if the player somehow is looking at one landmark and then another one in the next frame
                //I think it's impossible for this to happen but good to note here.
            }
            else if (CurrentTarget) { //This condition should read "if your ray didn't hit anything and also you had a current target before do this:
                CurrentTarget.GetComponent<Landmark>().Un_Highlight();
                CurrentTarget = null;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 start = ray.origin;
        Vector3 end = ray.origin + ray.direction * maxDistance;

        // Line (direction)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);

        // Start sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start, radius);

        // End sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(end, radius);
    }
}

