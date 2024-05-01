namespace Infrastructure.RateAppLogic
{
    public interface IRateAppService
    {
        public void RateApp();
        public bool IsRateAvailable();
    }
}