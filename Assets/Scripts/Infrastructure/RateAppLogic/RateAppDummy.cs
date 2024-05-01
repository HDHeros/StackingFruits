using System;
using HDH.Popups;
using I2.Loc;
using UI.Popups.Confirmation;
using UnityEngine;

namespace Infrastructure.RateAppLogic
{
    public class RateAppDummy : IRateAppService
    {
        private readonly PopupsController _popups;
        
        public RateAppDummy(PopupsController popups) => 
            _popups = popups;

        public virtual bool IsRateAvailable()
        {
            return true;
        }

        public virtual void RateApp()
        { 
            ShowRateAppPopup();
            Debug.Log("RATEAPP DUMMY");
        }

        protected virtual void ShowRateAppPopup()
        {
            _popups[typeof(ConfirmationPopup)].Open();
            if (_popups[typeof(ConfirmationPopup)].View is ConfirmationPopup cPopup == false)
                throw new InvalidCastException();

            cPopup.Setup(
                LocalizationManager.GetTranslation("RATE_APP_POPUP_HEADER"),
                LocalizationManager.GetTranslation("RATE_APP_POPUP_TEXT"),
                LocalizationManager.GetTranslation("OK"),
                LocalizationManager.GetTranslation("NOT_NOW"), 
                ConfirmationButtonWrapper.Style.Positive, 
                ConfirmationButtonWrapper.Style.Negative, 
                OnPositiveBtnClick,
                null,
                null
                );
        }

        protected virtual void OnPositiveBtnClick()
        {
            
        }
    }
}