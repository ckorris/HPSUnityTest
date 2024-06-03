
using System.Runtime.InteropServices;
using UnityEngine;


[StructLayout(LayoutKind.Sequential)]
public struct Float3
{
    public float x;
    public float y;
    public float z;

    public static implicit operator Vector3(Float3 float3)
    {
        return new Vector3(float3.x, float3.y, float3.z);
    }

    public static implicit operator Float3(Vector3 vector3)
    {
        return new Float3() {x = vector3.x, y = vector3.y, z = vector3.z };
    }
}

