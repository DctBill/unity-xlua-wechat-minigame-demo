using UnityEngine;
using XLua;

public sealed class LuaManager
{
    LuaEnv _luaEnv;
    LuaLogBridge _luaLogBridge;
    LuaSaveBridge _luaSaveBridge;
    ILuaGameApp _gameApp;
    LuaTable _gameAppTable;

    public ILuaGameApp GameApp => _gameApp;

    public void Initialize()
    {
        if (_luaEnv != null)
            return;

        _luaEnv = new LuaEnv();
#if ENABLE_WX_PERF_FEATURE && !UNITY_EDITOR
        WXSDKPerf.WXPerfEngine.SetLuaState(_luaEnv.L);
#endif
        _luaLogBridge = new LuaLogBridge();
        _luaSaveBridge = new LuaSaveBridge();
        _luaEnv.Global.Set("logBridge", _luaLogBridge);
        _luaEnv.Global.Set("saveBridge", _luaSaveBridge);
        _luaEnv.DoString("require 'Lua.main'");
        _gameAppTable = _luaEnv.Global.Get<LuaTable>("GameApp");
        if (_gameAppTable != null)
            _gameApp = _gameAppTable.Cast<ILuaGameApp>();
    }

    public void Tick()
    {
        if (_luaEnv == null)
            return;

        _luaEnv.Tick();
    }

    public void Dispose()
    {
        _gameApp = null;
        _luaLogBridge = null;
        _luaSaveBridge = null;

        if (_gameAppTable != null)
        {
            _gameAppTable.Dispose();
            _gameAppTable = null;
        }

        if (_luaEnv != null)
        {
            _luaEnv.Dispose();
            _luaEnv = null;
        }
    }
}
