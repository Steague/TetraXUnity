using TMPro;
using UnityEngine;

namespace TetraxEngine.UI
{
    public class CrtPanel : MonoBehaviour
    {
        [SerializeField] private GameObject textInfo;
        private Canvas _canvas;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
            transform.SetParent(_canvas.transform);

            var textInfoTransform = transform.FirstOrDefault(x => x.CompareTag("InfoText"));
            if (textInfoTransform != null)
            {
                textInfo = textInfo.gameObject;
                // var list = textInfo.GetComponents(typeof(Component));
                // foreach (var t in list)
                // {
                //     Debug.Log(t.ToString());
                // }
            }
        }

        public void SetContent(string content)
        {
            textInfo.GetComponent<TextMeshProUGUI>().SetText(content);
            // var infoTextComponent = transform.Find("Content/InfoText");//.GetComponent<TextMeshPro>();
            // //infoTextComponent.SetText(content);
            // Debug.Log(""+infoTextComponent.GetComponent<GameObject>());
            //textInfo.GetComponent<TextMeshPro>().SetText(content);
            //gameObject.GetComponent<GameObject>().
            // var found = new List<GameObject>(GameObject.FindGameObjectsWithTag("InfoText")).Find(g => g.transform.IsChildOf(panel.transform));
            // var panelComponent = panel.GetComponent<CrtPanel>();
            // var gameObjectSearcher = panelComponent.GetComponent<GameObjectSearcher>();
            // gameObjectSearcher.tag = "InfoText";
            //
            // if (gameObjectSearcher.actors.Count == 1)
            // {
            //     gameObjectSearcher.actors[0]
            //         .GetComponent<TextMeshPro>()
            //         .SetText(content);
            // }
            // else
            // {
            //     gameObjectSearcher.GetComponent<GameObjectSearcher>().actors.Count
            //     Debug.Log("Actors: "+gameObjectSearcher.actors.Count);
            // }
        }
    }
}
