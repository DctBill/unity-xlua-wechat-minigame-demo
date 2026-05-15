using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuaWorldHud : MonoBehaviour
{
    [SerializeField] RectTransform hudRoot;
    [SerializeField] RectTransform floatingTextRoot;
    [SerializeField] Slider enemyHpSlider;
    [SerializeField] Text enemyNameText;
    [SerializeField] Text enemyHpText;
    [SerializeField] Text floatingTextTemplate;
    [SerializeField] Camera worldCamera;
    [SerializeField] Vector3 worldOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] float floatingTextDuration = 0.8f;
    [SerializeField] float floatingTextSpeed = 36f;

    readonly List<FloatingTextRuntime> _floatingTexts = new List<FloatingTextRuntime>();
    Transform _target;
    Canvas _canvas;
    RectTransform _canvasRect;
    string _enemyLabel = "敌人";
    string _hpLabel = "生命";

    class FloatingTextRuntime
    {
        public Text text;
        public float remainingTime;
        public Color baseColor;
    }

    public bool HasBindings()
    {
        return hudRoot != null || enemyHpSlider != null || enemyNameText != null || enemyHpText != null;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        CacheCanvas();
        UpdateHudPosition();
    }

    public void RefreshState(string enemyName, int enemyHp, int enemyMaxHp)
    {
        if (enemyNameText != null)
            enemyNameText.text = _enemyLabel + ": " + enemyName;

        if (enemyHpText != null)
            enemyHpText.text = _hpLabel + ": " + enemyHp + " / " + enemyMaxHp;

        if (enemyHpSlider != null)
        {
            enemyHpSlider.minValue = 0f;
            enemyHpSlider.maxValue = Mathf.Max(1, enemyMaxHp);
            enemyHpSlider.value = enemyHp;
        }
    }

    public void ApplyTexts(string enemyLabel, string hpLabel)
    {
        if (!string.IsNullOrEmpty(enemyLabel))
            _enemyLabel = enemyLabel;

        if (!string.IsNullOrEmpty(hpLabel))
            _hpLabel = hpLabel;
    }

    public void ApplyFont(Font font)
    {
        if (font == null)
            return;

        ApplyTextFont(enemyNameText, font);
        ApplyTextFont(enemyHpText, font);
        ApplyTextFont(floatingTextTemplate, font);

        for (int i = 0; i < _floatingTexts.Count; i++)
        {
            ApplyTextFont(_floatingTexts[i].text, font);
        }
    }

    public void SpawnFloatingText(string value, Color color)
    {
        if (floatingTextTemplate == null)
            return;

        var parent = floatingTextRoot != null ? floatingTextRoot : hudRoot;
        if (parent == null)
            return;

        var textInstance = Instantiate(floatingTextTemplate, parent);
        textInstance.gameObject.SetActive(true);
        textInstance.text = value;
        textInstance.color = color;
        textInstance.rectTransform.anchoredPosition = Vector2.zero;

        var preferredWidth = Mathf.Max(textInstance.preferredWidth + 20f, textInstance.rectTransform.sizeDelta.x);
        var preferredHeight = Mathf.Max(textInstance.preferredHeight + 10f, textInstance.rectTransform.sizeDelta.y);
        textInstance.rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);

        _floatingTexts.Add(new FloatingTextRuntime
        {
            text = textInstance,
            remainingTime = floatingTextDuration,
            baseColor = color
        });
    }

    public void ClearFloatingTexts()
    {
        for (int i = 0; i < _floatingTexts.Count; i++)
        {
            if (_floatingTexts[i].text != null)
                Destroy(_floatingTexts[i].text.gameObject);
        }

        _floatingTexts.Clear();
    }

    void Awake()
    {
        CacheCanvas();
        if (floatingTextTemplate != null)
            floatingTextTemplate.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        UpdateHudPosition();
        UpdateFloatingTexts(Time.deltaTime);
    }

    void OnDisable()
    {
        ClearFloatingTexts();
    }

    void CacheCanvas()
    {
        if (hudRoot == null)
            return;

        if (_canvas == null)
            _canvas = hudRoot.GetComponentInParent<Canvas>();

        if (_canvas != null)
            _canvasRect = _canvas.transform as RectTransform;

        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    void UpdateHudPosition()
    {
        if (hudRoot == null || _target == null)
            return;

        CacheCanvas();
        if (_canvasRect == null || worldCamera == null)
            return;

        Vector3 screenPos = worldCamera.WorldToScreenPoint(_target.position + worldOffset);
        bool visible = screenPos.z > 0f;
        if (hudRoot.gameObject.activeSelf != visible)
            hudRoot.gameObject.SetActive(visible);

        if (!visible)
            return;

        Camera uiCamera = _canvas != null && _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, uiCamera, out var localPoint);
        hudRoot.anchoredPosition = localPoint;
    }

    void UpdateFloatingTexts(float dt)
    {
        for (int i = _floatingTexts.Count - 1; i >= 0; i--)
        {
            var item = _floatingTexts[i];
            if (item.text == null)
            {
                _floatingTexts.RemoveAt(i);
                continue;
            }

            item.remainingTime -= dt;
            item.text.rectTransform.anchoredPosition += Vector2.up * (floatingTextSpeed * dt);
            float alpha = Mathf.Clamp01(item.remainingTime / floatingTextDuration);
            item.text.color = new Color(item.baseColor.r, item.baseColor.g, item.baseColor.b, alpha);

            if (item.remainingTime <= 0f)
            {
                Destroy(item.text.gameObject);
                _floatingTexts.RemoveAt(i);
            }
        }
    }

    void ApplyTextFont(Text text, Font font)
    {
        if (text != null)
            text.font = font;
    }
}
