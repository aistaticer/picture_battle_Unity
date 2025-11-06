using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class GameLogicContext
{
    public ClickEvent _clickEvent { get; set; }

    public ClickState _clickState { get; set; }

    public TileSpawner spawner { get; set; }
}
