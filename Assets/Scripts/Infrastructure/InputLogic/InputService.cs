using Infrastructure.InputLogic.Providers;
using Zenject;

namespace Infrastructure.InputLogic
{
    public class InputService
    {
        private readonly BaseInputProvider _inputProvider;
        
        [Inject]
        public InputService()
        {
#if UNITY_EDITOR
            _inputProvider = new StandaloneInputProvider();
#else
            _inputProvider = new MobileInputProvider();
#endif
        }
    }
}