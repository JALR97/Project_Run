using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class RunnerSpawner : MonoBehaviour
{
    [Header("BContainer")]
    [SerializeField] private BContainer refContainer;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject runnerPrefab;

    [Header("Balance")]
    [SerializeField] private float runnersNum;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float horizontalOffsetMin;
    [SerializeField] private float horizontalOffsetMax;

    [Header("Effective Area")] 
    [SerializeField] private float ratioStart;
    [SerializeField] private float ratioEnd;

    [Header("DEBUG")]
    [Range(0f, 1f)]public float TempRatio;
    
    //private
    private SplineContainer splineContainer;
    private GameObject runnersContainer;
    private GameObject I_runnersContainer;
    
    private void Start() {
        splineContainer = refContainer._splineContainer;
        runnersContainer = refContainer.runnersContainer;
        I_runnersContainer = refContainer.I_runnersContainer;
        SpawnAllRunners();
    }

    private void SpawnAllRunners() {
        float segmentSize = (ratioEnd - ratioStart) / runnersNum;
        float i = ratioStart;
        for (int j = 0; j < runnersNum; j++) {
            var ratioRight = Random.Range(i + segmentSize * 0.1f, i + segmentSize * 0.9f);
            var ratioLeft = Random.Range(i + segmentSize * 0.1f, i + segmentSize * 0.9f);
            SpawnRunner(ratioRight, false);
            SpawnRunner(ratioLeft, true);
            i += segmentSize;
        }
    }
    
    private void SpawnRunner(float ratio, bool inverse) {
        var parent = inverse ? I_runnersContainer.transform : runnersContainer.transform;
        var position = GetPostitionFromRatio(ratio, out Vector3 tangent);
        position.y += verticalOffset;
        
        var runner = Instantiate(runnerPrefab, position, Quaternion.identity, parent);
        var trk = runner.GetComponent<TrackRunning>();
        if (inverse) {
            trk.SetInverse();
        }
        trk.SetHorizontalOffset(Random.Range(horizontalOffsetMin, horizontalOffsetMax));
    }

    private Vector3 GetPostitionFromRatio(float ratio, out Vector3 tangent) {
        var postition = splineContainer.transform.TransformPoint(SplineUtility.EvaluatePosition(splineContainer.Spline, ratio));
        tangent = splineContainer.transform.TransformDirection(SplineUtility.EvaluateTangent(splineContainer.Spline, ratio));
        return postition;
    }

    /*private void OnDrawGizmos() {
        splineContainer = refContainer._splineContainer;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetPostitionFromRatio(1f, out _), 0.2f);
    }*/
}
