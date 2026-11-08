using UnityEngine;
using UnityEngine.UI;

// A small UI mesh keeps panel edges crisp at every Canvas scale.
[RequireComponent(typeof(CanvasRenderer))]
public class OrbitalPanelGraphic : MaskableGraphic
{
    public Color borderColor = new Color(0.35f, 0.7f, 0.82f, 0.3f);
    public Color endColor = new Color(0.015f, 0.035f, 0.06f, 0.96f);
    public float corner = 10f;
    public float borderWidth = 1f;
    public bool horizontalGradient;

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect rect = GetPixelAdjustedRect();
        if (rect.width <= 0f || rect.height <= 0f) return;

        float cut = Mathf.Min(corner, Mathf.Min(rect.width, rect.height) * 0.25f);
        float inset = Mathf.Min(borderWidth, Mathf.Min(rect.width, rect.height) * 0.25f);
        Vector2[] outer = Corners(rect, cut);
        Rect innerRect = new Rect(rect.xMin + inset, rect.yMin + inset,
            rect.width - inset * 2f, rect.height - inset * 2f);
        Vector2[] inner = Corners(innerRect, Mathf.Max(0f, cut - inset));
        for (int i = 0; i < 8; i++) mesh.AddVert(outer[i], borderColor, Vector2.zero);
        for (int i = 0; i < 8; i++)
        {
            float t = horizontalGradient
                ? Mathf.InverseLerp(rect.xMin, rect.xMax, inner[i].x)
                : Mathf.InverseLerp(rect.yMax, rect.yMin, inner[i].y);
            mesh.AddVert(inner[i], Color.Lerp(color, endColor, t), Vector2.zero);
        }
        mesh.AddVert(rect.center, Color.Lerp(color, endColor, 0.5f), Vector2.zero);
        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;
            mesh.AddTriangle(i, next, 8 + next);
            mesh.AddTriangle(i, 8 + next, 8 + i);
            mesh.AddTriangle(16, 8 + i, 8 + next);
        }
    }

    private static Vector2[] Corners(Rect r, float c)
    {
        return new[] {
            new Vector2(r.xMin + c, r.yMax), new Vector2(r.xMax - c, r.yMax),
            new Vector2(r.xMax, r.yMax - c), new Vector2(r.xMax, r.yMin + c),
            new Vector2(r.xMax - c, r.yMin), new Vector2(r.xMin + c, r.yMin),
            new Vector2(r.xMin, r.yMin + c), new Vector2(r.xMin, r.yMax - c)
        };
    }
}
