using System;
using Gameplay.LevelsLogic;
using HDH.UserData;
using Infrastructure.Tutor;
using YG;

namespace YandexGamesIntegration
{
    public class SaveLoadYandex : ISaveLoadService
    {
        private const string TutorDataModelName = nameof(TutorModel);
        private const string LevelsDataModelName = nameof(LevelsModel);

        
        public bool IsInitialized => YandexGame.SDKEnabled;
        
        public void Save<T>(T model) where T : DataModel, new()
        {
            string typeName = model.GetType().Name;
            switch (typeName)
            {
                case TutorDataModelName:
                    if (model is TutorModel tdm == false) throw new ArgumentException();
                    YandexGame.savesData.TutorDataModel = tdm;
                    break;
                case LevelsDataModelName:
                    if (model is LevelsModel ldm == false) throw new ArgumentException();
                    YandexGame.savesData.LevelsDataModel = ldm;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            YandexGame.SaveProgress();
        }

        public T Load<T>() where T : DataModel, new()
        {
            string typeName = typeof(T).Name;
            switch (typeName)
            {
                case TutorDataModelName:
                    YandexGame.savesData.TutorDataModel ??= new TutorModel();
                    return (T)(object)YandexGame.savesData.TutorDataModel;
                case LevelsDataModelName:
                    YandexGame.savesData.LevelsDataModel ??= new LevelsModel();
                    return (T)(object)YandexGame.savesData.LevelsDataModel;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}