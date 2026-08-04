using UnityEngine;

public class Landmark : MonoBehaviour {
// Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject tempLight;
    private bool seen;
    public bool Highlight() {
        if (!seen) {
            tempLight.SetActive(true);
            return true;
        }
        return false;
    }
    
    public void Un_Highlight() {
        if (!seen) {
            tempLight.SetActive(false);    
        }
    }
    
    public void Observed() {
        tempLight.SetActive(true);
        seen = true;
    }
}
