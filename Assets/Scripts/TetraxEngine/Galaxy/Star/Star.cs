using System;
using System.Linq;
using UnityEngine;

namespace TetraxEngine.Galaxy.Star
{
    public class Star : MonoBehaviour
    {
        [SerializeField] public StarSo template;
        [SerializeField] public StarType type;
        [ColorUsage(true,true)]
        [SerializeField] public Color color;
        [SerializeField] public float temp;
        [SerializeField] public float mass;
        [SerializeField] public float radius;
        [SerializeField] public float luminosity;
        public GameObject star;

        private static StarSo[] _starTypes;

        public void Awake()
        {
            if (_starTypes == null)
            {
                _starTypes = Resources.LoadAll("Stars", typeof(StarSo)).Cast<StarSo>().ToArray();
                Array.Sort(_starTypes, (o1, o2) => o1.Rarity.CompareTo(o2.Rarity));
            }

            var hash = TetraxUtils.HashFromPosition(star.GetComponent<Star>().transform.position);
            
            template = GetStarType(hash);
            type = template.StarType;
            color = template.Color;
            temp = template.Temp.SelectByHash(hash);
            mass = template.Mass.SelectByHash(hash);
            radius = template.Radius.SelectByHash(hash);
            luminosity = template.Luminosity.SelectByHash(hash);
        }
    
        private static StarSo GetStarType(float hash)
        {
            var mappedHash = TetraxUtils.Map(
                hash,
                0,
                TetraxUtils.MatrixSize,
                0,
                1);
            int i;
            float raritySum;
            for (i = 0, raritySum = _starTypes[i].Rarity;
                i < _starTypes.Length;
                raritySum = i + 2 < _starTypes.Length ? raritySum + _starTypes[i + 1].Rarity : 1, i++)
            {
                if (mappedHash > raritySum) continue;
            
                return _starTypes[i];
            }

            return _starTypes[_starTypes.Length - 1];
        }
    }
}