using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TileClickEvent : IClickable
{
    [Inject] private SignalBus _signalBus;

    public void OnClick(GameObject clickedObject)
    {
        if (!clickedObject.TryGetComponent<Tile>(out var tile))
            return;
        
        _signalBus.Fire(new TileClickedSignal(tile));
    }
}
