using System;

namespace HQFramework.Procedure
{
    public abstract class ProcedureBase
    {
        protected internal virtual void OnRegistered()
        {

        }

        protected internal virtual void OnEnter()
        {

        }

        protected internal virtual void OnUpdate()
        {

        }

        protected internal virtual void OnExit()
        {

        }

        protected internal virtual void OnUnregistered()
        {

        }

        protected void SwitchProcedure(Type targetProcedureType)
        {
            HQFrameworkEngine.GetModule<IProcedureManager>().SwitchProcedure(targetProcedureType);
        }

        protected void SwitchProcedure<T>() where T : ProcedureBase
        {
            HQFrameworkEngine.GetModule<IProcedureManager>().SwitchProcedure<T>();
        }
    }
}
