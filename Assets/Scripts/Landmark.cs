using UnityEngine;

public class Landmark : MonoBehaviour {
// Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject tempLight;
    private bool seen;
    public void Highlight() {
        tempLight.SetActive(true);
    }
    
    public void Un_Highlight() {
        tempLight.SetActive(false);
    }
    
    public void Observed() {
        tempLight.SetActive(false);
        seen = true;
    }
}
