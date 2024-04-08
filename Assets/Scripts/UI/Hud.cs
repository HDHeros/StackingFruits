using System;
using System.Collections.Generic;
using System.Linq;
using HDH.UnityExt.Extensions;
using UI.Screens;
using UnityEngine;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        public enum ScreenType{None = 0, HomeScreen = 1, CommonScreen = 3}
        [Serializable]
        public struct Screen
        {
            public ScreenType Type;
            public BaseScreen View;
        }
        
        [SerializeField] private Screen[] _screens;
        private Dictionary<ScreenType, BaseScreen> _screensDict;
        private BaseScreen _activeScreen;

        public void ActivateScreen(ScreenType type)
        {
            if (_activeScreen.IsNotNull())
                _activeScreen.gameObject.SetActive(false);
            _activeScreen = _screensDict[type];
            _activeScreen.gameObject.SetActive(true);
        }

        private void Awake()
        {
            _screensDict = _screens.ToDictionary(p => p.Type, p => p.View);
            foreach (Screen screen in _screens) 
                screen.View.gameObject.SetActive(false);
        }
    }
}