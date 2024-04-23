using HDH.Popups;
using Zenject;

namespace UI.Popups
{
    public class ZenPopupFactory : IPopupViewFactory
    {
        private readonly DiContainer _container;

        public ZenPopupFactory(DiContainer container) => 
            _container = container;

        public PopupView Instantiate(PopupView prefab) => 
            _container.InstantiatePrefabForComponent<PopupView>(prefab);
    }
}