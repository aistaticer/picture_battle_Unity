using UnityEngine;

public static class DtoLogger
{
    public static void LogAsJson<T>(T dto, string label = null)
    {
        if (dto == null)
        {
            Debug.LogWarning($"{label ?? typeof(T).Name} is null");
            return;
        }

        string json = JsonUtility.ToJson(dto, true); // true = インデント付き
        Debug.Log($"{label ?? typeof(T).Name}:\n{json}");
    }
}
