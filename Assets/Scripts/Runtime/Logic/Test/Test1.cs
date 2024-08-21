using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Unity;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityLogHelper logHelper = new UnityLogHelper();
        Debugger.SetLogHelper(logHelper);

        Debugger.LogInfo("Hello HQFramework");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debugger.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }
    }
}
