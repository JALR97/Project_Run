using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Lungs : VisualElement
{
    float _innerRadius = 25f;
    float _outerRadius = 50f;
    float _thickness = 10f;
    Color _strokeCol = Color.red;

    //Exposed values
    [UxmlAttribute, CreateProperty]
    public float innerRadius
    {
        get => _innerRadius;
        set {
            _innerRadius = value;
            MarkDirtyRepaint(); }
    }
    [UxmlAttribute, CreateProperty]
    public float outerRadius{
        get => _outerRadius;
        set {
            _outerRadius = value;
            MarkDirtyRepaint(); }
    }
    [UxmlAttribute, CreateProperty]
    public float thickness{
        get => _thickness;
        set {
            _thickness = value;
            MarkDirtyRepaint(); }
    }
    [UxmlAttribute, CreateProperty]
    public Color strokeCol{
        get => _strokeCol;
        set {
            _strokeCol = value;
            MarkDirtyRepaint(); }
    }
    
    public Lungs() {
        generateVisualContent += GenerateVisualContent;
    }

    void GenerateVisualContent(MeshGenerationContext context) {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        //Inner
        painter.BeginPath();
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f),_innerRadius,0, 360);
        painter.ClosePath();
        painter.strokeColor = _strokeCol;
        painter.lineWidth = _thickness;
        painter.Stroke();
        
        //Outer
        painter.BeginPath();
        painter.lineWidth = _thickness;
        painter.Arc(new Vector2(width * 0.5f, height * 0.5f),_outerRadius,0, 360);
        painter.ClosePath();
        painter.strokeColor = _strokeCol;
        painter.Stroke();
    }

}
