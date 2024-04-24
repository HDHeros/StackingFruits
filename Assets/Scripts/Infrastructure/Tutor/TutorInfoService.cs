using HDH.UserData;

namespace Infrastructure.Tutor
{
    public class TutorInfoService
    {
        private readonly UserDataService _userData;
        private readonly TutorConfig _tutorConfig;
        public TutorConfig Config => _tutorConfig;
        public bool IsTutorCompleted => false;

        public TutorInfoService(UserDataService userData, TutorConfig tutorConfig)
        {
            _userData = userData;
            _tutorConfig = tutorConfig;
        }
    }
}