using UnityEngine;

public class LuaGameEntry : MonoBehaviour
{
    [SerializeField] Transform enemyVisual;
    [Header("Optional Canvas UI")]
    [SerializeField] LuaGameUI canvasUi;
    [SerializeField] LuaWorldHud worldHud;
    [Header("WeChat Font")]
    [SerializeField] bool useWXFont = true;
    [SerializeField] string wxFallbackFontPath = WXFontManager.DefaultFallbackFontPath;
    [SerializeField] Font editorFallbackFont;
    [SerializeField] bool hideOnGuiWhenCanvasReady = true;
    [SerializeField] float hitFlashDuration = 0.12f;
    [SerializeField] float hitScaleMultiplier = 1.12f;
    [SerializeField] float defeatFlashDuration = 0.25f;
    [SerializeField] float shakeDuration = 0.1f;
    [SerializeField] float shakeStrength = 0.08f;
    [SerializeField] Color floatingDamageColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] Color floatingCritColor = new Color(1f, 0.6f, 0.1f, 1f);
    [SerializeField] Color floatingSkillColor = new Color(0.35f, 0.8f, 1f, 1f);
    [SerializeField] Color floatingRewardColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] Color floatingDropColor = new Color(0.8f, 1f, 0.75f, 1f);
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color hitColor = new Color(1f, 0.45f, 0.45f, 1f);
    [SerializeField] Color defeatColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    LuaManager _luaManager;
    ILuaGameApp _gameApp;
    float _elapsed;
    float _visualTimer;
    float _currentFlashDuration;
    float _shakeTimer;
    Vector3 _enemyVisualBaseScale;
    Vector3 _enemyVisualBaseLocalPosition;
    Renderer _enemyRenderer;
    ILuaGameTexts _uiTexts;
    Font _runtimeUiFont;
    Color _enemyBaseColor;

    struct GameState
    {
        public int playerLevel;
        public int playerAttack;
        public int gold;
        public int enemyId;
        public string enemyName;
        public int enemyHp;
        public int enemyMaxHp;
        public int enemyGold;
        public Color enemyColor;
        public string message;
        public string inventorySummary;
        public string lastDropText;
        public float playTime;
        public int lastDamage;
        public bool lastAttackWasCrit;
        public bool lastAttackWasSkill;
        public float skillCooldownRemaining;
        public bool skillReady;
    }

    GameState _state;
    GameState _previousState;

    string TitleLabel => _uiTexts != null ? _uiTexts.Title : "Lua 学习案例：打史莱姆";
    string PlayerLevelLabel => _uiTexts != null ? _uiTexts.PlayerLevelLabel : "玩家等级";
    string PlayerAttackLabel => _uiTexts != null ? _uiTexts.PlayerAttackLabel : "玩家攻击";
    string GoldLabel => _uiTexts != null ? _uiTexts.GoldLabel : "金币";
    string EnemyLabel => _uiTexts != null ? _uiTexts.EnemyLabel : "敌人";
    string HpLabel => _uiTexts != null ? _uiTexts.HpLabel : "生命";
    string PlayTimeLabel => _uiTexts != null ? _uiTexts.PlayTimeLabel : "游玩时长";
    string MessageLabel => _uiTexts != null ? _uiTexts.MessageLabel : "消息";
    string InventoryLabel => _uiTexts != null ? _uiTexts.InventoryLabel : "材料背包";
    string AttackButtonLabel => _uiTexts != null ? _uiTexts.AttackButtonLabel : "攻击一次";
    string SkillButtonLabel => _uiTexts != null ? _uiTexts.SkillButtonLabel : "强力一击";
    string ResetButtonLabel => _uiTexts != null ? _uiTexts.ResetButtonLabel : "重新开始";
    string SkillCooldownLabel => _uiTexts != null ? _uiTexts.SkillCooldownLabel : "技能冷却";
    string SecondsSuffix => _uiTexts != null ? _uiTexts.SecondsSuffix : "秒";

    void Start()
    {
        _luaManager = new LuaManager();
        _luaManager.Initialize();
        _gameApp = _luaManager.GameApp;
        _uiTexts = _gameApp != null ? _gameApp.GetUiTexts() : null;
        if (_gameApp != null)
            _gameApp.LoadProgress();
        if (canvasUi != null)
        {
            canvasUi.BindActions(AttackOnce, UseSkill, ResetGame);
            canvasUi.ApplyTexts(new LuaGameUI.UiTexts
            {
                PlayerLevelLabel = PlayerLevelLabel,
                PlayerAttackLabel = PlayerAttackLabel,
                GoldLabel = GoldLabel,
                EnemyLabel = EnemyLabel,
                HpLabel = HpLabel,
                PlayTimeLabel = PlayTimeLabel,
                MessageLabel = MessageLabel,
                InventoryLabel = InventoryLabel,
                AttackButtonLabel = AttackButtonLabel,
                SkillButtonLabel = SkillButtonLabel,
                ResetButtonLabel = ResetButtonLabel,
                SkillCooldownLabel = SkillCooldownLabel,
                SecondsSuffix = SecondsSuffix
            });
        }
        if (worldHud != null)
        {
            worldHud.ApplyTexts(EnemyLabel, HpLabel);
            worldHud.SetTarget(enemyVisual);
        }
        RequestRuntimeFont();
        CacheEnemyVisual();
        RefreshState();
        _previousState = _state;
    }

    void Update()
    {
        if (_luaManager == null || _gameApp == null)
            return;

        _elapsed += Time.deltaTime;
        UpdateEnemyVisual(Time.deltaTime);
        _gameApp.Tick(Time.deltaTime);
        _luaManager.Tick();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackOnce();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }

        if (_elapsed >= 1f)
        {
            RefreshState();
            _elapsed = 0f;
        }
    }

    void OnGUI()
    {
        if (hideOnGuiWhenCanvasReady && ((canvasUi != null && canvasUi.HasBindings()) || (worldHud != null && worldHud.HasBindings())))
            return;

        if (_runtimeUiFont != null)
            GUI.skin.font = _runtimeUiFont;

        GUILayout.BeginArea(new Rect(20f, 20f, 320f, 260f), GUI.skin.box);
        GUILayout.Label(TitleLabel);
        GUILayout.Space(8f);
        GUILayout.Label(PlayerLevelLabel + ": " + _state.playerLevel);
        GUILayout.Label(PlayerAttackLabel + ": " + _state.playerAttack);
        GUILayout.Label(GoldLabel + ": " + _state.gold);
        GUILayout.Space(8f);
        GUILayout.Label(EnemyLabel + ": " + _state.enemyName);
        GUILayout.Label(HpLabel + ": " + _state.enemyHp + " / " + _state.enemyMaxHp);
        GUILayout.Label(PlayTimeLabel + ": " + _state.playTime.ToString("F1") + " " + SecondsSuffix);
        GUILayout.Label(InventoryLabel + ": " + _state.inventorySummary);
        GUILayout.Label(SkillCooldownLabel + ": " + (_state.skillReady ? SkillButtonLabel + " 就绪" : _state.skillCooldownRemaining.ToString("F1") + " " + SecondsSuffix));
        GUILayout.Space(8f);
        GUILayout.Label(MessageLabel + ": " + _state.message);
        GUILayout.Space(12f);

        if (GUILayout.Button(AttackButtonLabel, GUILayout.Height(40f)))
        {
            AttackOnce();
        }

        GUI.enabled = _state.skillReady;
        if (GUILayout.Button(SkillButtonLabel, GUILayout.Height(36f)))
        {
            UseSkill();
        }
        GUI.enabled = true;

        if (GUILayout.Button(ResetButtonLabel, GUILayout.Height(32f)))
        {
            ResetGame();
        }

        GUILayout.EndArea();
    }

    void RequestRuntimeFont()
    {
        if (!useWXFont)
        {
            ApplyRuntimeFont(editorFallbackFont);
            return;
        }

        WXFontManager.LoadFont(wxFallbackFontPath, editorFallbackFont, ApplyRuntimeFont);
    }

    void ApplyRuntimeFont(Font font)
    {
        if (font == null)
            return;

        _runtimeUiFont = font;

        if (canvasUi != null)
            canvasUi.ApplyFont(font);

        if (worldHud != null)
            worldHud.ApplyFont(font);
    }

    void PlayFloatingTextFeedback()
    {
        if (worldHud == null)
            return;

        if (_previousState.enemyHp > _state.enemyHp)
        {
            var damageColor = _state.lastAttackWasSkill ? floatingSkillColor : (_state.lastAttackWasCrit ? floatingCritColor : floatingDamageColor);
            var damageLabel = _state.lastAttackWasSkill ? "SKILL -" + _state.lastDamage : (_state.lastAttackWasCrit ? "CRIT -" + _state.lastDamage : "-" + _state.lastDamage);
            worldHud.SpawnFloatingText(damageLabel, damageColor);
        }

        if (_state.gold > _previousState.gold)
        {
            worldHud.SpawnFloatingText("+1 Gold", floatingRewardColor);
        }

        if (!string.IsNullOrEmpty(_state.lastDropText) && _state.lastDropText != _previousState.lastDropText)
        {
            worldHud.SpawnFloatingText(_state.lastDropText, floatingDropColor);
        }
    }

    void AttackOnce()
    {
        if (_gameApp == null)
            return;

        _previousState = _state;
        _gameApp.AttackEnemy();
        RefreshState();
        PlayVisualFeedback();
        PlayFloatingTextFeedback();
    }

    void UseSkill()
    {
        if (_gameApp == null)
            return;

        _previousState = _state;
        _gameApp.UseSkill();
        RefreshState();
        PlayVisualFeedback();
        PlayFloatingTextFeedback();
    }

    void ResetGame()
    {
        if (_gameApp == null)
            return;

        _gameApp.ResetGame();
        RefreshState();
        ResetEnemyVisual();
    }

    void CacheEnemyVisual()
    {
        if (enemyVisual == null)
            return;

        _enemyVisualBaseScale = enemyVisual.localScale;
        _enemyVisualBaseLocalPosition = enemyVisual.localPosition;
        _enemyRenderer = enemyVisual.GetComponentInChildren<Renderer>();
        _enemyBaseColor = normalColor;
        ApplyEnemyColor(_enemyBaseColor);
    }

    void PlayVisualFeedback()
    {
        if (enemyVisual == null)
            return;

        bool defeated = _previousState.enemyHp > 0 && _state.enemyHp == _state.enemyMaxHp;
        _currentFlashDuration = defeated ? defeatFlashDuration : hitFlashDuration;
        _visualTimer = _currentFlashDuration;
        _shakeTimer = defeated ? shakeDuration * 1.5f : shakeDuration;

        enemyVisual.localScale = _enemyVisualBaseScale * (defeated ? hitScaleMultiplier * 1.2f : hitScaleMultiplier);
        ApplyEnemyColor(defeated ? defeatColor : hitColor);
    }

    void UpdateEnemyVisual(float dt)
    {
        if (enemyVisual == null)
            return;

        if (_visualTimer > 0f)
        {
            _visualTimer -= dt;
            if (_visualTimer <= 0f)
            {
                enemyVisual.localScale = _enemyVisualBaseScale;
                ApplyEnemyColor(_enemyBaseColor);
            }
        }

        if (_shakeTimer > 0f)
        {
            _shakeTimer -= dt;
            float strength = shakeStrength * Mathf.Clamp01(_shakeTimer / Mathf.Max(0.0001f, shakeDuration));
            enemyVisual.localPosition = _enemyVisualBaseLocalPosition + new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0f);

            if (_shakeTimer <= 0f)
            {
                enemyVisual.localPosition = _enemyVisualBaseLocalPosition;
            }
        }
    }

    void ResetEnemyVisual()
    {
        if (enemyVisual == null)
            return;

        enemyVisual.localScale = _enemyVisualBaseScale;
        enemyVisual.localPosition = _enemyVisualBaseLocalPosition;
        ApplyEnemyColor(_enemyBaseColor);
        _visualTimer = 0f;
        _shakeTimer = 0f;
        if (worldHud != null)
            worldHud.ClearFloatingTexts();
    }

    void ApplyEnemyColor(Color color)
    {
        if (_enemyRenderer == null)
            return;

        _enemyRenderer.material.color = color;
    }

    void RefreshState()
    {
        if (_gameApp == null)
            return;

        var luaState = _gameApp.GetGameState();
        if (luaState == null)
            return;

        _state.playerLevel = luaState.PlayerLevel;
        _state.playerAttack = luaState.PlayerAttack;
        _state.gold = luaState.Gold;
        _state.enemyId = luaState.EnemyId;
        _state.enemyName = luaState.EnemyName;
        _state.enemyHp = luaState.EnemyHp;
        _state.enemyMaxHp = luaState.EnemyMaxHp;
        _state.enemyGold = luaState.EnemyGold;
        _state.enemyColor = new Color((float)luaState.EnemyColorR, (float)luaState.EnemyColorG, (float)luaState.EnemyColorB, 1f);
        _state.message = luaState.Message;
        _state.inventorySummary = luaState.InventorySummary;
        _state.lastDropText = luaState.LastDropText;
        _state.playTime = (float)luaState.PlayTime;
        _state.lastDamage = luaState.LastDamage;
        _state.lastAttackWasCrit = luaState.LastAttackWasCrit;
        _state.lastAttackWasSkill = luaState.LastAttackWasSkill;
        _state.skillCooldownRemaining = (float)luaState.SkillCooldownRemaining;
        _state.skillReady = luaState.SkillReady;
        _enemyBaseColor = _state.enemyColor != default(Color) ? _state.enemyColor : normalColor;
        if (_visualTimer <= 0f)
            ApplyEnemyColor(_enemyBaseColor);
        if (worldHud != null)
            worldHud.RefreshState(_state.enemyName, _state.enemyHp, _state.enemyMaxHp);

        if (canvasUi != null)
        {
            canvasUi.RefreshState(
                _state.playerLevel,
                _state.playerAttack,
                _state.gold,
                _state.enemyName,
                _state.enemyHp,
                _state.enemyMaxHp,
                _state.message,
                _state.inventorySummary,
                _state.playTime,
                _state.skillReady,
                _state.skillCooldownRemaining);
        }
    }

    void OnDestroy()
    {
        if (canvasUi != null)
            canvasUi.UnbindActions();

        if (_luaManager != null)
        {
            ResetEnemyVisual();
            _gameApp = null;
            _uiTexts = null;
            _luaManager.Dispose();
            _luaManager = null;
        }
    }
}
