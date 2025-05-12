using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Systems.UISystem
{
    public class WinUIView : BaseUIView
    {
        [SerializeField] private TextMeshProUGUI levelNumberText;
        [SerializeField] private Button          nextButton;

        public event System.Action OnNextClicked;

        protected override void OnBind()
        {
            nextButton .onClick.AddListener(() => OnNextClicked?.Invoke());
        }

        protected override void OnViewUpdated()
        {
            var m = (WinUIModel)Model;
            levelNumberText.text = m.Message;
        }
    }
}