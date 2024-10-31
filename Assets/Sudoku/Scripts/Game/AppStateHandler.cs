using GoogleMobileAds.Sample;
using Sudoku.Scripts.Ads;
using UnityEngine;

namespace Sudoku.Scripts.Game
{
    public class AppStateHandler : MonoBehaviour
    {
        
        [SerializeField] private AppOpenAdController appOpenAdController;

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Приложение переходит в фоновый режим
                Debug.Log("Приложение в фоновом режиме");
                // Здесь вы можете обработать ситуацию, когда приложение приостанавливается
            }
            else
            {
                appOpenAdController.ShowAd();

                
                // Приложение возвращается в передний план
                Debug.Log("Приложение вернулось в передний план");
                // Здесь вы можете обработать ситуацию, когда приложение активируется
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                // Приложение получает фокус
                Debug.Log("Приложение получило фокус");
            }
            else
            {
                // Приложение теряет фокус
                Debug.Log("Приложение потеряло фокус");
            }
        }
    }
}