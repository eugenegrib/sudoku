using Sudoku.Framework.Scripts.Sound;
using Sudoku.Scripts.Game;
using UnityEngine;

namespace Sudoku.Framework.Scripts.UI
{
    public class SettingsPopup : Popup.Popup
    {
        public override void Hide(bool result)
        {
            base.Hide(result);
        }

        #region Inspector Variables

        [Space]
        [SerializeField] private MusicToggle musicToggle = null;
        [SerializeField] private SoundToggle soundToggle = null;
        [SerializeField] private MistakesToggle mistakesToggle = null;
        [SerializeField] private NotesToggle notesToggle = null;
        [SerializeField] private NumbersToggle numbersToggle = null;

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeMusicToggle();
            InitializeSoundToggle();
            InitializeMistakesToggle();
            InitializeNotesToggle();
            InitializeNumbersToggle();
        }

        #endregion

        #region Private Methods

        private void InitializeMusicToggle()
        {
            musicToggle.SetMusicToggle(SoundManager.Instance.IsMusicOn);
            musicToggle.OnMusicToggleChanged += OnMusicValueChanged;
        }

        private void InitializeSoundToggle()
        {
            soundToggle.SetSoundToggle(SoundManager.Instance.IsSoundEffectsOn);
            soundToggle.OnSoundToggleChanged += OnSoundEffectsValueChanged;
        }

        private void InitializeMistakesToggle()
        {
            mistakesToggle.SetMistakesToggle(GameManager.Instance.ShowIncorrectNumbers);
            mistakesToggle.OnMistakesToggleChanged += OnMistakesToggleChanged;
        }

        private void InitializeNotesToggle()
        {
            notesToggle.SetNotesToggle(GameManager.Instance.RemoveNumbersFromNotes);
            notesToggle.OnNotesToggleChanged += OnNotesToggleChanged;
        }

        private void InitializeNumbersToggle()
        {
            numbersToggle.SetNumbersToggle(GameManager.Instance.HideAllPlacedNumbers);
            numbersToggle.OnNumbersToggleChanged += OnNumbersToggleChanged;
        }

        private void OnMusicValueChanged(bool isOn)
        {
            SoundManager.Instance.SetSoundTypeOnOff(SoundManager.SoundType.Music, isOn);
        }

        private void OnSoundEffectsValueChanged(bool isOn)
        {
            SoundManager.Instance.SetSoundTypeOnOff(SoundManager.SoundType.SoundEffect, isOn);
        }

        private void OnMistakesToggleChanged(bool isOn)
        {
            GameManager.Instance.SetGameSetting("mistakes", isOn);
        }

        private void OnNotesToggleChanged(bool isOn)
        {
            GameManager.Instance.SetGameSetting("notes", isOn);
        }

        private void OnNumbersToggleChanged(bool isOn)
        {
            GameManager.Instance.SetGameSetting("numbers", isOn);
        }

        #endregion
    }
}
