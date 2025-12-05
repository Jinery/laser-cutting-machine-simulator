using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeLibrary : MonoBehaviour
{
    [SerializeField] private List<PresetShape> _shapes = new List<PresetShape>();

    public List<PresetShape> Shapes => _shapes;

    [SerializeField] private Sprite _squareImage, _triangleImage;

    private void CreateSampleShapes()
    {
        _shapes.Add(new PresetShape
        {
            shapeName = "Квадрат",
            shapeImage = _squareImage,
            points = new Vector2[]
            {
                new Vector2(50, 50),
                new Vector2(100, 50),
                new Vector2(100, 100),
                new Vector2(50, 100),
                new Vector2(50, 50)
            }
        });

        _shapes.Add(new PresetShape
        {
            shapeName = "Треугольник",
            shapeImage = _triangleImage,
            points = new Vector2[]
            {
                new Vector2(100, 50),
                new Vector2(100, 100),
                new Vector2(50, 100),
                new Vector2(100, 50)
            }
        });
    }

    private void Start()
    {
        CreateSampleShapes();
    }
}
