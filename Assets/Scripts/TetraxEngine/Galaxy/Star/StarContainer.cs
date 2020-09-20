using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using UnityEngine;
using Canvas = TetraxEngine.UI.Canvas;

namespace TetraxEngine.Galaxy.Star
{
    public class StarContainer : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
        public GameObject star;
        public GameObject starPlane;

        [SerializeField] private Vector3 starLocation;
        private GameObject _star;
        public IVoronoiCell Cell;
        private Canvas _canvas;
        private Game _game;

        private void Start()
        {
            _canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
            _game = GameObject.FindGameObjectWithTag("MainGame").GetComponent<Game>();
        }

        public GameObject StarInfo()
        {
            return _star;
        }
        
        public StarContainer Setup(Vector3 localStarLocation, IVoronoiCell localCell)
        {
            starLocation = localStarLocation;
            Cell = localCell;

            SetupStar();
            SetupCell();

            return this;
        }

        private void SetupStar()
        {
            _star = Instantiate(star, starLocation, Quaternion.identity);
            _star.GetComponent<Star>().transform.SetParent(transform);
        
            var component = _star.GetComponent<Star>();
            var starRadius = component.radius;
            var starLuminosity = component.luminosity;
            var starColor = component.color;
        
            // Set the star Radius
            var radius = TetraxUtils.Map(starRadius, 0.00001f, 10f, 1.5f, 6f);
            _star.transform.localScale = new Vector3(radius, radius, radius);
        
            // Calculate the star and dust cloud luminosity 
            var luminosity = TetraxUtils.Map(starLuminosity, 0.08f, 1100000f, 500f, 5000f);
        
            // Tint the star's material
            var starMat = _star.gameObject.GetComponent<MeshRenderer>().material;
            starMat.EnableKeyword("_EMISSION");
            starMat.SetColor(EmissionColor, starColor * luminosity);
        }

        private void SetupCell()
        {
            var curStarCell = Instantiate(starPlane, starLocation, Quaternion.identity).GetComponent<PlaneFromPoly>();
            curStarCell.SetPolyMesh(Cell.Points.ToVectors3(), OnPolyClick);
            curStarCell.transform.SetParent(transform);
        }

        private void OnPolyClick()
        {
            _canvas.SelectedStarLocation = starLocation;
            _game.ShowInfoPanel(starLocation);
        }
    }
}
