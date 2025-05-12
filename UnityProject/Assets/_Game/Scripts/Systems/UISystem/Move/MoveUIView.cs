using TMPro;
using UnityEngine;

namespace _Game.Systems.UISystem.Move
{
    public class MoveUIView : BaseUIView
    {
        [SerializeField] private TextMeshProUGUI movesText;

        protected override void OnBind()
        {
            // No special wiring needed here
        }

        protected override void OnViewUpdated()
        {
            var m = (MoveUIModel)Model;
            movesText.text = $"{m.MovesLeft}";
        }
    }
}