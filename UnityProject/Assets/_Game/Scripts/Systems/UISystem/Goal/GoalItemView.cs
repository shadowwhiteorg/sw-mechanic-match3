using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Systems.UISystem
{
    public class GoalItemView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private GameObject _doneOverlay;

        public void Set(int remaining, Sprite icon)
        {
            _iconImage.sprite = icon;
            _doneOverlay.SetActive(remaining <= 0);
            _countText.text = remaining <= 0 ? string.Empty : remaining.ToString();
        }
    }
}