using System;
using System.Collections.Generic;
using System.Linq;

namespace HQFramework.Procedure
{
    internal sealed class ProcedureManager : HQModuleBase, IProcedureManager
    {
        private Dictionary<Type, ProcedureBase> procedureDic;
        private ProcedureBase currentProcedure;
        private bool launched;

        public override byte Priority => 1;

        protected override void OnInitialize()
        {
            procedureDic = new Dictionary<Type, ProcedureBase>();
            launched = false;
        }

        protected override void OnUpdate()
        {
            if (!launched)
                return;

            currentProcedure.OnUpdate();
        }

        protected override void OnShutdown()
        {
            foreach (var item in procedureDic.Values)
            {
                item.OnDispose();
            }
            procedureDic.Clear();
            launched = false;
            currentProcedure = null;
        }

        public void SetEntryProcedure(Type entryProcedureType)
        {
            if (!procedureDic.ContainsKey(entryProcedureType))
            {
                throw new ArgumentException($"You have not registered {entryProcedureType} yet, can't set it as entry procedure.");
            }
            currentProcedure = procedureDic[entryProcedureType];
        }

        public void Launch()
        {
            if (launched)
            {
                throw new Exception("The Procedure Engine has been launched, you can't launch it again.");
            }
            else if (currentProcedure == null)
            {
                throw new Exception("You need to set entry procedure before launch. call SetEntryProcedure(Type entryProcedureType)");
            }

            currentProcedure.OnEnter();
            launched = true;
        }

        public void RegisterProcedure(Type procedureType)
        {
            if (procedureDic.ContainsKey(procedureType))
            {
                throw new Exception($"Unable to register procedure type: {procedureType}, because it has been registered.");
            }

            ProcedureBase procedure = Activator.CreateInstance(procedureType) as ProcedureBase;
            procedureDic.Add(procedureType, procedure);
            procedure.OnInit();
        }

        public void RegisterProcedure<T>() where T : ProcedureBase
        {
            RegisterProcedure(typeof(T));
        }

        public void SwitchProcedure(Type targetProcedureType)
        {
            if (!launched)
            {
                throw new Exception("The Procedure Engine has not been launched, you need to launch it.");
            }

            if (!procedureDic.ContainsKey(targetProcedureType))
            {
                throw new Exception($"Unable to switch to procedure type: {targetProcedureType}, because it hasn't been registered.");
            }

            currentProcedure.OnExit();
            currentProcedure = procedureDic[targetProcedureType];
            currentProcedure.OnEnter();
        }

        public void SwitchProcedure<T>() where T : ProcedureBase
        {
            SwitchProcedure(typeof(T));
        }

        public void UnregisterProcedure(Type procedureType)
        {
            if (!procedureDic.ContainsKey(procedureType))
            {
                throw new Exception($"Unable to unregister procedure type: {procedureType}, because it hasn't been registered.");
            }

            procedureDic[procedureType].OnDispose();
            procedureDic.Remove(procedureType);
        }

        public void UnregisterProcedure<T>() where T : ProcedureBase
        {
            UnregisterProcedure(typeof(T));
        }

        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (!procedureDic.ContainsKey(procedureType))
            {
                throw new Exception($"Unable to get procedure type: {procedureType}, because it hasn't been registered.");
            }
            return procedureDic[procedureType];
        }

        public T GetProcedure<T>() where T : ProcedureBase
        {
            return GetProcedure(typeof(T)) as T;
        }

        public ProcedureBase[] GetAllProcedures()
        {
            return procedureDic.Values.ToArray();
        }
    }
}
