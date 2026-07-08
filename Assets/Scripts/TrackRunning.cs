using System;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class TrackRunning : MonoBehaviour {

    //Balance
    [SerializeField] private float runningSpeedMin;
    [SerializeField] private float runningSpeedMax;
    
    [SerializeField] private float yOffset;
    [SerializeField] private float HorizontalOffset;
    
    //Components
    private SplineContainer _splineContainer;
    [SerializeField] private Rigidbody _rb;
    
    //Inner work
    private float currentRatio;
    private float ratioSpeed;
    private float runningSpeed;
    private bool inverse;
    
    private void Start() {
        //Bindings:
        _splineContainer = BContainer.Instance._splineContainer;
        //--------------------//
        runningSpeed = Random.Range(runningSpeedMin, runningSpeedMax);
        ratioSpeed = runningSpeed / _splineContainer.Spline.GetLength();
        SetStartingRatio();
        transform.position = EvaluatedPosition();
    }

    private Vector3 EvaluatedPosition() {
        Vector3 position = GetRailPosition(currentRatio) + GetHorizontalOffsetVector(currentRatio);
        position.y += yOffset;
        return position;
    }

    private void FixedUpdate() {
        currentRatio += (inverse ? 1f : -1f) * ratioSpeed * Time.fixedDeltaTime;
        if (currentRatio > 1f) {
            currentRatio -= 1f;
        }else if (currentRatio < 0f) {
            currentRatio += 1f;
        }
        _rb.MovePosition(EvaluatedPosition());
    }

    public void SetInverse() {
        inverse = true;
    } 
    public void SetHorizontalOffset(float offset) {
        HorizontalOffset = offset;
    }

    private void SetStartingRatio() {
        var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out _, out var ratio, 30, 3);
        currentRatio = ratio;
    }

    private Vector3 GetHorizontalOffsetVector(float ratio) {
        var tangent = GetRailTangent(ratio);
        var horizontalVector = Vector3.Cross(tangent, Vector3.up);
        horizontalVector.Normalize();
        return horizontalVector * HorizontalOffset;
    }
    private Vector3 GetRailTangent(float ratio) {
        Vector3 tangent = SplineUtility.EvaluateTangent(_splineContainer.Spline, ratio);
        tangent.Normalize();
        if (inverse) {
            tangent = -1 * tangent;
        }
        tangent = _splineContainer.transform.TransformDirection(tangent);
        return tangent;
    }

    private Vector3 GetRailPosition(float ratio) {
        Vector3 position = SplineUtility.EvaluatePosition(_splineContainer.Spline, ratio);
        position = _splineContainer.transform.TransformPoint(position);
        return position;
    }
}
