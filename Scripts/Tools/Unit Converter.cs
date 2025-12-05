using UnityEngine;

public static class UnitConverter 
{
    public static float ToMillimeters(this float meters) => meters * 1000.0f;
    public static Vector3 ToMillimeters(this Vector3 metersVector) => metersVector * 1000.0f;

    public static float ToMeters(this float millimeters) => millimeters / 1000.0f;
    public static Vector3 ToMeters(this Vector3 millimetersVector) => millimetersVector / 1000.0f;
}
