using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class SoundToggle : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image soundOnImage = null;
        [SerializeField] private Image soundOffImage = null;

        #endregion

        #region Member Variables

        private bool isSoundOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnSoundToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateSoundVisuals(isSoundOn);
        }

        #endregion

        #region Public Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleSound(!isSoundOn);
        }

        public void SetSoundToggle(bool on)
        {
            isSoundOn = on;
            UpdateSoundVisuals(on);
            OnSoundToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleSound(bool state)
        {
            SetSoundToggle(state);
        }

        private void UpdateSoundVisuals(bool state)
        {
            soundOffImage.gameObject.SetActive(!state);
            soundOnImage.gameObject.SetActive(state);
        }

        #endregion
    }
}