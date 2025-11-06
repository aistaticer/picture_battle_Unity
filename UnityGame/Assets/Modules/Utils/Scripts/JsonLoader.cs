using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class JsonLoader
{
    public static T LoadFromStreamingAssets<T>(string relativePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);

        if (!File.Exists(path))
        {
            Debug.LogError($"JSON file not found at path: {path}");
            return default;
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }
}
