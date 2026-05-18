using BepInEx;
using UnityEngine;
using UnityEngine.UI;

[BepInPlugin("com.yourname.wobblyac", "Wobbly AC", "1.0")]
public class WobblyAC : BaseUnityPlugin
{
    private GameObject canvas;
    private bool visible = true;

    void Start()
    {
        CreateUI();
        Logger.LogInfo("UI created");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            visible = !visible;
            canvas.SetActive(visible);
        }
    }

    void CreateUI()
    {
        // ===== Canvas =====
        canvas = new GameObject("AC_Canvas");

        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // ===== Panel =====
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvas.transform, false);

        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.75f);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 250);
        panelRect.anchoredPosition = Vector2.zero;

        // ===== Title =====
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);

        Text title = titleObj.AddComponent<Text>();
        title.text = "Wobbly AC Menu";
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 26;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;

        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(380, 40);
        titleRect.anchoredPosition = new Vector2(0, 90);

        // ===== Status Text =====
        GameObject statusObj = new GameObject("Status");
        statusObj.transform.SetParent(panel.transform, false);

        Text status = statusObj.AddComponent<Text>();
        status.text = "Mod loaded successfully";
        status.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        status.fontSize = 18;
        status.alignment = TextAnchor.MiddleCenter;
        status.color = Color.green;

        RectTransform statusRect = status.GetComponent<RectTransform>();
        statusRect.sizeDelta = new Vector2(380, 40);
        statusRect.anchoredPosition = new Vector2(0, 20);

        // ===== Button =====
        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(panel.transform, false);

        Image buttonImg = buttonObj.AddComponent<Image>();
        buttonImg.color = Color.gray;

        Button button = buttonObj.AddComponent<Button>();

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(160, 40);
        buttonRect.anchoredPosition = new Vector2(0, -70);

        // Button text
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(buttonObj.transform, false);

        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "Click Me";
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.sizeDelta = new Vector2(160, 40);
        btnTextRect.anchoredPosition = Vector2.zero;

        // Button action
        button.onClick.AddListener(() =>
        {
            status.text = "Button clicked!";
            status.color = Color.yellow;
        });
    }
}