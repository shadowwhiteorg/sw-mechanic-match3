using _Game.Interfaces;
using UnityEngine;

namespace _Game.Core
{
    public class SystemRunnerDriver : MonoBehaviour
    {
        private ISystemRunner _runner;

        public void Initialize(ISystemRunner runner)
        {
            _runner = runner;
        }

        private void Update() => _runner?.Tick();
        private void FixedUpdate() => _runner?.FixedTick();
    }
}