using System.Collections;
using System.Collections.Generic;
using HQFramework.Runtime;
using UnityEngine;
using HQFramework;
using TMPro;

public class UIFormTest3 : UIFormBase
{
    public override string AssetPath => "Assets/GameAssets/Public/UI/UIFormTest1.prefab";

    public override uint AssetCrc => Utility.CRC32.ComputeCrc32(AssetPath);

    public override int GroupID => 1;

    private string name = nameof(UIFormTest3);

    private HQListBase list;

    protected override void OnCreate()
    {
        base.OnCreate();
        HQDebugger.LogInfo($"{name} OnCreate");

        list = GetUIControl<HQListBase>(5);
        list.SetItemCount(888);
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
        // HQDebugger.LogInfo($"{name} OnUpdate");

        if (Input.GetKeyDown(KeyCode.A))
        {
            list.SetItemCount(12);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            list.AppendItemCount(28);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            list.InsertItem(0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            list.RemoveItem(0);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            list.ScrollTo(20);
        }
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
                GameEntry.GetModule<UIComponent>().OpenUIForm<UIFormTest1>(777, null, (errMsg) => HQDebugger.LogError(errMsg));
                break;
            case "Btn2":
                GameEntry.GetModule<UIComponent>().CloseUIForm<UIFormTest3>();
                break;
        }

    }

    protected override void OnListItemInit(string listName, HQListItem item, int index)
    {
        if (listName == "ListH")
        {
            TextMeshProUGUI txt = item.GetUIControl<TextMeshProUGUI>(0);
            txt.text = $"Item-{index}";
        }
    }

    protected override void OnListItemClick(string listName, HQListItem item, int index)
    {
        if (listName == "ListH")
        {
            HQDebugger.Log($"selected : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverEnter(string listName, HQListItem item, int index)
    {
        if (listName == "ListH")
        {
            HQDebugger.Log($"hover enter : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverExit(string listName, HQListItem item, int index)
    {
        if (listName == "ListH")
        {
            HQDebugger.Log($"hover exit : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemButtonClick(string listName, HQListItem item, string buttonName, GameObject buttonObject, int index)
    {
        if (listName == "ListH")
        {
            if (buttonName == "BtnTest")
            {
                HQDebugger.Log($"button click : {listName}, {item.name}, {buttonName}, {index}");
            }
        }
    }
}
