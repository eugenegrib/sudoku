using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class MusicToggle : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image musicOnImage = null;
        [SerializeField] private Image musicOffImage = null;

        #endregion

        #region Member Variables

        private bool isMusicOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnMusicToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateMusicVisuals(isMusicOn);
        }

        #endregion

        #region Public Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleMusic(!isMusicOn);
        }

        public void SetMusicToggle(bool on)
        {
            isMusicOn = on;
            UpdateMusicVisuals(on);
            OnMusicToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleMusic(bool state)
        {
            SetMusicToggle(state);
        }

        private void UpdateMusicVisuals(bool state)
        {
            musicOffImage.gameObject.SetActive(!state);
            musicOnImage.gameObject.SetActive(state);
        }

        #endregion
    }
}