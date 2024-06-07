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
        public enum ScreenType{None = 0, HomeScreen = 1, EmptyScreen = 2, CommonScreen = 3, GameInProgressScreen = 4,}
        [Serializable]
        public struct Screen
        {
            public ScreenType Type;
            public BaseScreen View;
        }
        
        [SerializeField] private Screen[] _screens;
        private Dictionary<ScreenType, BaseScreen> _screensDict;
        private Stack<BaseScreen> _screensStack;
        private BaseScreen _activeScreen;

        public void PushScreen(ScreenType type) => 
            PushScreen<BaseScreen>(type);

        public T PushScreen<T>(ScreenType type) where T : BaseScreen
        {
            if (_activeScreen.IsNotNull())
                _activeScreen.gameObject.SetActive(false);
            _activeScreen = _screensDict[type];
            _activeScreen.gameObject.SetActive(true);
            _screensStack.Push(_activeScreen);
            return _activeScreen as T;
        }

        public void PopScreen(ScreenType type)
        {
            BaseScreen targetScreen = _screensDict[type];
            if (targetScreen != _activeScreen)
            {
                Debug.LogWarning("Trying to pop screen that is not on the top of the stack");
                return;
            }
            _activeScreen.gameObject.SetActive(false);
            _activeScreen = null;
            if (_screensStack.Count > 0)
                _screensStack.Pop();
            if (_screensStack.Count > 0)
            {
                _activeScreen = _screensStack.Peek();
                _activeScreen.gameObject.SetActive(true);
            }
        }

        private void Awake()
        {
            _screensStack = new Stack<BaseScreen>(6);
            _screensDict = _screens.ToDictionary(p => p.Type, p => p.View);
            foreach (Screen screen in _screens) 
                screen.View.gameObject.SetActive(false);
        }
    }
}