using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LuaGameUI : MonoBehaviour
{
    [SerializeField] Button attackButton;
    [SerializeField] Button skillButton;
    [SerializeField] Button resetButton;
    [SerializeField] Text playerLevelText;
    [SerializeField] Text playerAttackText;
    [SerializeField] Text goldText;
    [SerializeField] Text enemyNameText;
    [SerializeField] Text enemyHpText;
    [SerializeField] Text playTimeText;
    [SerializeField] Text messageText;
    [SerializeField] Text inventoryText;
    [SerializeField] Text skillCooldownText;
    [SerializeField] Slider enemyHpSlider;

    UnityAction _attackAction;
    UnityAction _skillAction;
    UnityAction _resetAction;
    UiTexts _uiTexts;

    public struct UiTexts
    {
        public string PlayerLevelLabel;
        public string PlayerAttackLabel;
        public string GoldLabel;
        public string EnemyLabel;
        public string HpLabel;
        public string PlayTimeLabel;
        public string MessageLabel;
        public string InventoryLabel;
        public string AttackButtonLabel;
        public string SkillButtonLabel;
        public string ResetButtonLabel;
        public string SkillCooldownLabel;
        public string SecondsSuffix;
    }

    public bool HasBindings()
    {
        return attackButton != null
            || skillButton != null
            || resetButton != null
            || playerLevelText != null
            || playerAttackText != null
            || goldText != null
            || enemyNameText != null
            || enemyHpText != null
            || playTimeText != null
            || messageText != null
            || inventoryText != null
            || skillCooldownText != null
            || enemyHpSlider != null;
    }

    public void BindActions(UnityAction attackAction, UnityAction skillAction, UnityAction resetAction)
    {
        UnbindActions();

        _attackAction = attackAction;
        _skillAction = skillAction;
        _resetAction = resetAction;

        if (attackButton != null && _attackAction != null)
            attackButton.onClick.AddListener(_attackAction);

        if (skillButton != null && _skillAction != null)
            skillButton.onClick.AddListener(_skillAction);

        if (resetButton != null && _resetAction != null)
            resetButton.onClick.AddListener(_resetAction);
    }

    public void UnbindActions()
    {
        if (attackButton != null && _attackAction != null)
            attackButton.onClick.RemoveListener(_attackAction);

        if (skillButton != null && _skillAction != null)
            skillButton.onClick.RemoveListener(_skillAction);

        if (resetButton != null && _resetAction != null)
            resetButton.onClick.RemoveListener(_resetAction);

        _attackAction = null;
        _skillAction = null;
        _resetAction = null;
    }

    public void ApplyTexts(UiTexts uiTexts)
    {
        _uiTexts = uiTexts;

        SetButtonLabel(attackButton, _uiTexts.AttackButtonLabel);
        SetButtonLabel(skillButton, _uiTexts.SkillButtonLabel);
        SetButtonLabel(resetButton, _uiTexts.ResetButtonLabel);
    }

    public void ApplyFont(Font font)
    {
        if (font == null)
            return;

        ApplyTextFont(playerLevelText, font);
        ApplyTextFont(playerAttackText, font);
        ApplyTextFont(goldText, font);
        ApplyTextFont(enemyNameText, font);
        ApplyTextFont(enemyHpText, font);
        ApplyTextFont(playTimeText, font);
        ApplyTextFont(messageText, font);
        ApplyTextFont(inventoryText, font);
        ApplyTextFont(skillCooldownText, font);
        SetButtonFont(attackButton, font);
        SetButtonFont(skillButton, font);
        SetButtonFont(resetButton, font);
    }

    public void RefreshState(int playerLevel, int playerAttack, int gold, string enemyName, int enemyHp, int enemyMaxHp, string message, string inventorySummary, float playTime, bool skillReady, float skillCooldownRemaining)
    {
        if (playerLevelText != null)
            playerLevelText.text = _uiTexts.PlayerLevelLabel + ": " + playerLevel;

        if (playerAttackText != null)
            playerAttackText.text = _uiTexts.PlayerAttackLabel + ": " + playerAttack;

        if (goldText != null)
            goldText.text = _uiTexts.GoldLabel + ": " + gold;

        if (enemyNameText != null)
            enemyNameText.text = _uiTexts.EnemyLabel + ": " + enemyName;

        if (enemyHpText != null)
            enemyHpText.text = _uiTexts.HpLabel + ": " + enemyHp + " / " + enemyMaxHp;

        if (playTimeText != null)
            playTimeText.text = _uiTexts.PlayTimeLabel + ": " + playTime.ToString("F1") + " " + _uiTexts.SecondsSuffix;

        if (messageText != null)
            messageText.text = _uiTexts.MessageLabel + ": " + message;

        if (inventoryText != null)
            inventoryText.text = _uiTexts.InventoryLabel + ": " + inventorySummary;

        if (skillCooldownText != null)
        {
            skillCooldownText.text = _uiTexts.SkillCooldownLabel + ": " + (skillReady ? _uiTexts.SkillButtonLabel + " OK" : skillCooldownRemaining.ToString("F1") + " " + _uiTexts.SecondsSuffix);
        }

        if (skillButton != null)
            skillButton.interactable = skillReady;

        if (enemyHpSlider != null)
        {
            enemyHpSlider.minValue = 0f;
            enemyHpSlider.maxValue = Mathf.Max(1, enemyMaxHp);
            enemyHpSlider.value = enemyHp;
        }
    }

    void SetButtonLabel(Button button, string label)
    {
        if (button == null)
            return;

        var text = button.GetComponentInChildren<Text>();
        if (text != null && !string.IsNullOrEmpty(label))
            text.text = label;
    }

    void SetButtonFont(Button button, Font font)
    {
        if (button == null || font == null)
            return;

        var text = button.GetComponentInChildren<Text>();
        ApplyTextFont(text, font);
    }

    void ApplyTextFont(Text text, Font font)
    {
        if (text != null)
            text.font = font;
    }
}
