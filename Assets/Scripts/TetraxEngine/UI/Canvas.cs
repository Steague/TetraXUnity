using UnityEngine;

namespace TetraxEngine.UI
{
    public class Canvas : MonoBehaviour
    {
        [SerializeField] private GameObject crtPanel;
        [SerializeField] private GameObject reticule;

        private Vector3 _selectedStarLocation = Vector3.zero - new Vector3(10000,10000, 0);
        public Vector3 SelectedStarLocation
        {
            get => _selectedStarLocation;
            set
            {
                _reticuleVisible = true;
                _selectedStarLocation = value;
            }
        }
        private bool _reticuleVisible;
        
        private Canvas _canvas;
        private RectTransform _rt;
        private Camera _camera;
    
        private void Start()
        {
            _canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
            _camera = Camera.main;

            InitReticule();
        }

        private void InitReticule()
        {
            reticule = Instantiate(reticule, _selectedStarLocation, Quaternion.identity);
            reticule.transform.SetParent(_canvas.transform);
            _rt = reticule.GetComponent<RectTransform>();
            _rt.SetParent(_canvas.transform);
        }
        
        private void Update()
        {
            PlaceReticule();
        }

        private void PlaceReticule()
        {
            if (_reticuleVisible == false) return;
            var screenPos = _camera.WorldToScreenPoint(_selectedStarLocation);
            _rt.transform.position = new Vector3(screenPos.x, screenPos.y, 0.0f);
        }

        public GameObject UiPanel(
            RectTransform.Edge e1 = RectTransform.Edge.Left,
            int i1 = 10,
            int s1 = 256,
            RectTransform.Edge e2 = RectTransform.Edge.Bottom,
            int i2 = 10,
            int s2 = 256)
        {
            var panel = Instantiate(crtPanel, Vector3.zero, Quaternion.identity);
            panel.transform.SetParent(_canvas.transform);
            
            var rt = panel.GetComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(e1, i1, s1);
            rt.SetInsetAndSizeFromParentEdge(e2, i2, s2);

            return panel;
        }
    }
}
