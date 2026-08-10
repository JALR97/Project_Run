using System;
using UnityEngine;
using UnityEngine.UI;

public class PatternScroller : MonoBehaviour
{
    [SerializeField] private Color decreaseColor = Color.red; 
    [SerializeField] private Color increaseColor = Color.green;
    [SerializeField] private RawImage pattern;
    [SerializeField] private float scrollSpeed = 1;
    
    private static readonly Vector2 RIGHT = new Vector2(1f, 0f);
    private static readonly Vector2 LEFT = new Vector2(-1f, 0f);
    
    private Vector2 scrollDirection = RIGHT;
    
    private bool isScrolling = true;

    private void Update() {
        if (!isScrolling) {return;}
        
        Rect uv = pattern.uvRect;
        uv.position += scrollSpeed * Time.deltaTime * scrollDirection;
        pattern.uvRect = uv;
    }
    
    public void ScrollRight() {}
}
