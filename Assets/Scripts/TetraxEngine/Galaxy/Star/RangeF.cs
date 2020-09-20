using UnityEngine;

namespace TetraxEngine.Galaxy.Star
{
    [System.Serializable]
    public struct RangeF
    {
        public float min;
        public float max;

        public RangeF(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float SelectRandom()
        {
            return Random.Range(min, max);
        }

        public float SelectByHash(float hash)
        {
            return TetraxUtils.Map(TetraxUtils.HashFromPosition(new Vector3(hash, 0f, 0f)), 0, TetraxUtils.MatrixSize, min, max);
        }
    }
}