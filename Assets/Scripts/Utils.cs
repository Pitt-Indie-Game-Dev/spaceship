using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;
using static UnityEngine.Mathf;
using Unity.VisualScripting;

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

    public static void RotateTowards(this Transform t, Transform target, float delta) => t.RotateTowards(target.position, delta);
    public static void RotateTowards(this Transform t, Vector2 target, float delta) =>
        t.rotation = Quaternion.Euler(0, 0, MoveTowardsAngle(t.eulerAngles.z, target.ToAngle(), delta));
    

    //VisualElement Extensions
    public static float Width(this VisualElement v) => v.style.width.value.value;
    public static float Height(this VisualElement v) => v.style.height.value.value;
    public static float Angle(this VisualElement v) => v.style.rotate.value.angle.value;
    public static void AddHeightPercent(this VisualElement v, float percent) => v.SetHeightPercent(Min(v.Height() + percent, 100));
    public static void IncWidthByPercent(this VisualElement v, float percent) => v.SetWidthPercent(Min(v.Width() + percent, 100));
    public static void SetHeightPercent(this VisualElement v, float percent) => v.style.height = Length.Percent(percent);
    public static void SetWidthPercent(this VisualElement v, float percent) => v.style.width = Length.Percent(percent);
    public static void AddAngle(this VisualElement v, float addition) => v.style.rotate = new Rotate(new Angle(v.Angle() + addition));
    public static void SetAngle(this VisualElement v, float angle) => v.style.rotate = new Rotate(new Angle(angle));
}
