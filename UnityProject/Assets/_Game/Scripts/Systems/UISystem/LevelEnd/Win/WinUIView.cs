using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Systems.UISystem
{
    public class WinUIView : BaseUIView
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button          _nextButton;
        [SerializeField] private Button          _retryButton;

        /// <summary>Invoked when the Next button is clicked.</summary>
        public event System.Action OnNextClicked;
        /// <summary>Invoked when the Retry button is clicked.</summary>
        public event System.Action OnRetryClicked;

        protected override void OnBind()
        {
            _nextButton .onClick.AddListener(() => OnNextClicked?.Invoke());
            _retryButton.onClick.AddListener(() => OnRetryClicked?.Invoke());
        }

        protected override void OnViewUpdated()
        {
            var m = (WinUIModel)Model;
            _messageText.text = m.Message;
        }
    }
}