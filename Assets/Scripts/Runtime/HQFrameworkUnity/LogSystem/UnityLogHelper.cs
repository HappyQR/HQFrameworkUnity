using System.Collections;
using System.Collections.Generic;
using HQFramework;
using UnityEngine;

namespace HQFramework.Unity
{
    public class UnityLogHelper : ILogHelper
    {
        public void Log(object message, LogLevel level, LogColor color)
        {
            Debug.Log(message);
        }
    }
}
