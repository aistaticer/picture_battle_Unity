// Assets/Scripts/Models/ClickState.cs
using UnityEngine;

/// <summary>
/// 最後にクリックされたオブジェクトの情報を保持する状態オブジェクト
/// </summary>
public class ClickState
{
    /// <summary>最後にクリックされた GameObject</summary>
    public GameObject LastClickedObject { get; private set; }

    /// <summary>クリック情報を更新するメソッド</summary>
    public void SetClickedObject(GameObject obj)
    {
        LastClickedObject = obj;
    }

    /// <summary>クリック情報をクリアするメソッド</summary>
    public void Clear()
    {
        LastClickedObject = null;
    }
}

