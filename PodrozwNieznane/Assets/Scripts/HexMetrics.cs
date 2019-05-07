using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f; //(sqrt(3)/2)

    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    // flat top orientation:
    /*public static Vector3[] corners = {
        new Vector3(0.5f * outerRadius, 0f, innerRadius),
        new Vector3(outerRadius, 0f, 0f),
        new Vector3(0.5f * outerRadius, 0f, -innerRadius),
        new Vector3(-0.5f * outerRadius, 0f, -innerRadius),
        new Vector3(-outerRadius, 0f, 0f),
        new Vector3(-0.5f * outerRadius, 0f, innerRadius),
        new Vector3(0.5f * outerRadius, 0f, innerRadius), //"seventh" corner - duplicate of first
    };
    */
    // pointy top orientation:
    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) //"seventh" corner - duplicate of first
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }
    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
            blendFactor;
    }

}


public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }


}