using System;
using System.IO;
using UnityEngine;
using static Define;
public static class Utils
{
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static void SetParentActive(GameObject go, bool active)
    {
        if (go == null) return;
        var parent = go.transform.parent;
        if (parent == null) return;
        parent.gameObject.SetActive(active);
    } 
    
    public static Sprite Texture2DToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        
        return Sprite.Create(texture, rect, pivot);
    }

    public static int GetLayerNumber(int layerNum)
    {
        return 1 << layerNum;
    }

    public static string ConvertResourcePath(string assetPath)
    {
        var startIdx = assetPath.IndexOf("Sprites", StringComparison.Ordinal);
        string relativePath = assetPath.Substring(startIdx, assetPath.Length - startIdx);
        string resourcePath = Path.ChangeExtension(relativePath, null);

        return resourcePath;
    }

    public static Color GetCodeToColor(string targetColorCode)
    {
        if (ColorUtility.TryParseHtmlString(targetColorCode, out var targetColor))
        {
            return targetColor;
        }

        return default;
    }
}
