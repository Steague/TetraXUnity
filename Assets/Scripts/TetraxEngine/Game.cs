using TetraxEngine.Data;
using TetraxEngine.Galaxy.Star;
using TetraxEngine.UI;
using UnityEngine;
using Canvas = TetraxEngine.UI.Canvas;

namespace TetraxEngine
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameObject saveData;
        public Galaxy.Galaxy galaxy;
        private Canvas _canvas;
        private bool _showInfoPanel;
        private GameObject _infoPanel;

        private void Start()
        {
            _canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        }

        public void SaveStars()
        {
            saveData.GetComponent<SaveGalaxy>().AddGalaxy(galaxy.StarContainers);
            saveData.GetComponent<SaveGalaxy>().Save();
        }

        public void LoadStars()
        {
            
        }

        public void ShowInfoPanel(Vector3 starLocation)
        {
            if (_infoPanel != null) Destroy(_infoPanel);

            _infoPanel = _canvas.UiPanel();
                // RectTransform.Edge.Left,
                // 10,
                // 256,
                // RectTransform.Edge.Bottom,
                // 10,
                // 256);

            var starInfo = galaxy.StarContainers[starLocation].StarInfo().GetComponent<Star>();
            
            var crtPanel = _infoPanel.GetComponent<CrtPanel>();

            if (starInfo == null)
            {
                Debug.Log("Something wrong with the star info Object. ("+starLocation+")");
                return;
            }

            crtPanel.SetContent(
                "Type: "+starInfo.template.StarType+"\n"+
                "Radius: "+starInfo.radius+"\n"+
                "Mass: "+starInfo.mass+"\n"+
                "Temp: "+starInfo.temp+"\n"+
                "Luminosity: "+starInfo.luminosity);
        }
    }
}