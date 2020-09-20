using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using TetraxEngine.Data;
using TetraxEngine.Galaxy.Star;
using UnityEngine;

namespace TetraxEngine.Galaxy
{
    public class Galaxy : MonoBehaviour
    {
        public float spiralConstantA = 0.5f;
        public float spiralConstantK = 0.225f;
        [Range(10.0f, 200.0f)]
        public float coreRadius = 100f;
        [Range(200.0f, 900.0f)]
        public float galaxyRadius = 450f;

        public GameObject starContainer;
        public readonly Dictionary<Vector3, StarContainer> StarContainers = new Dictionary<Vector3, StarContainer>();

        private void Start()
        {
            
        }

        public void BuildGalaxyFromData(List<SaveGalaxy.SaveStarContainer> starContainers)
        {
            StarContainers.Clear();
            ClearChildren();

            for (var i = 0; i < starContainers.Count; i++)
            {
                var curStarPoint = starContainers[i].saveStar.location;
                var pointsArray = new IPoint[starContainers[i].cell.cellVectors.Length];
                for (var j = 0; j < starContainers[i].cell.cellVectors.Length; j++)
                {
                    var cellVector = starContainers[i].cell.cellVectors[j];
                    pointsArray[j] = new Point(cellVector.x, cellVector.y);
                }
                var cell = (IVoronoiCell) new VoronoiCell(i, pointsArray);
                var curStarContainer = SetupStarContainer(curStarPoint);
                StarContainers.Add(curStarPoint, curStarContainer.GetComponent<StarContainer>().Setup(curStarPoint, cell));
            }
        }

        public void CreateNewGalaxy()
        {
            StarContainers.Clear();
            ClearChildren();
            
            BuildGalaxy();
        }

        public void SaveGalaxy()
        {
            Debug.Log("Clicked save button");
            //SaveSystem.SaveStars(starContainers);
        }

        private void BuildGalaxy()
        {
            var spiralPoints = SpiralPoints();
            var poissonPoints = PoissonPoints();
            
            var enumerable = poissonPoints as Vector2[] ?? poissonPoints.ToArray();
            var stars = enumerable.ToPoints();
            var del = new Delaunator(stars);
         
            var delPoints = del.Points;
            del.ForEachVoronoiCell(cell =>
            {
                // Where the hell even is the star?
                var curStarPoint = delPoints[cell.Index].ToVector3();

                // Return if the star is outside of the galaxy
                if (OutsideGalaxy(curStarPoint, spiralPoints)) return;

                // Setup the star container to hold the star, the cell around it, etc
                var curStarContainer = SetupStarContainer(curStarPoint);
                StarContainers.Add(curStarPoint, curStarContainer.GetComponent<StarContainer>().Setup(curStarPoint, cell));
            });
        }

        private void ClearChildren()
        {
            var childCount = transform.childCount;
            Debug.Log(childCount);
            var i = 0;

            //Array to hold all child obj
            var allChildren = new GameObject[childCount];

            //Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (var child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }

            Debug.Log(transform.childCount);
        }
        
        private GameObject SetupStarContainer(Vector3 curStarPoint)
        {
            var curStarContainer = Instantiate(starContainer, curStarPoint, Quaternion.identity);
            // Attach the cloned object to the Galaxy
            curStarContainer.GetComponent<StarContainer>().transform.SetParent(transform);

            return curStarContainer;
        }
        
        private bool OutsideGalaxy(Vector3 curStarPoint, IEnumerable<Vector2> spiralPoints)
        {
            const float margin = 0.07f;
         
            var distanceFromCenter = Vector2.Distance(Vector2.zero, curStarPoint);
            if (distanceFromCenter > galaxyRadius * (1f - margin)) return true;
            if (distanceFromCenter < coreRadius) return true;
         
            return !spiralPoints.Any(v => Vector2.Distance(curStarPoint, v) <= galaxyRadius * margin);
        }
        
        private IEnumerable<Vector2> PoissonPoints()
        {
            var sampler = new PoissonDiscSampler(galaxyRadius * 2, galaxyRadius * 2, galaxyRadius * 0.07f);
            var samples = new List<Vector2>();
            foreach (var sample in sampler.Samples())
            {
                sample.Set(sample.x - galaxyRadius, sample.y - galaxyRadius);

                samples.Add(sample);
            }

            return samples;
        }

        private IEnumerable<Vector2> SpiralPoints()
        {
            var spiral = GetSpiralPoints(spiralConstantA, spiralConstantK, coreRadius, galaxyRadius);
            var spiralPoints = new List<Vector2>();
            foreach (var v in spiral)
            {
                spiralPoints.Add(new Vector2(v.x, v.y));
                spiralPoints.Add(new Vector2(-v.x, -v.y));
            }

            return spiralPoints;
        }
        
        private static IEnumerable<Vector2> GetSpiralPoints(double a, double k, float minRadius, float maxRadius)
        {
            var minTheta = Math.Log(0.1 / a) / k;
            const float dTheta = (float)(5 * Math.PI / 180);    // Five degrees.
            var points = new List<Vector2>();
            for (var theta = minTheta; ; theta += dTheta)
            {
                var r = a * Math.Exp(k * theta);
                var r2 = (a - 0.49) * Math.Exp(k * theta);

                if (r >= minRadius && r <= maxRadius)
                {
                    var x = r * Math.Cos(theta);
                    var y = r * Math.Sin(theta);
                    points.Add(new Vector2((float) x, (float) y));
                }

                if (r2 >= minRadius && r2 <= maxRadius)
                {
                    var x2 = r2 * Math.Cos(theta);
                    var y2 = r2 * Math.Sin(theta);
                    points.Add(new Vector2((float) x2, (float) y2));
                }

                // If we have gone far enough, stop.
                if (r > maxRadius && r2 > maxRadius) break;
            }
            return points;
        }
    }
}

// public class Galaxy : MonoBehaviour
// {
//     public float spiralConstantA = 0.5f;
//     public float spiralConstantK = 0.225f;
//     [Range(10.0f, 200.0f)]
//     public float coreRadius = 100f;
//     [Range(200.0f, 900.0f)]
//     public float galaxyRadius = 450f;
//
//     public GameObject core;
//     public GameObject starContainer;
//     public Transform mainCamera;
//
//     private readonly Dictionary<Vector3, GameObject> _starContainers = new Dictionary<Vector3, GameObject>();
//     
//     private Delaunator _del;
//     private IPoint[] _delPoints;
//     private int _clickedId = -1;
//     private Camera _camera;
//     public GameObject reticule;
//     private Canvas _canvas;
//     private RectTransform _rt;
//
//     private GameObject SetupStarContainer(Vector3 curStarPoint)
//     {
//         var curStarContainer = Instantiate(starContainer, curStarPoint, Quaternion.identity);
//         curStarContainer.GetComponent<StarContainer>().transform.SetParent(transform);
//
//         return curStarContainer;
//     }
//
//     // Start is called before the first frame update
//     private void Start()
//     {
//         _camera = Camera.main;
//         reticule = Instantiate(reticule, Vector3.zero - new Vector3(10000,10000, 0), Quaternion.identity);
//         _canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
//         reticule.transform.SetParent(_canvas.transform);
//         _rt = reticule.GetComponent<RectTransform>();
//         _rt.SetParent(_canvas.transform);
//         
//         // Instantiate(core, new Vector3(0, 0, 0), Quaternion.identity);
//         
//         var spiralPoints = SpiralPoints();
//         var poissonPoints = PoissonPoints();
//
//         var enumerable = poissonPoints as Vector2[] ?? poissonPoints.ToArray();
//         var stars = enumerable.ToPoints();
//         _del = new Delaunator(stars);
//         
//         _delPoints = _del.Points;
//         _del.ForEachVoronoiCell(cell =>
//         {
//             // Where the hell even is the star?
//             var curStarPoint = _delPoints[cell.Index].ToVector3();
//
//             // Return if the star is outside of the galaxy
//             if (OutsideGalaxy(curStarPoint, spiralPoints)) return;
//
//             // Setup the star container to hold the star, the cell around it, etc
//             var curStarContainer = SetupStarContainer(curStarPoint);
//             _starContainers.Add(curStarPoint, curStarContainer.GetComponent<StarContainer>().Setup(curStarPoint, cell));
//         });
//
//         // var cells = del.GetVoronoiCells();
//         // var voronoiCells = cells as IVoronoiCell[] ?? cells.ToArray();
//         // //foreach (var sample in enumerable)
//         // var valid = false;
//         // for (var i = 0; i < enumerable.Length; i++)
//         // {
//         //     if (valid) break;
//         //     var sample = enumerable[i];
//         //     var cell = voronoiCells[i];
//         //     var distanceFromCenter = Vector2.Distance(Vector2.zero, sample);
//         //     if (distanceFromCenter > galaxyRadius) continue;
//         //     if (distanceFromCenter < coreRadius) continue;
//         //
//         //     foreach (var v in spiralPoints.Where(v => Vector2.Distance(sample, v) <= galaxyRadius * 0.07f))
//         //     {
//         //         valid = true;
//         //         var curStar = Instantiate(star, new Vector3(sample.x, sample.y, 0), Quaternion.identity);
//         //         // var starDust = Instantiate(dust, new Vector3(sample.x, sample.y, 0), Quaternion.identity);
//         //         // var spiralDust = Instantiate(dust, v, Quaternion.identity);
//         //
//         //         curStar.GetComponent<Star>().SetId(i);
//         //
//         //         var curStarCell = Instantiate(starCell, Vector2.zero, Quaternion.identity);
//         //         curStarCell.GetComponent<PlaneFromPoly>().Poly = cell.Points.ToVectors3();
//         //         curStarCell.GetComponent<PlaneFromPoly>().SetPolyMesh(i);
//         //         // curStarCell.GetComponent<PlaneFromPoly>().transform.SetParent(curStar.transform);
//         //
//         //         var component = curStar.GetComponent<Star>();
//         //         var starRadius = component.radius;
//         //         var starLuminosity = component.luminosity;
//         //         var starColor = component.color;
//         //
//         //         // Set the star Radius
//         //         var radius = Map(starRadius, 0.00001f, 10f, 1.5f, 6f);
//         //         curStar.transform.localScale = new Vector3(radius, radius, radius);
//         //         
//         //         // Calculate the star and dust cloud luminosity 
//         //         var luminosity = Map(starLuminosity, 0.08f, 1100000f, 500f, 5000f);
//         //         
//         //         // Tell the billboard dust cloud where the camera is
//         //         // starDust.SendMessage("SetTarget", mainCamera.transform);
//         //         // spiralDust.SendMessage("SetTarget", mainCamera.transform);
//         //         
//         //         // Tint the star's material
//         //         var starMat = curStar.gameObject.GetComponent<MeshRenderer>().material;
//         //         starMat.EnableKeyword("_EMISSION");
//         //         starMat.SetColor(EmissionColor, starColor * luminosity);
//         //         
//         //         // Tint the star's dust's material
//         //         // var starDustMat = starDust.gameObject.GetComponent<MeshRenderer>().material;
//         //         // starDustMat.EnableKeyword("_EMISSION");
//         //         // starDustMat.SetColor(EmissionColor, starColor * luminosity / 5000);
//         //
//         //         break;
//         //     }
//         // }
//     }
//
//     private bool OutsideGalaxy(Vector3 curStarPoint, IEnumerable<Vector2> spiralPoints)
//     {
//         const float margin = 0.07f;
//         
//         var distanceFromCenter = Vector2.Distance(Vector2.zero, curStarPoint);
//         if (distanceFromCenter > galaxyRadius * (1f - margin)) return true;
//         if (distanceFromCenter < coreRadius) return true;
//         
//         return !spiralPoints.Any(v => Vector2.Distance(curStarPoint, v) <= galaxyRadius * margin);
//     }
//
//     private IEnumerable<Vector2> PoissonPoints()
//     {
//         var sampler = new PoissonDiscSampler(galaxyRadius * 2, galaxyRadius * 2, galaxyRadius * 0.07f);
//         var samples = new List<Vector2>();
//         foreach (var sample in sampler.Samples())
//         {
//             sample.Set(sample.x - galaxyRadius, sample.y - galaxyRadius);
//
//             samples.Add(sample);
//         }
//
//         return samples;
//     }
//     
//     private List<Vector2> SpiralPoints()
//     {
//         var spiral = GetSpiralPoints(spiralConstantA, spiralConstantK, coreRadius, galaxyRadius);
//         var spiralPoints = new List<Vector2>();
//         foreach (var v in spiral)
//         {
//             spiralPoints.Add(new Vector2(v.x, v.y));
//             spiralPoints.Add(new Vector2(-v.x, -v.y));
//         }
//
//         return spiralPoints;
//     }
//     
//     // This should be handled somewhere else
//     private void Update()
//     {
//         PlaceReticule();
//     }
//
//     public Vector3 StarPointById(int id = -1)
//     {
//         if (id == -1) id = _clickedId;
//         return id == -1 ? Vector3.zero : _delPoints[id].ToVector3();
//     }
//
//     public GameObject StarDataByPoint(Vector3 point)
//     {
//         return _starContainers[point];
//     }
//
//     private void PlaceReticule()
//     {
//         if (_camera is null) return;
//         if (_clickedId == -1) return;
//
//         var curStarPoint = StarPointById(_clickedId);
//         var screenPos = _camera.WorldToScreenPoint(curStarPoint);
//         _rt.transform.position = new Vector3(screenPos.x, screenPos.y, 0.0f);
//     }
//     
//     // Return points that define a spiral.
//     private static IEnumerable<Vector2> GetSpiralPoints(double a, double k, float minRadius, float maxRadius)
//     {
//         var minTheta = Math.Log(0.1 / a) / k;
//         const float dTheta = (float)(5 * Math.PI / 180);    // Five degrees.
//         var points = new List<Vector2>();
//         for (var theta = minTheta; ; theta += dTheta)
//         {
//             var r = a * Math.Exp(k * theta);
//             var r2 = (a - 0.49) * Math.Exp(k * theta);
//
//             if (r >= minRadius && r <= maxRadius)
//             {
//                 var x = r * Math.Cos(theta);
//                 var y = r * Math.Sin(theta);
//                 points.Add(new Vector2((float) x, (float) y));
//             }
//
//             if (r2 >= minRadius && r2 <= maxRadius)
//             {
//                 var x2 = r2 * Math.Cos(theta);
//                 var y2 = r2 * Math.Sin(theta);
//                 points.Add(new Vector2((float) x2, (float) y2));
//             }
//
//             // If we have gone far enough, stop.
//             if (r > maxRadius && r2 > maxRadius) break;
//         }
//         return points;
//     }
//
//     private void OnCellClick(int id)
//     {
//         Debug.Log("Clicked Cell "+id);
//         _clickedId = id;
//         _clickCallback?.Invoke(id);
//     }
//
//     public delegate void ClickCallback(int id);
//     private ClickCallback _clickCallback;
//     public void setupOnClickStarCallback(ClickCallback callback)
//     {
//         _clickCallback = callback;
//     }
// }
