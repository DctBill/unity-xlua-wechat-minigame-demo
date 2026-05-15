public class LuaLogBridge
{
    public void Log(string message)
    {
        UnityEngine.Debug.Log("[Lua] " + message);
    }
}

public interface ILuaGameApp
{
    void LoadProgress();
    void Tick(float dt);
    void AttackEnemy();
    void UseSkill();
    void ResetGame();
    ILuaGameState GetGameState();
    ILuaGameTexts GetUiTexts();
}

public interface ILuaGameState
{
    int PlayerLevel { get; }
    int PlayerAttack { get; }
    int Gold { get; }
    int EnemyId { get; }
    string EnemyName { get; }
    int EnemyHp { get; }
    int EnemyMaxHp { get; }
    int EnemyGold { get; }
    double EnemyColorR { get; }
    double EnemyColorG { get; }
    double EnemyColorB { get; }
    string Message { get; }
    string InventorySummary { get; }
    string LastDropText { get; }
    double PlayTime { get; }
    int LastDamage { get; }
    bool LastAttackWasCrit { get; }
    bool LastAttackWasSkill { get; }
    double SkillCooldownRemaining { get; }
    bool SkillReady { get; }
}

public interface ILuaGameTexts
{
    string Title { get; }
    string PlayerLevelLabel { get; }
    string PlayerAttackLabel { get; }
    string GoldLabel { get; }
    string EnemyLabel { get; }
    string HpLabel { get; }
    string PlayTimeLabel { get; }
    string MessageLabel { get; }
    string InventoryLabel { get; }
    string AttackButtonLabel { get; }
    string SkillButtonLabel { get; }
    string ResetButtonLabel { get; }
    string SkillCooldownLabel { get; }
    string SecondsSuffix { get; }
}
