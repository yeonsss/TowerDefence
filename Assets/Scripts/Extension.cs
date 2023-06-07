using System;
using UI;
using UnityEngine;

public static class Extension
{
    public static void BindEvent(this GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
    {
        BaseUI.BindEvent(go, action, type);
    } 
    
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }
    
    public static void SetParentActive(this GameObject go, bool active)
    {
        Utils.SetParentActive(go, active);
    } 
}