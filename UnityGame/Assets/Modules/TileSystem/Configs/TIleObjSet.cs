using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace picture_game_view.Assets.Modules.TileSystem.Configs
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TIleObjType
    {
        NomalTile,
        GrassTIle
    }

    [CreateAssetMenu(menuName = "Config/TileObjSet")]
    public class TIleObjSet : ScriptableObject
    {
        public GameObject NomalTile;
        public GameObject GrassTIle;

        public GameObject GetTileObj(TIleObjType type)
        {
            switch (type)
            {
                case TIleObjType.NomalTile: return NomalTile;
                case TIleObjType.GrassTIle: return GrassTIle;
                default: return null;
            }
        }
    }
}
