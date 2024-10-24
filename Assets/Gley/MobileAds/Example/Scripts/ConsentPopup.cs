using Gley.MobileAds.Scripts.ToUse;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gley.MobileAds.Internal
{
    public class ConsentPopup : MonoBehaviour
    {
        [SerializeField] Text popupText;
        UnityAction consentPopupClosed;

        public void Initialize(string popupText, UnityAction consentPopupClosed)
        {
            this.popupText.text = popupText;
            this.consentPopupClosed = consentPopupClosed;
        }


        public void Accept()
        {
            API.SetGDPRConsent(true);
            Close();
        }

        public void Reject()
        {
            API.SetGDPRConsent(false);
            Close();
        }


        public void Close()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            consentPopupClosed?.Invoke();
        }
    }
}