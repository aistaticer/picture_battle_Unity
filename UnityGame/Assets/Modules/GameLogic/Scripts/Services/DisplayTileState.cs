using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using System.Threading.Tasks;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    public class DisplayTileState
    {
        private TileManager _tileManager;
        private GameObject _markerPrefab;

        /// <summary>
        /// シーンに表示されているクリック可能タイル表示用のオブジェクトリスト
        /// </summary>
        private List<GameObject> _displayedMarkers = new List<GameObject>();

        [Inject]
        public void Construct(TileManager tileManager)
        {
            _tileManager = tileManager;
            LoadMarkerPrefab();
        }

        public DisplayTileState(TileManager tileManager)
		{
			_tileManager = tileManager;
			LoadMarkerPrefab();
		}

        /// <summary>
        /// マーカープレハブをResourcesフォルダからロードする
        /// </summary>
        private void LoadMarkerPrefab()
        {
            // Assets/Resources/Prefabs/highlightTile.prefab からロード
            _markerPrefab = Resources.Load<GameObject>("Prefabs/highlightTile");

            if (_markerPrefab == null)
            {
                Debug.LogError("highlightTile プレハブが見つかりません。Assets/Resources/Prefabs/highlightTile.prefab に配置してください。");
            }
        }

        private static readonly Vector3Int[] offsets4 = new Vector3Int[]
        {
                new Vector3Int( 1, 0,  0), // +X
                new Vector3Int(-1, 0,  0), // -X
                new Vector3Int( 0, 0,  1), // +Z
                new Vector3Int( 0, 0, -1)  // -Z
        };

        /// <summary>
        /// 指定されたタイルから特定の距離にあるタイルにマーカーオブジェクトを表示し、TileManagerに登録する。
        /// BFS（幅優先探索）を使用して、開始タイルから指定された移動距離（moveDistance）だけ離れた
        /// 全てのタイルにマーカーオブジェクトを生成し、クリック可能タイルとして登録する。
        /// </summary>
        /// <param name="onTileKey">開始タイルのキー（例: "1-0-1"）</param>
        /// <param name="moveDistance">移動可能な距離（タイル数）</param>
        public void DisplayClicableTile(string onTileKey, int moveDistance)
		{
            if (_tileManager.GetTileData(onTileKey) == null)
                return;

            // 既存の表示オブジェクトとクリック可能タイルをクリア
            ClearDisplayMarkers();
            _tileManager.ClearClickableTiles();

            var startTile = _tileManager.GetTileData(onTileKey);

            // BFS用のデータ構造を初期化
            var visited = new HashSet<string>();              // 探索済みタイルのキーを保持
            var markerCreated = new HashSet<string>();        // マーカー作成済みタイルのキーを保持（重複防止）
            var queue = new Queue<(TileData tile, int distance)>();

            // スタート地点をキューに追加
            queue.Enqueue((startTile, 0));
            visited.Add(startTile.Key);

            // BFS（幅優先探索）で距離moveDistanceのタイルを探索
            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                // distance < moveDistance の時のみ隣接タイルを探索
                if (distance < moveDistance)
                {
                    // 4方向（+X, -X, +Z, -Z）の隣接タイルを探索
                    foreach (var offset in offsets4)
                    {
                        int newX = (int)current.Position.x + offset.x;
					    int newY = (int)current.Position.y + offset.y;
					    int newZ = (int)current.Position.z + offset.z;

                        // 隣接タイルのキーを生成
					    var adjacentKey = $"{newX}-{newY}-{newZ}";

                        // タイルが存在するか確認
                        var nextTileData = _tileManager.GetTileData(adjacentKey);
                        if (nextTileData == null)
                            continue;

                        // 移動距離が指定された距離と一致する場合、マーカーオブジェクトを配置
                        // 同じタイルに複数の経路で到達する可能性があるため、markerCreatedで重複チェック
                        if (distance + 1 == moveDistance)
                        {
                            if (!markerCreated.Contains(adjacentKey))
                            {
                                CreateMarkerAtTile(adjacentKey);
                                _tileManager.RegisterClickableTile(adjacentKey);
                                markerCreated.Add(adjacentKey);
                            }
                        }

                        // まだ探索していないタイルをキューに追加
                        if (!visited.Contains(adjacentKey))
                        {
                            visited.Add(adjacentKey);
                            queue.Enqueue((nextTileData, distance + 1));
                        }
                    }
                }
            }
		}

        /// <summary>
        /// 指定されたタイルキーの位置にマーカーオブジェクトを生成する
        /// </summary>
        /// <param name="tileKey">タイルのキー（例: "1-0-1"）</param>
        private void CreateMarkerAtTile(string tileKey)
        {
            if (_markerPrefab == null)
            {
                Debug.LogError("マーカープレハブが設定されていません");
                return;
            }

            // タイルの位置を取得
            var tileData = _tileManager.GetTileData(tileKey);
            if (tileData == null)
            {
                Debug.LogError($"タイルが見つかりません: {tileKey}");
                return;
            }

            // プレハブをタイルと同じ位置にインスタンス化
            var marker = GameObject.Instantiate(_markerPrefab, tileData.Position.ToVector3(), Quaternion.identity);
            marker.name = $"ClickableMarker_{tileKey}";

            // リストに追加
            _displayedMarkers.Add(marker);
        }

        /// <summary>
        /// 全ての表示マーカーオブジェクトを削除する
        /// </summary>
        public void ClearDisplayMarkers()
        {
            foreach (var marker in _displayedMarkers)
            {
                if (marker != null)
                {
                    GameObject.Destroy(marker);
                }
            }
            _displayedMarkers.Clear();
        }

        /// <summary>
        /// クリック可能なタイルの中から指定されたタイルの色を変更する
        /// </summary>
        /// <param name="tileKey">色を変更するタイルのキー</param>
        /// <param name="tileType">変更後のタイルタイプ（色）</param>
        /// <returns>タイルの色変更に成功した場合true、失敗した場合false</returns>
        public bool ChangeClickableTileColor(string tileKey, TileType tileType)
        {
            // 指定されたタイルがクリック可能タイルに登録されているか確認
            if (!_tileManager.IsClickable(tileKey))
            {
                Debug.LogWarning($"タイル {tileKey} はクリック可能タイルとして登録されていません");
                return false;
            }

            // タイルの色を変更
            _tileManager.ChangesetColor(tileKey, tileType);
            return true;
        }
    }
}

