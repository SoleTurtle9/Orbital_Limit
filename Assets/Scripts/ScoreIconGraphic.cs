using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class ScoreIconGraphic : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vertices)
    {
        vertices.Clear();

        Rect rect = GetPixelAdjustedRect();
        float outerRadius = Mathf.Min(rect.width, rect.height) * 0.5f;
        if (outerRadius <= 0f)
            return;

        Vector2 center = rect.center;
        vertices.AddVert(center, color, Vector2.zero);

        // Draw a five-pointed star directly, without a font glyph or texture asset.
        for (int i = 0; i < 10; i++)
        {
            float angle = (90f - i * 36f) * Mathf.Deg2Rad;
            float radius = i % 2 == 0 ? outerRadius : outerRadius * 0.45f;
            Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vertices.AddVert(point, color, Vector2.zero);
        }

        for (int i = 0; i < 10; i++)
        {
            vertices.AddTriangle(0, i + 1, (i + 1) % 10 + 1);
        }
    }
}
