using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Controllers
{
    public class InputController : ITickable
    {
        private readonly SignalBus _signalBus;

        public InputController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider.TryGetComponent<TileHighlighter>(out var highlighter))
                    {
                        _signalBus.Fire(new TileClickedSignal(highlighter));
                    }
                }
            }
        }
    }
}