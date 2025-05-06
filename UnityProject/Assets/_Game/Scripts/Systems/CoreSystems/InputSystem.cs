using UnityEngine;
using _Game.Systems.GridSystem;
using _Game.Interfaces;
using _Game.Utils;

namespace _Game.Systems
{
    public class InputSystem : IUpdatableSystem
    {
        private readonly Camera _camera;
        private readonly GridWorldHelper _gridHelper;
        private readonly IEventBus _eventBus;

        public InputSystem(Camera camera, GridWorldHelper gridHelper, IEventBus eventBus)
        {
            _camera = camera;
            _gridHelper = gridHelper;
            _eventBus = eventBus;
        }

        public void Tick()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Vector3 mouseWorld = _camera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(_camera.transform.position.z))
            );

            if (_gridHelper.TryGetGridPosition(mouseWorld, out int row, out int col))
            {
                _eventBus.Fire(new BlockSelectedEvent(row, col));
            }
        }
    }
}