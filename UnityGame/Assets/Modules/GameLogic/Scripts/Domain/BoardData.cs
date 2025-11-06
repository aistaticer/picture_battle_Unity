using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class BoardData {
		public string boardId;
		public Dictionary<string, TileData> tiles;
}
