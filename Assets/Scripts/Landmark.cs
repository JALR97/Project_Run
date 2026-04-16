using UnityEngine;

public class Landmark : MonoBehaviour {
// Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject tempLight;
    public void Highlight() {
        tempLight.SetActive(true);
    }
    
    public void Un_Highlight() {
        tempLight.SetActive(false);
    }
}
