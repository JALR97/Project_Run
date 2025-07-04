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
    private Vector3 temp_spline;
    private float _turnSmoothSpeed;
    private float t = 0;
    
//Balance variables - serialized 
    [SerializeField] private float speed;
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        //Slope logic
        Physics.Raycast(groundedPoint.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        //Movement logic
        if (!_auto && inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothSpeed, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            adjustedDirection = slopeRotation * targetDirection;

            /*if (adjustedDirection.y < 0)
            {
                //Let's not use this for noe   
            }*/
            
            transform.Translate(adjustedDirection * (speed * Time.deltaTime), Space.World);
        }

        /*if (true) {
            var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out var nearest, out var ratio, 10, 10);
            
            temp_spline = _splineContainer.Spline.EvaluatePosition(ratio);
            Debug.Log(temp_spline);
            
            //temp_spline = nearest;
            //Debug.Log(temp_spline);
        }*/
        t += Time.deltaTime / 5;
        temp_spline = _splineContainer.Spline.EvaluatePosition(t);
        temp_spline = _splineContainer.transform.TransformPoint(temp_spline);
        t = t >= 1f ? 0f : t;
    }

    private void OnDrawGizmos()
    {
        //Ground check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundedPoint.position, groundRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + adjustedDirection);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(temp_spline, 1.0f);
    }

//Inner process - private


//External interaction - public


}
