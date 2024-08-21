using System;
using System.Reflection;
using HQFramework.Procedure;
using UnityEngine;

namespace HQFramework.Unity
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The full type name of LogHelper")]
        public string logHelperTypeName;

        [SerializeField]
        public string[] gameProcedures;

        [SerializeField]
        public string entryProcedure;

        private delegate void HQFrameworkLifecyleMethod();
        private delegate void HQFrameworkUpdateMethod(float deltaTimeLogic, float deltaTimeRealtime);

        private HQFrameworkLifecyleMethod frameworkInitialize;
        private HQFrameworkUpdateMethod frameworkUpdate;
        private HQFrameworkLifecyleMethod frameworkShutdown;

        private void Awake()
        {
            InitializeFramework();
            InitializeFrameworkHelper();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            RegisterAllProcedures();
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
            HQDebugger.SetLogHelper(logHelper);
        }

        private void RegisterAllProcedures()
        {
            IProcedureManager procedureManager = HQFrameworkEngine.GetModule<IProcedureManager>();
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type baseProcedureType = typeof(ProcedureBase);
            Type entryProcedureType = currentAssembly.GetType(entryProcedure);
            if (gameProcedures.Length > 0)
            {
                for (int i = 0; i < gameProcedures.Length; i++)
                {
                    Type type = currentAssembly.GetType(gameProcedures[i]);
                    if (!baseProcedureType.IsAssignableFrom(type))
                    {
                        Debug.LogError($"{type} is not a subclass of ProcedureBase.");
                    }
                    procedureManager.RegisterProcedure(type);
                }
            }

            procedureManager.SetEntryProcedure(entryProcedureType);
            procedureManager.Launch();
        }

        private void Update()
        {
            frameworkUpdate.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            frameworkShutdown.Invoke();
        }
    }
}
