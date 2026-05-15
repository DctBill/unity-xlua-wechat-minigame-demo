using System;
using UnityEngine;
#if UNITY_WEBGL || WEIXINMINIGAME || UNITY_EDITOR
using WeChatWASM;
#endif

[Serializable]
public class LuaGameSaveData
{
    public int PlayerLevel;
    public int PlayerAttack;
    public int Gold;
    public int EnemyId;
    public string EnemyName;
    public int EnemyHp;
    public int EnemyMaxHp;
    public int EnemyGold;
    public int DefeatedCount;
    public int SlimeGel;
    public int HeavyCore;
    public int FlameShard;
    public float PlayTime;
    public float SkillCooldownRemaining;
}

public class LuaSaveBridge
{
    const string SaveKey = "slime_game_save";

    public bool HasSave()
    {
        return !string.IsNullOrEmpty(GetSaveJson());
    }

    public LuaGameSaveData LoadGame()
    {
        var json = GetSaveJson();
        if (string.IsNullOrEmpty(json))
            return null;

        try
        {
            return JsonUtility.FromJson<LuaGameSaveData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Save] Load failed: " + ex.Message);
            return null;
        }
    }

    public void SaveGame(int playerLevel, int playerAttack, int gold, int enemyId, string enemyName, int enemyHp, int enemyMaxHp, int enemyGold, int defeatedCount, int slimeGel, int heavyCore, int flameShard, float playTime, float skillCooldownRemaining)
    {
        var data = new LuaGameSaveData
        {
            PlayerLevel = playerLevel,
            PlayerAttack = playerAttack,
            Gold = gold,
            EnemyId = enemyId,
            EnemyName = enemyName,
            EnemyHp = enemyHp,
            EnemyMaxHp = enemyMaxHp,
            EnemyGold = enemyGold,
            DefeatedCount = defeatedCount,
            SlimeGel = slimeGel,
            HeavyCore = heavyCore,
            FlameShard = flameShard,
            PlayTime = playTime,
            SkillCooldownRemaining = skillCooldownRemaining
        };

        var json = JsonUtility.ToJson(data);
        SetSaveJson(json);
    }

    public void ClearSave()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
#elif UNITY_WEBGL || WEIXINMINIGAME
        WX.StorageDeleteKeySync(SaveKey);
#else
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
#endif
    }

    string GetSaveJson()
    {
#if UNITY_EDITOR
        return PlayerPrefs.GetString(SaveKey, string.Empty);
#elif UNITY_WEBGL || WEIXINMINIGAME
        return WX.StorageGetStringSync(SaveKey, string.Empty);
#else
        return PlayerPrefs.GetString(SaveKey, string.Empty);
#endif
    }

    void SetSaveJson(string json)
    {
#if UNITY_EDITOR
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
#elif UNITY_WEBGL || WEIXINMINIGAME
        WX.StorageSetStringSync(SaveKey, json);
#else
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
#endif
    }
}
