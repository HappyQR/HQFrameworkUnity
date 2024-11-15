using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.EventSystem;
using UnityEngine;

public class TestEventArgs : EventArgsBase
{
    public static readonly int ID = typeof(TestEventArgs).GetHashCode();

    public override int SerialID => ID;

    public string Message
    {
        get;
        private set;
    }

    public static TestEventArgs Create(string message)
    {
        TestEventArgs args = ReferencePool.Spawn<TestEventArgs>();
        args.Message = message;
        return args;
    }

    protected override void OnRecyle()
    {
        Message = null;
    }
}
