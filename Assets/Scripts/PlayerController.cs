using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    //-----------------//Data structures//-----------------//
//enums


//structs


//-----------------//Components//-----------------//
//Internal Components


//Prefabs


//External References
    [SerializeField] private Transform cam;
    [SerializeField] private Transform groundedPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SplineContainer _splineContainer;

//-----------------//Variables//-----------------//
//Process variables - private
    private bool _isGrounded = false;
    private Vector3 adjustedDirection;
    private bool _auto = false;
    private Vector3 railDirection;
    private float _turnSmoothSpeed;
    private float t = 0;
    
//Balance variables - serialized 
    [SerializeField] private float speed;
    [SerializeField] private float strafeSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private float turnSmoothTime = 0.1f;

//Public properties - private set "Name { get; private set; }"


//-----------------//Functions//-----------------//
//Built-in
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            _auto = !_auto;
        }
        
        //Ground check
        _isGrounded = Physics.CheckSphere(groundedPoint.position, groundRadius, groundLayer);
        if (!_isGrounded)
        {
            transform.Translate(Vector3.down * (gravity * Time.deltaTime));
        }
        //Player input
        Vector3 inputDirection = Vector3.zero;
        if (!_auto) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }
        
        //Slope logic
        Physics.Raycast(groundedPoint.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        //Movement logic
        Vector3 targetDirection = Vector3.zero;
        //Behavior when autopilot is on aka running nonstop
        if (_auto) {
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
            
            //Rotation
            float targetAngle = Mathf.Atan2(0, 1) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothSpeed, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
        }else {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothSpeed, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        
        if (inputDirection.magnitude >= 0.1f || _auto) {
            
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


//External interaction - public


}
