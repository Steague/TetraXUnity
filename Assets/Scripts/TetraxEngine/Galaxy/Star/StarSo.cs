using UnityEngine;

namespace TetraxEngine.Galaxy.Star
{
    public enum StarType {
        A,B,F,G,K,M,O
    }

    [CreateAssetMenu(fileName = "Data", menuName = "Data/StarSo", order = 1)]
    public class StarSo : ScriptableObject
    {
        [SerializeField] private StarType type;
        [ColorUsage(true,true)]
        [SerializeField] private Color color;
        [SerializeField] private RangeF temp;
        [SerializeField] private RangeF mass;
        [SerializeField] private RangeF radius;
        [SerializeField] private RangeF luminosity;
        [Range(0, 1)]
        [SerializeField] private float rarity;

        public StarType StarType => type;
        public Color Color => color;
        public RangeF Temp => temp;
        public RangeF Mass => mass;
        public RangeF Radius => radius;
        public RangeF Luminosity => luminosity;
        public float Rarity => rarity;
    }
}