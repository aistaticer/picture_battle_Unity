using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticGui.WorkspaceWindow;
using PlasticPipe.PlasticProtocol.Messages;

namespace UnityGame.Assets.Modules.TileSystem.Scripts.Domain.Tile
{
    [Serializable]
    public class TileOwner
    {
        /// <summary>
        /// key: Tileのキー,Value: userId
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> OwnerRelationship { get; internal set; }
    }
}