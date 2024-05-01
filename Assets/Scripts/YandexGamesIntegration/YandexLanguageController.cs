using I2.Loc;
using UnityEngine;
using YG;

namespace YandexGamesIntegration
{
    public class YandexLanguageController : MonoBehaviour
    {
        private void OnEnable()
        {
            if (YandexGame.SDKEnabled == false)
            {
                YandexGame.GetDataEvent += SetAppLanguage;
                return;
            }

            SetAppLanguage();
        }

        private void OnDisable() => YandexGame.GetDataEvent -= SetAppLanguage;

        private void SetAppLanguage()
        {
            LocalizationManager.CurrentLanguageCode = YandexGame.savesData.language;
        }
    }
}