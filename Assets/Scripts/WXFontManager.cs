using System;
using UnityEngine;
using WeChatWASM;

public static class WXFontManager
{
    public const string DefaultFallbackFontPath = "https://uwa-public.oss-cn-beijing.aliyuncs.com/wxminigame/demogame_monotest/msyh.ttc";

    public static void LoadFont(string fallbackFontPath, Font editorFallbackFont, Action<Font> onLoaded)
    {
        if (onLoaded == null)
            return;

#if UNITY_EDITOR
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            var editorFont = editorFallbackFont;
            if (editorFont == null)
                editorFont = LoadEditorFallbackFont(string.IsNullOrEmpty(fallbackFontPath) ? DefaultFallbackFontPath : fallbackFontPath);

            onLoaded(editorFont);
            return;
        }
#endif

        var runtimeFallbackPath = string.IsNullOrEmpty(fallbackFontPath) ? DefaultFallbackFontPath : fallbackFontPath;
        WX.GetWXFont(runtimeFallbackPath, onLoaded);
    }

#if UNITY_EDITOR
    static Font LoadEditorFallbackFont(string fallbackFontPath)
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Font>(fallbackFontPath);
    }
#endif
}
