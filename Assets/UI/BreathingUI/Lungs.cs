using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Lungs : VisualElement
{
    private UIDocument uiDoc;
    private VisualElement lungsButton;

    private bool DeepBreath;
    private float breatheTimer;
    private float radius = 5f;

    public Lungs() {
        generateVisualContent += GenerateVisualContent;
    }

    void GenerateVisualContent(MeshGenerationContext context) {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        painter.BeginPath();
        painter.lineWidth = 5f;
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f),radius,0, 360);
        painter.ClosePath();
        painter.Stroke();
    }
    
    /*void Start(){
        uiDoc = GetComponent<UIDocument>();    
	
        lungsButton = uiDoc.rootVisualElement.Q<VisualElement>("LungsButton");
        lungsButton.RegisterCallback<PointerDownEvent>(OnDownLungs);
        lungsButton.RegisterCallback<PointerUpEvent>(OnUpLungs);
    }

    private void OnDownLungs(PointerDownEvent e) {
        DeepBreath = true;
    }

    private void OnUpLungs(PointerUpEvent e) {  
        DeepBreath = false;
    }

    private void Update() {
        if (DeepBreath) {
            breatheTimer += Time.deltaTime;
        }
    }*/
}
