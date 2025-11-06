using UnityEngine;
using System;

[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static Position FromVector3(Vector3 v)
    {
        return new Position { x = v.x, y = v.y, z = v.z };
    }
}
