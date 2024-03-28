using UnityEngine;

namespace YandexGamesIntegration
{
    public class AT : MonoBehaviour
    {
        public void PageVisible() 
        {
            Debug.Log("Игра в фокусе");
        }

        public void PageNotVisible() 
        {
            Debug.Log("Игра НЕ в фокусе");
        }
    }
}