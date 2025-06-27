using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

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

//-----------------//Variables//-----------------//
//Process variables - private
    private bool _isGrounded = false;
    private Vector3 adjustedDirection;

//Balance variables - serialized 
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private float turnSmoothTime = 0.1f;

//Public properties - private set "Name { get; private set; }"
    private float _turnSmoothSpeed;

//-----------------//Functions//-----------------//
//Built-in
    void Update()
    {
        //Ground check
        _isGrounded = Physics.CheckSphere(groundedPoint.position, groundRadius, groundLayer);
        if (!_isGrounded)
        {
            transform.Translate(Vector3.down * (gravity * Time.deltaTime));
        }
        //PLayer input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        //Slope logic
        Physics.Raycast(groundedPoint.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        //Movement logic
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
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
    }

    private void OnDrawGizmos()
    {
        //Ground check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundedPoint.position, groundRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + adjustedDirection);
    }

//Inner process - private


//External interaction - public


}
