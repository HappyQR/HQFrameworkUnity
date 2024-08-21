using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HQFramework.Editor
{
    public class DebuggerEditor
    {
        public static bool Log_Enable
        {
            get
            {
                return ScriptingDefineSymbols.HasScriptingDefineSymbolAll(Debugger.ENABLE_LOG_SYMBOL);
            }
        }

        [MenuItem("HQFramework/Debugger/Enable Log")]
        public static void EnableLog()
        {
            if (!Log_Enable)
            {
                ScriptingDefineSymbols.AddScriptingDefineSymbol(Debugger.ENABLE_LOG_SYMBOL);
            }

            Debug.Log("Log Enable : " + Log_Enable);
        }

        [MenuItem("HQFramework/Debugger/Disable Log")]
        public static void DisableLog()
        {
            if (Log_Enable)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(Debugger.ENABLE_LOG_SYMBOL);
            }

            Debug.Log("Log Enable : " + Log_Enable);
        }
    }
}
