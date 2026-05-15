using System;
using System.Collections.Generic;
using XLua;

public static class XLuaGameConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>
    {
        typeof(LuaLogBridge),
        typeof(LuaGameEntry),
        typeof(LuaSaveBridge),
        typeof(LuaGameSaveData)
    };

    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>
    {
        typeof(ILuaGameApp),
        typeof(ILuaGameState),
        typeof(ILuaGameTexts)
    };
}
