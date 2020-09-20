
using System;
using UnityEngine;

namespace TetraxEngine
{
    internal static class TetraxUtils
    {
        public const int MatrixSize = 99999999;

        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax) =>
            ((value - fromMin) * (toMax - toMin)) / (fromMax - fromMin) + toMin;
        
        public static int HashFromPosition(Vector3 position, int n = MatrixSize) => 
            Math.Abs(
                (
                    ((int)position.x * 40778063) ^
                    ((int)position.y * 73176001) ^
                    ((int)position.z * 20153153)
                ) % n);
        
        public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
        {
            if (query(transform)) {
                return transform;
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                var result = FirstOrDefault(transform.GetChild(i), query);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}