using System;
using System.Collections;
using HQFramework.Procedure;
using UnityEngine;

namespace HQFramework.Runtime
{
    public class ProcedureComponent : BaseComponent
    {
#if UNITY_EDITOR
        public UnityEditor.MonoScript[] procedureScripts;
#endif

        [SerializeField]
        private string[] gameProcedures;
        [SerializeField]
        private string entryProcedure;

        private IProcedureManager procedureManager;

        private void Start()
        {
            procedureManager = HQFrameworkEngine.GetModule<IProcedureManager>();
            GameEntry.GetModule<CoroutineComponent>().StartHQCoroutine(RegisterAllProcedures());
        }

        private IEnumerator RegisterAllProcedures()
        {
            Type baseProcedureType = typeof(ProcedureBase);
            Type entryProcedureType = Utility.Assembly.GetType(entryProcedure);
            if (gameProcedures.Length > 0)
            {
                for (int i = 0; i < gameProcedures.Length; i++)
                {
                    Type type = Utility.Assembly.GetType(gameProcedures[i]);
                    if (!baseProcedureType.IsAssignableFrom(type))
                    {
                        HQDebugger.LogError($"{type} is not a subclass of ProcedureBase.");
                    }
                    procedureManager.RegisterProcedure(type);

                    yield return null;
                }
            }

            procedureManager.SetEntryProcedure(entryProcedureType);
            procedureManager.Launch();
        }

        public void RegisterProcedure(Type procedureType)
        {
            procedureManager.RegisterProcedure(procedureType);
        }

        public void RegisterProcedure<T>() where T : ProcedureBase
        {
            procedureManager.RegisterProcedure<T>();
        }

        public void SwitchProcedure(Type targetProcedureType)
        {
            procedureManager.SwitchProcedure(targetProcedureType);
        }

        public void SwitchProcedure<T>() where T : ProcedureBase
        {
            procedureManager.SwitchProcedure<T>();
        }

        public void UnregisterProcedure(Type procedureType)
        {
            procedureManager.UnregisterProcedure(procedureType);
        }

        public void UnregisterProcedure<T>() where T : ProcedureBase
        {
            procedureManager.UnregisterProcedure<T>();
        }

        public ProcedureBase GetProcedure(Type procedureType)
        {
            return procedureManager.GetProcedure(procedureType);
        }

        public T GetProcedure<T>() where T : ProcedureBase
        {
            return procedureManager.GetProcedure<T>();
        }

        public ProcedureBase[] GetAllProcedures()
        {
            return procedureManager.GetAllProcedures();
        }
    }
}
