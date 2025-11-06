// using UnityEngine;

// public class ObjectSpawner : MonoBehaviour
// {
//     public GameObject prefab;
//     private GameObject spawnedObj;

//     void Awake() {
//         prefab = Resources.Load<GameObject>("Prefabs/Cube"); // Prefab 化済み
//     }

//     void Start() {
//         Debug.Log("ObjectSpawner.Start");
//         if (prefab == null) {
//             Debug.LogError("Prefab がロードされていません！");
//             return;
//         }
//         spawnedObj = Instantiate(prefab, new Vector3(0,1,0), Quaternion.identity);
//         spawnedObj.transform.localScale = Vector3.one * 0.5f;

//         // マテリアル確認
//         Renderer rend = spawnedObj.GetComponent<Renderer>();
//         if (rend != null) {
//             Debug.Log("Material: " + rend.sharedMaterial.name);
//         }
//     }

//     void Update() {
//         if (spawnedObj != null) {
//             // Y軸回転
//             spawnedObj.transform.Rotate(0, 45 * Time.deltaTime, 0);
//         }
//     }
// }
