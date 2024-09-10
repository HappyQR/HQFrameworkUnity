using HQFramework;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine;

public class HotfixProcedure : ProcedureBase
{
    private IResourceManager resourceManager;

    protected override void OnInit()
    {
        JsonLitHelper jsonLitHelper = new JsonLitHelper();
        SerializeManager.SetJsonHelper(jsonLitHelper);

        DefaultResourceHelper helper = new DefaultResourceHelper();
        resourceManager = HQFrameworkEngine.GetModule<IResourceManager>();
        resourceManager.SetHelper(helper);

        
    }

    protected override void OnEnter()
    {
        
    }

    protected override void OnExit()
    {
        base.OnExit();
    }
}
