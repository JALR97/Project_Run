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
    private Vector3 railDirection;
    
    [SerializeField] private float ratioRefreshRate = 1f;
    private float refreshTimer;
    /*[SerializeField] private float slopeRefreshRate = 0.1f;
    private float slopeTimer;*/
    
    private void Start() {
        runningSpeed = Random.Range(runningSpeedMin, runningSpeedMax);
        _splineContainer = BContainer.Instance._splineContainer;
        refreshTimer = Random.Range(0f, ratioRefreshRate);
    }

    private void Update() {
        //Possibly optimize here the raycasting
        Physics.Raycast(transform.position, Vector3.down, out var hit, 1f, groundLayer);
        Vector3 groundNormal = hit.normal;
        Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= ratioRefreshRate) {
            refreshTimer = 0f;
            SplineEval();
        }
        Vector3 adjustedDirection = slopeRotation * railDirection;
        transform.Translate(adjustedDirection * (runningSpeed * Time.deltaTime), Space.World);
    }

    public void SetInverse() {
        inverse = true;
    }

    private void SplineEval() {
        var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out _, out var ratio, 30, 3);
            
        Vector3 tangent = _splineContainer.Spline.EvaluateTangent(ratio);
        tangent = inverse ? -tangent : tangent;
        Vector3 normalizedTangent = tangent.normalized; 
        railDirection = _splineContainer.transform.TransformDirection(normalizedTangent); //Direction of the spline
        //Two options, don't zero out the y and see how that works, vs applying the sloperotation based on the track slope
        railDirection.y = 0;
    }
}
