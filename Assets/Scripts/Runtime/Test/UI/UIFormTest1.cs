using System;
using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.EventSystem;
using HQFramework.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFormTest1 : UIFormBase
{
    public override string AssetPath => "Assets/GameAssets/Public/UI/UIFormTest1.prefab";

    public override uint AssetCrc => Utility.CRC32.ComputeCrc32(AssetPath);

    public override int GroupID => 0;

    private string name = nameof(UIFormTest1);

    private HQListBase listTest;

    protected override void OnCreate()
    {
        base.OnCreate();
        HQDebugger.LogInfo($"{name} OnCreate");

        listTest = GetUIControl<HQListBase>(4);
    }

    private void OnTestEvent(object sender, EventArgsBase e)
    {
        HQDebugger.LogInfo($"{sender}, {(e as TestEventArgs).Message}");
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        HQDebugger.LogInfo($"{name} OnOpen, user data : {userData}");
        GameEntry.GetModule<EventComponent>().RegisterEventListener(TestEventArgs.ID, OnTestEvent);

        listTest.SetItemCount(32);
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
            listTest.ScrollTo(20);
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        HQDebugger.LogInfo($"{name} OnClose");
        GameEntry.GetModule<EventComponent>().UnregisterEventListener(TestEventArgs.ID, OnTestEvent);
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
                GameEntry.GetModule<UIComponent>().OpenUIForm<UIFormTest2>(666, null, (errMsg) => HQDebugger.LogError(errMsg));
                GameEntry.GetModule<UIComponent>().CloseUIForm(this);
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

    protected override void OnListItemSelected(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            HQDebugger.Log($"selected : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemUnselected(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            HQDebugger.Log($"unselected : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverEnter(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            HQDebugger.Log($"hover enter : {listName}, {item.name}, {index}");
        }
    }

    protected override void OnListItemHoverExit(string listName, HQListItem item, int index)
    {
        if (listName == "ListTest")
        {
            HQDebugger.Log($"hover exit : {listName}, {item.name}, {index}");
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
