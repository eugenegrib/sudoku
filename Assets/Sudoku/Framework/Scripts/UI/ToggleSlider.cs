using dotmob;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sudoku.Framework.Scripts.UI
{
    public class ToggleSlider : UIMonoBehaviour, IPointerClickHandler
    {
        #region Inspector Variables

        [SerializeField] private Image musicOnImage = null;
        [SerializeField] private Image musicOffImage = null;
        [SerializeField] private Image soundOnImage = null;
        [SerializeField] private Image soundOffImage = null;
        [SerializeField] private Image mistakesOnImage = null;
        [SerializeField] private Image mistakesOffImage = null;
        [SerializeField] private Image notesOnImage = null;
        [SerializeField] private Image notesOffImage = null;
        [SerializeField] private Image numbersOnImage = null;
        [SerializeField] private Image numbersOffImage = null;

        #endregion

        #region Member Variables

        private bool isMusicOn = true;
        private bool isSoundOn = true;
        private bool isMistakesOn = true;
        private bool isNotesOn = true;
        private bool isNumbersOn = true;

        #endregion

        #region Properties

        public System.Action<bool> OnMusicToggleChanged { get; set; }
        public System.Action<bool> OnSoundToggleChanged { get; set; }
        public System.Action<bool> OnMistakesToggleChanged { get; set; }
        public System.Action<bool> OnNotesToggleChanged { get; set; }
        public System.Action<bool> OnNumbersToggleChanged { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateMusicVisuals(isMusicOn);
            UpdateSoundVisuals(isSoundOn);
            UpdateMistakesVisuals(isMistakesOn);
            UpdateNotesVisuals(isNotesOn);
            UpdateNumbersVisuals(isNumbersOn);
        }

        #endregion

        #region Public Methods

        

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == musicOffImage.gameObject ||
                eventData.pointerCurrentRaycast.gameObject == musicOnImage.gameObject)
            {
                ToggleMusic(!isMusicOn);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == soundOffImage.gameObject ||
                     eventData.pointerCurrentRaycast.gameObject == soundOnImage.gameObject)
            {
                ToggleSound(!isSoundOn);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == mistakesOffImage.gameObject ||
                     eventData.pointerCurrentRaycast.gameObject == mistakesOnImage.gameObject)
            {
                ToggleMistakes(!isMistakesOn);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == notesOffImage.gameObject ||
                     eventData.pointerCurrentRaycast.gameObject == notesOnImage.gameObject)
            {
                ToggleNotes(!isNotesOn);
            }
            else if (eventData.pointerCurrentRaycast.gameObject == numbersOffImage.gameObject ||
                     eventData.pointerCurrentRaycast.gameObject == numbersOnImage.gameObject)
            {
                ToggleNumbers(!isNumbersOn);
            }
        }

        public void Toggle()
        {
            // Метод может быть оставлен, если потребуется общий переключатель
        }

        public void SetMusicToggle(bool on)
        {
            isMusicOn = on;
            UpdateMusicVisuals(on);
            OnMusicToggleChanged?.Invoke(on);
        }

        public void SetSoundToggle(bool on)
        {
            isSoundOn = on;
            UpdateSoundVisuals(on);
            OnSoundToggleChanged?.Invoke(on);
        }

        // Новые методы для установки состояний
        public void SetMistakesToggle(bool on)
        {
            isMistakesOn = on;
            UpdateMistakesVisuals(on);
            OnMistakesToggleChanged?.Invoke(on);
        }

        public void SetNotesToggle(bool on)
        {
            isNotesOn = on;
            UpdateNotesVisuals(on);
            OnNotesToggleChanged?.Invoke(on);
        }

        public void SetNumbersToggle(bool on)
        {
            isNumbersOn = on;
            UpdateNumbersVisuals(on);
            OnNumbersToggleChanged?.Invoke(on);
        }

        #endregion

        #region Private Methods

        private void ToggleMusic(bool state)
        {
            SetMusicToggle(state);
        }

        private void ToggleSound(bool state)
        {
            SetSoundToggle(state);
        }

        private void ToggleMistakes(bool state)
        {
            SetMistakesToggle(state);
        }

        private void ToggleNotes(bool state)
        {
            SetNotesToggle(state);
        }

        private void ToggleNumbers(bool state)
        {
            SetNumbersToggle(state);
        }

        private void UpdateMusicVisuals(bool state)
        {
            musicOffImage.gameObject.SetActive(state);
            musicOnImage.gameObject.SetActive(!state);
        }

        private void UpdateSoundVisuals(bool state)
        {
            soundOffImage.gameObject.SetActive(state);
            soundOnImage.gameObject.SetActive(!state);
        }

        // Новые методы для обновления визуалов
        private void UpdateMistakesVisuals(bool state)
        {
            mistakesOffImage.gameObject.SetActive(state);
            mistakesOnImage.gameObject.SetActive(!state);
        }

        private void UpdateNotesVisuals(bool state)
        {
            notesOffImage.gameObject.SetActive(state);
            notesOnImage.gameObject.SetActive(!state);
        }

        private void UpdateNumbersVisuals(bool state)
        {
            numbersOffImage.gameObject.SetActive(state);
            numbersOnImage.gameObject.SetActive(!state);
        }

        #endregion
    }
}
