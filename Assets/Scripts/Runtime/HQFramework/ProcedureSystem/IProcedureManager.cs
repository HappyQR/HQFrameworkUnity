using System;

namespace HQFramework.Procedure
{
    public interface IProcedureManager
    {
        void SetEntryProcedure(Type entryProcedureType);

        void Launch();

        void RegisterProcedure(Type procedureType);

        void RegisterProcedure<T>() where T : ProcedureBase;

        void SwitchProcedure(Type targetProcedureType);

        void SwitchProcedure<T>() where T : ProcedureBase;

        void UnregisterProcedure(Type procedureType);

        void UnregisterProcedure<T>() where T : ProcedureBase;

        ProcedureBase GetProcedure(Type procedureType);

        T GetProcedure<T>() where T : ProcedureBase;

        ProcedureBase[] GetAllProcedures();
    }
}
