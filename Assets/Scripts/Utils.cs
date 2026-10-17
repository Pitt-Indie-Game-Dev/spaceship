using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;
using static UnityEngine.Mathf;

public static class Utils
{
    public static VectorImage LoadVector(string filePath)
    {
        var guid = AssetDatabase.FindAssets(Path.GetFileName(filePath) + " t:VectorImage", new[] { Path.GetDirectoryName(filePath) })[0];
        return AssetDatabase.LoadAssetAtPath<VectorImage>(AssetDatabase.GUIDToAssetPath(guid));
    }

    public static Vector2 Radial(float angle, float magnitude) => new Vector2(Cos(angle * Deg2Rad), Sin(angle * Deg2Rad)) * magnitude;
    public static Vector2 Radial(float magnitude) => Radial(Random.Range(0,360), magnitude);

    //Vector2 Extensions
    public static float ToAngle(this Vector2 v) => Atan2(v.y, v.x) * Rad2Deg;

    //VisualElement Extensions
    public static float Width(this VisualElement v) => v.style.width.value.value;
    public static float Height(this VisualElement v) => v.style.height.value.value;
    public static void AddHeightPercent(this VisualElement v, float percent) => v.SetHeightPercent(Min(v.Height() + percent, 100));
    public static void IncWidthByPercent(this VisualElement v, float percent) => v.SetWidthPercent(Min(v.Width() + percent, 100));
    public static void SetHeightPercent(this VisualElement v, float percent) => v.style.height = Length.Percent(percent);
    public static void SetWidthPercent(this VisualElement v, float percent) => v.style.width = Length.Percent(percent);
}
