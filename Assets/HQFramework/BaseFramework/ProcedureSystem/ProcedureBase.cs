using System;

namespace HQFramework.Procedure
{
    public abstract class ProcedureBase
    {
        protected internal virtual void OnInit()
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

        protected internal virtual void OnShutdown()
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
