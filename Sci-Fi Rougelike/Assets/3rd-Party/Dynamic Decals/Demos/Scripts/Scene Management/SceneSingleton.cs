#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

namespace LlockhamIndustries.Misc
{
    public class SceneSingleton : MonoBehaviour
    {
        private static SceneSingleton system;

        //Backing fields
        private GameObject canvas;

        private readonly SceneMenu menu = new SceneMenu();

        //Multi-Scene Singleton 
        public static SceneSingleton System
        {
            get
            {
                if (system == null)
                {
                    var go = new GameObject("Scene Manager");
                    go.AddComponent<SceneSingleton>();
                }
                return system;
            }
        }

        //Generic methods
        private void OnEnable()
        {
            if (system == null)
                system = this;
            else if (this != system)
                Destroy(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OpenMenu();
        }

        public void OpenMenu()
        {
            //Make sure we have a valid canvas
            if (canvas == null) GrabCanvas();

            //Open our menu on the canvas
            menu.Open(System.canvas);
        }

        public void CloseMenu()
        {
            menu.Close();
        }

        //Utility
        private void GrabCanvas()
        {
            //Search for a suitable canvas
            Object[] Canvases = FindObjectsOfType<Canvas>();
            for (var i = 0; i < Canvases.Length; i++)
            {
                var c = (Canvas)Canvases[i];
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas = c.gameObject;
                    break;
                }
            }

            //No suitable canvas found, create one
            if (canvas == null)
            {
                canvas = new GameObject("Canvas");
                canvas.AddComponent<Canvas>();
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
            }
        }
    }

    public class SceneMenu
    {
        private readonly int buttonCount = 5;
        private GameObject window;

        //Primary Methods
        public void Open(GameObject Canvas)
        {
            if (window == null) GenerateMenu(Canvas);
            window.SetActive(true);
        }

        public void Close()
        {
            if (window != null && window.activeInHierarchy) window.SetActive(false);
        }

        //Menu Generation
        public void GenerateMenu(GameObject Canvas)
        {
            //Create primary window
            window = new GameObject("Scene Menu");
            window.transform.SetParent(Canvas.transform, false);

            //Setup transform
            var rect = window.AddComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.offsetMax = new Vector2(50, 0);
            rect.offsetMin = new Vector2(-50, -(buttonCount * 20) + 8);

            //Setup image
            var image = window.AddComponent<Image>();
            image.color = new Color(0.28f, 0.28f, 0.28f, 1);

            //Generate load buttons
            GenerateButton("Scene 1", 1, delegate { LoadScene(0); });
            GenerateButton("Scene 2", 2, delegate { LoadScene(1); });
            GenerateButton("Scene 3", 3, delegate { LoadScene(2); });
            GenerateButton("Scene 4", 4, delegate { LoadScene(3); });

            //Generate cancel button
            GenerateButton("Cancel", 5, delegate { Close(); });
        }

        public void GenerateButton(string Text, int Index, UnityAction Action)
        {
            //Setup menu
            var bo = new GameObject("Scene Button");
            bo.transform.SetParent(window.transform, false);

            //Setup transform
            var rect = bo.AddComponent<RectTransform>();
            var position = 1 - ((float)Index - 1) / buttonCount;
            rect.anchorMax = new Vector2(0.5f, position);
            rect.anchorMin = new Vector2(0.5f, position);
            rect.offsetMax = new Vector2(40, 0);
            rect.offsetMin = new Vector2(-40, -16);

            //Setup image
            var image = bo.AddComponent<Image>();
            image.color = new Color(0.22f, 0.22f, 0.22f, 1);

            //Setup button
            var button = bo.AddComponent<Button>();
            button.onClick.AddListener(Action);

            //Setup menu text
            var to = new GameObject("Button Text");
            to.transform.SetParent(rect, false);

            //Setup text transform
            var textRect = to.AddComponent<RectTransform>();
            textRect.anchorMax = new Vector2(0, 0);
            textRect.anchorMin = new Vector2(0, 0);
            textRect.offsetMax = new Vector2(0, 0);
            textRect.offsetMin = new Vector2(0, 0);
        }

        //Menu funtion
        public void LoadScene(int Index)
        {
            SceneManager.LoadScene(Index, LoadSceneMode.Single);
        }
    }
}