using System;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class TrackRunning : MonoBehaviour {
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float runningSpeedMin;
    [SerializeField] private float runningSpeedMax;
    private float runningSpeed;
    [SerializeField] private bool inverse;
    private SplineContainer _splineContainer;

    private void Start() {
        runningSpeed = Random.Range(runningSpeedMin, runningSpeedMax);
        _splineContainer = BContainer.Instance._splineContainer;
    }

    private void Update() {
        Physics.Raycast(transform.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out _, out var ratio, 100, 5);
            
        Vector3 tangent = _splineContainer.Spline.EvaluateTangent(ratio);
        tangent = inverse ? -tangent : tangent;
        Vector3 normalizedTangent = tangent.normalized; 
        Vector3 railDirection = _splineContainer.transform.TransformDirection(normalizedTangent); //Direction of the spline
        
        railDirection.y = 0;
        //Two options, don't zero out the y and see how that works, vs applying the sloperotation based on the track slope
        Vector3 adjustedDirection = slopeRotation * railDirection;
            
        transform.Translate(adjustedDirection * (runningSpeed * Time.deltaTime), Space.World);
    }
}
