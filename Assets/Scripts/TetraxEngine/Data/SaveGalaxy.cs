using System.Collections.Generic;
using BayatGames.SaveGameFree;
using DelaunatorSharp.Unity.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace TetraxEngine.Data
{

    public class SaveGalaxy : MonoBehaviour
    {

        [System.Serializable]
        public struct SaveStar
        {
            public Vector3 location;

            public SaveStar(Vector3 location)
            {
                this.location = location;
            }
        }
        
        [System.Serializable]
        public struct SaveStarCell
        {
            public Vector3[] cellVectors;

            public SaveStarCell(Vector3[] cellVectors)
            {
                this.cellVectors = cellVectors;
            }
        }
        
        [System.Serializable]
        public struct SaveStarContainer
        {
            public SaveStar saveStar;
            public SaveStarCell cell;

            public SaveStarContainer(Vector3 star, Vector3[] cell)
            {
                this.saveStar = new SaveStar(star);
                this.cell = new SaveStarCell(cell);
            }
        }

        [System.Serializable]
        public class GalaxyData
        {
            public List<SaveStarContainer> starData;

            public GalaxyData()
            {
                starData = new List<SaveStarContainer>();
            }
        }

        public GalaxyData galaxyData;
        public GameObject galaxy;
        public bool loadOnStart = true;
        public string identifier = "SaveGalaxy";

        private void Start()
        {
            if (loadOnStart) Load();
        }

        private void AddStar(Vector3 star, Vector3[] cell)
        {
            var saveStarContainer = new SaveStarContainer(star, cell);
            // Debug.Log("star \""+ObjectDumper.Dump(star)+"\"");
            // Debug.Log("cell \""+ObjectDumper.Dump(cell)+"\"");
            // Debug.Log("galaxyData.starData \""+ObjectDumper.Dump(galaxyData.starData)+"\"");
            if (galaxyData.starData == null)
            {
                galaxyData.starData = new List<SaveStarContainer>();
            }
            galaxyData.starData.Add(saveStarContainer);
        }
        
        public void AddGalaxy(Dictionary<Vector3, Galaxy.Star.StarContainer> starContainers)
        {
            Debug.Log("Adding galaxy info to save data");
            galaxyData.starData = new List<SaveStarContainer>();
            foreach (var starContainer in starContainers)
            {
                AddStar(starContainer.Key, starContainer.Value.Cell.Points.ToVectors3());
            }
        }

        public void Save()
        {
            Debug.Log("Saving data");
            SaveGame.Save<GalaxyData>(identifier, galaxyData);
            
            Debug.Log("stars "+galaxyData.starData.Count);
            // Debug.Log(ObjectDumper.Dump(galaxyData.starData));
        }

        public void Load()
        {
            Debug.Log("Loading data");
            galaxyData = SaveGame.Load<GalaxyData>(identifier, new GalaxyData());
            Debug.Log("Loaded stars: "+galaxyData.starData.Count);
            
            galaxy.GetComponent<Galaxy.Galaxy>().BuildGalaxyFromData(galaxyData.starData);
        }

    }

}