#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class LuaSaveBridgeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(LuaSaveBridge);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HasSave", _m_HasSave);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadGame", _m_LoadGame);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SaveGame", _m_SaveGame);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearSave", _m_ClearSave);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new LuaSaveBridge();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to LuaSaveBridge constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasSave(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaSaveBridge gen_to_be_invoked = (LuaSaveBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.HasSave(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadGame(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaSaveBridge gen_to_be_invoked = (LuaSaveBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.LoadGame(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveGame(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaSaveBridge gen_to_be_invoked = (LuaSaveBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _playerLevel = LuaAPI.xlua_tointeger(L, 2);
                    int _playerAttack = LuaAPI.xlua_tointeger(L, 3);
                    int _gold = LuaAPI.xlua_tointeger(L, 4);
                    int _enemyId = LuaAPI.xlua_tointeger(L, 5);
                    string _enemyName = LuaAPI.lua_tostring(L, 6);
                    int _enemyHp = LuaAPI.xlua_tointeger(L, 7);
                    int _enemyMaxHp = LuaAPI.xlua_tointeger(L, 8);
                    int _enemyGold = LuaAPI.xlua_tointeger(L, 9);
                    int _defeatedCount = LuaAPI.xlua_tointeger(L, 10);
                    int _slimeGel = LuaAPI.xlua_tointeger(L, 11);
                    int _heavyCore = LuaAPI.xlua_tointeger(L, 12);
                    int _flameShard = LuaAPI.xlua_tointeger(L, 13);
                    float _playTime = (float)LuaAPI.lua_tonumber(L, 14);
                    float _skillCooldownRemaining = (float)LuaAPI.lua_tonumber(L, 15);
                    
                    gen_to_be_invoked.SaveGame( _playerLevel, _playerAttack, _gold, _enemyId, _enemyName, _enemyHp, _enemyMaxHp, _enemyGold, _defeatedCount, _slimeGel, _heavyCore, _flameShard, _playTime, _skillCooldownRemaining );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearSave(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaSaveBridge gen_to_be_invoked = (LuaSaveBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearSave(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
