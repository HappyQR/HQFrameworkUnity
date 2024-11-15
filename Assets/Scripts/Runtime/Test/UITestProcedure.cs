using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Procedure;
using HQFramework.Runtime;
using UnityEngine;

public class UITestProcedure : ProcedureBase
{
    private UIComponent uiComponent;

    private UIGroup bottom;
    private UIGroup top;

    protected override void OnRegistered()
    {
        uiComponent = GameEntry.GetModule<UIComponent>();
    }

    protected override void OnEnter()
    {
        bottom = GameObject.Find("Bottom").GetComponent<UIGroup>();
        top = GameObject.Find("Top").GetComponent<UIGroup>();

        uiComponent.AddUIGroup(bottom);
        uiComponent.AddUIGroup(top);

        uiComponent.OpenUIForm<UIFormTest1>("Hello, UIFormTest1", null, OnOpenFormError);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            uiComponent.DeleteUIGroup(top);
        }
    }

    protected override void OnExit()
    {
        
    }

    protected override void OnUnregistered()
    {
        
    }

    private void OnOpenFormError(string errorMessage)
    {
        HQDebugger.LogError(errorMessage);
    }
}
