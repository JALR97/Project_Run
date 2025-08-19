using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    //-----------------//Data structures//-----------------//
    public enum PlayerState {
        Inactive,
        Walking,
        Running
    }
    public enum RunningMode {
        Control,
        Attention,
        Information
    }

    //-----------------//Components//-----------------//
    //Internal Components
                                       
                                       
    //External References              
    [SerializeField] private Transform cam;
    [SerializeField] private Transform groundedPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SplineContainer _splineContainer;
    
    [SerializeField] private GameObject _modeSwitchUI;
    
    //Inputs
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference speedAction;
    [SerializeField] private InputActionReference modeSwitchAction;

//-----------------//Variables//-----------------//
//Process variables - private
    //movement stuff
    private bool _isGrounded = false;
    private Vector3 adjustedDirection;
    private Vector3 railDirection;
    private float _turnSmoothSpeed;
    private float t = 0;
    
    //state stuff
    private PlayerState playerState = PlayerState.Walking;
    private RunningMode runningMode;
    
    
    //Balance variables - serialized 
    [SerializeField] private float speed;
    [SerializeField] private float strafeSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    //Public properties - private set "Name { get; private set; }"


    //-----------------//Functions//-----------------//
    //Built-in
    private void OnEnable() {
        modeSwitchAction.action.performed += ModeSwitchUI;
    }

    private void OnDisable() {
        modeSwitchAction.action.performed -= ModeSwitchUI;
    }

    void Update()
    {
        //Ground check
        _isGrounded = Physics.CheckSphere(groundedPoint.position, groundRadius, groundLayer);
        if (!_isGrounded)
        {
            transform.Translate(Vector3.down * (gravity * Time.deltaTime));
        }
        //Player input
        Vector3 inputDirection = Vector3.zero;
        if (playerState == PlayerState.Walking) {
            var input2D = movementAction.action.ReadValue<Vector2>();
            inputDirection = new Vector3(input2D.x, 0f, input2D.y);
            /*float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            inputDirection = new Vector3(horizontal, 0f, vertical).normalized;*/
        }
        
        //Slope logic
        Physics.Raycast(groundedPoint.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        //Movement logic
        Vector3 targetDirection = Vector3.zero;
        //Behavior when autopilot is on aka running nonstop
        if (playerState == PlayerState.Running) {
            var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out _, out var ratio, 100, 5);
            
            Vector3 tangent = _splineContainer.Spline.EvaluateTangent(ratio);
            Vector3 normalizedTangent = tangent.normalized; 
            railDirection = _splineContainer.transform.TransformDirection(normalizedTangent); //Direction of the spline
            railDirection.y = 0; 
            targetDirection = railDirection;
            
            //Strafing
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > 0f) {
                targetDirection += Vector3.Cross(Vector3.up, targetDirection) * (strafeSpeed * Time.deltaTime);
            }else if (horizontal < 0f) {
                targetDirection += Vector3.Cross(targetDirection, Vector3.up) * (strafeSpeed * Time.deltaTime);
            }
            
            //rotation from movement
            float targetAngle = Mathf.Atan2(railDirection.x, railDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothSpeed, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
        }else {
            //Rotation from camera and input - Walking
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothSpeed, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        
        //Actual translation part
        if (inputDirection.magnitude >= 0.1f || playerState == PlayerState.Running) {
            
            adjustedDirection = slopeRotation * targetDirection;
            //adjustedDirection += strafeSpeed;
            
            transform.Translate(adjustedDirection * (speed * Time.deltaTime), Space.World);
        }
    }

    private void OnDrawGizmos()
    {
        //Ground check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundedPoint.position, groundRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + adjustedDirection);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + railDirection * 2f);
    }

    //Inner process - private
    private void ModeSwitchUI(InputAction.CallbackContext callbackContext) {
        if (playerState == PlayerState.Running) {
            _modeSwitchUI.SetActive(true);    
        }
    }
    
    //External interaction - public
    public void SwitchState(PlayerState newState)
    {
        playerState = newState;
        switch (newState)
        {
            case PlayerState.Inactive:
                
                break;
            case PlayerState.Walking:
                GameManager.Instance.SwitchCam();
                break;
            case PlayerState.Running:
                SwitchMode(RunningMode.Control);
                GameManager.Instance.SwitchCam();
                break;
        }
    }
    public void SwitchMode(RunningMode newMode)
    {
        runningMode = newMode;
        switch (newMode)
        {
            case RunningMode.Control:
                Debug.Log("Mode:Control");
                break;
            case RunningMode.Attention:
                Debug.Log("Mode:Attention");
                break;
            case RunningMode.Information:
                Debug.Log("Mode:Info");
                break;
        }
    }

}
