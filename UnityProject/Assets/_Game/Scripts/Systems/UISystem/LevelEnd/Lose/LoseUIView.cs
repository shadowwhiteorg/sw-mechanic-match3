using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Systems.UISystem
{
    public class LoseUIView : BaseUIView
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button          _retryButton;

        /// <summary>Invoked when the Retry button is clicked.</summary>
        public event System.Action OnRetryClicked;

        protected override void OnBind()
        {
            _retryButton.onClick.AddListener(() => OnRetryClicked?.Invoke());
        }

        protected override void OnViewUpdated()
        {
            var m = (LoseUIModel)Model;
            _messageText.text = m.Message;
        }
    }
}