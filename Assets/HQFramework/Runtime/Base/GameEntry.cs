using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class GameEntry : MonoBehaviour
    {
        private delegate void HQFrameworkLifecyleMethod();
        private delegate void HQFrameworkUpdateMethod(float deltaTimeLogic, float deltaTimeRealtime);

        private HQFrameworkLifecyleMethod frameworkInitialize;
        private HQFrameworkUpdateMethod frameworkUpdate;
        private HQFrameworkLifecyleMethod frameworkShutdown;

        private static Dictionary<Type, BaseComponent> moduleDic;

        public string logHelperTypeName;

        private void Awake()
        {
            InitializeFramework();
            InitializeFrameworkHelper();
            DontDestroyOnLoad(this);

            moduleDic = new Dictionary<Type, BaseComponent>();
        }

        private void InitializeFramework()
        {
            Type frameworkEngineType = typeof(HQFrameworkEngine);
            MethodInfo initializeMethod = frameworkEngineType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo updateMethod = frameworkEngineType.GetMethod("Update", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(float), typeof(float) }, null);
            MethodInfo shutdownMethod = frameworkEngineType.GetMethod("Shutdown", BindingFlags.Static | BindingFlags.NonPublic);

            frameworkInitialize = (HQFrameworkLifecyleMethod)Delegate.CreateDelegate(typeof(HQFrameworkLifecyleMethod), initializeMethod);
            frameworkUpdate = (HQFrameworkUpdateMethod)Delegate.CreateDelegate(typeof(HQFrameworkUpdateMethod), updateMethod);
            frameworkShutdown = (HQFrameworkLifecyleMethod)Delegate.CreateDelegate(typeof(HQFrameworkLifecyleMethod), shutdownMethod);

            frameworkInitialize.Invoke();
        }

        private void InitializeFrameworkHelper()
        {
            Type logHelperType = Type.GetType(logHelperTypeName);
            if (logHelperType == null)
            {
                throw new Exception($"{logHelperTypeName} is not found.");
            }
            ILogHelper logHelper = Activator.CreateInstance(logHelperType) as ILogHelper;
            HQDebugger.SetHelper(logHelper);
        }

        private void Update()
        {
            frameworkUpdate.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            frameworkShutdown.Invoke();
        }

        public static void RegisterModule(BaseComponent component)
        {
            Type type = component.GetType();
            moduleDic.Add(type, component);
        }

        public static T GetModule<T>() where T : BaseComponent
        {
            Type type = typeof(T);
            return moduleDic[type] as T;
        }

        public static BaseComponent GetModule(Type moduleType)
        {
            return moduleDic[moduleType];
        }

        public static BaseComponent GetModule(string moduleTypeName)
        {
            Type type = Type.GetType(moduleTypeName);
            return moduleDic[type];
        }
    }
}
