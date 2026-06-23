using UnityEngine;
using UnityEngine.Splines;

public class BContainer : MonoBehaviour
{
    //---------------Singleton
    public static BContainer Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    
    //---------------References
    public SplineContainer _splineContainer;
    
}
