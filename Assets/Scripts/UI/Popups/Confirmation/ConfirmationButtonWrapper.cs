using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Confirmation
{
    public class ConfirmationButtonWrapper : MonoBehaviour
    {
        public enum Style {Positive, Negative, Transparent, Black}
        [SerializeField] private Image _background;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private List<StyleConfig> _styles;
        public Button Btn => _button;


        
        [Button]
        public void SetStyle(Style style)
        {
            StyleConfig config = _styles.First(s => s.Style == style);
            _background.sprite = config.Sprite;
            _background.pixelsPerUnitMultiplier = config.PixelsPerUnitMultiplier;
            _background.color = config.Color;
            _text.color = config.TextColor;
        }

        public void SetText(string text) => 
            _text.SetText(text);

        [Serializable]
        private struct StyleConfig
        {
            public Style Style;
            public Sprite Sprite;
            public Color Color;
            public Color TextColor;
            public float PixelsPerUnitMultiplier;
        }
    }
}