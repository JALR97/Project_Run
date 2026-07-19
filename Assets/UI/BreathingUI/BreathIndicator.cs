using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class BreathIndicator : VisualElement
{
    private float _radius = 1f;

    [UxmlAttribute, CreateProperty]
    public float Radius
    {
        get => _radius;
        set {
            _radius = value;
            MarkDirtyRepaint(); }
    }
    
    public BreathIndicator() {
        generateVisualContent += GenerateVisualContent;
    }

    void GenerateVisualContent(MeshGenerationContext context) {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        painter.BeginPath();
        painter.lineWidth = 1f;
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f),_radius,0, 360);
        painter.ClosePath();
        painter.strokeColor = Color.white;
        painter.Stroke();
    }
}
