using HDH.UserData;

namespace Infrastructure.Tutor
{
    public class TutorInfoService
    {
        private readonly UserDataService _userData;
        private readonly TutorConfig _tutorConfig;
        private readonly TutorModel _model;
        public TutorConfig Config => _tutorConfig;
        public bool IsTutorCompleted
        {
            get => _model.IsTutorCompleted;
            set
            {
                if (_model.IsTutorCompleted == value) return;
                _model.IsTutorCompleted = value;
                _model.ForceSave();
            }
        }

        public TutorInfoService(UserDataService userData, TutorConfig tutorConfig)
        {
            _userData = userData;
            _tutorConfig = tutorConfig;
            _model = userData.GetModel<TutorModel>();
        }
    }
}