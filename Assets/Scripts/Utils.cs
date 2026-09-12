using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;

public static class Utils
{
    public static VectorImage LoadVector(string filePath)
    {
        var guid = AssetDatabase.FindAssets(Path.GetFileName(filePath) + " t:VectorImage", new[] { Path.GetDirectoryName(filePath) })[0];
        return AssetDatabase.LoadAssetAtPath<VectorImage>(AssetDatabase.GUIDToAssetPath(guid));
    }

    //Vector2 Extensions
    public static float ToAngle(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

    //VisualElement Extensions
    public static float Width(this VisualElement v) => v.style.width.value.value;
    public static float Height(this VisualElement v) => v.style.height.value.value;
    public static void AddHeightPercent(this VisualElement v, float percent) => v.SetHeightPercent(Mathf.Min(v.Height() + percent, 100));
    public static void IncWidthByPercent(this VisualElement v, float percent) => v.SetWidthPercent(Mathf.Min(v.Width() + percent, 100));
    public static void SetHeightPercent(this VisualElement v, float percent) => v.style.height = Length.Percent(percent);
    public static void SetWidthPercent(this VisualElement v, float percent) => v.style.width = Length.Percent(percent);
}
