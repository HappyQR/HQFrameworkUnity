using System.Collections;
using System.Collections.Generic;
using HQFramework.Runtime;
using UnityEngine;
using HQFramework;
using TMPro;

public class UIFormTest2 : UIFormBase
{
    public override string AssetPath => "Assets/GameAssets/Public/UI/UIFormTest2.prefab";

    public override uint AssetCrc => Utility.CRC32.ComputeCrc32(AssetPath);

    public override int GroupID => 1;

    private string name = nameof(UIFormTest2);

    private HQListBase listTest;

    protected override void OnCreate()
    {
        base.OnCreate();
        HQDebugger.LogInfo($"{name} OnCreate");

        listTest = GetUIControl<HQListBase>(4);
        listTest.SetItemCount(1024);
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
            listTest.SetItemCount(12);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            listTest.AppendItemCount(28);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            listTest.InsertItem(0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            listTest.RemoveItem(0);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            listTest.ScrollTo(208);
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

        GameEntry.GetModule<EventComponent>().InvokeEventImmediately(this, TestEventArgs.Create("Invoke Test Event Immediately"));
    }

    protected override void OnSliderValueChanged(string sliderName, float value)
    {
        base.OnSliderValueChanged(sliderName, value);
        HQDebugger.LogInfo($"{name} OnSliderValueChanged, {sliderName} {value}");

        GameEntry.GetModule<EventComponent>().InvokeEvent(this, TestEventArgs.Create("Invoke Test Event"));
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

    protected override void OnListItemInit(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            TextMeshProUGUI txt = item.GetUIControl<TextMeshProUGUI>(0);
            txt.text = $"Item-{index}";
        }
    }

    protected override void OnListItemClick(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            HQDebugger.Log($"selected : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverEnter(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            // HQDebugger.Log($"hover enter : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverExit(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            // HQDebugger.Log($"hover exit : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemButtonClick(string listName, HQListItem item, string buttonName, GameObject buttonObject, int index)
    {
        if (listName == "ListTest")
        {
            if (buttonName == "BtnTest")
            {
                HQDebugger.Log($"button click : {listName}, {item.name}, {buttonName}, {index}");
            }
        }
    }
}
