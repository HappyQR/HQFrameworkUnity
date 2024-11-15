using System.Collections;
using System.Collections.Generic;
using HQFramework.Runtime;
using UnityEngine;
using HQFramework;

public class UIFormTest2 : UIFormBase
{
    public override string AssetPath => "Assets/GameAssets/Public/UI/UIFormTest2.prefab";

    public override uint AssetCrc => Utility.CRC32.ComputeCrc32(AssetPath);

    public override int GroupID => 1;

    private string name = nameof(UIFormTest2);

    protected override void OnCreate()
    {
        base.OnCreate();
        HQDebugger.LogInfo($"{name} OnCreate");
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        HQDebugger.LogInfo($"{name} OnOpen, user data : {userData}");
    }

    protected override void OnCovered()
    {
        base.OnCovered();
        HQDebugger.LogInfo($"{name} OnCovered");
    }

    protected override void OnRevealed()
    {
        base.OnRevealed();
        HQDebugger.LogInfo($"{name} OnRevealed");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        HQDebugger.LogInfo($"{name} OnUpdate");
    }

    protected override void OnClose()
    {
        base.OnClose();
        HQDebugger.LogInfo($"{name} OnClose");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HQDebugger.LogInfo($"{name} OnDestroy");
    }

    protected override void OnToggleValueChanged(string toggleName, bool value)
    {
        base.OnToggleValueChanged(toggleName, value);
        HQDebugger.LogInfo($"{name} OnToggleValueChanged, {toggleName} {value}");
    }

    protected override void OnSliderValueChanged(string sliderName, float value)
    {
        base.OnSliderValueChanged(sliderName, value);
        HQDebugger.LogInfo($"{name} OnSliderValueChanged, {sliderName} {value}");
    }

    protected override void OnButtonClick(string buttonName)
    {
        base.OnButtonClick(buttonName);
        switch (buttonName)
        {
            case "Btn1":
                GameEntry.GetModule<UIComponent>().OpenUIForm<UIFormTest3>(888, null, (errMsg) => HQDebugger.LogError(errMsg));
                break;
            case "Btn2":
                GameEntry.GetModule<UIComponent>().CloseUIForm<UIFormTest2>();
                GameEntry.GetModule<UIComponent>().OpenUIForm<UIFormTest1>("hahahaha", null, (errMsg) => HQDebugger.LogError(errMsg));
                break;
        }

    }
}
