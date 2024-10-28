using System.Collections.Generic;
using dotmob;
using dotmob.Sudoku;
using Sudoku.Framework.Scripts.Currency;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Framework.Scripts.Screen;
using Sudoku.Framework.Scripts.Sound;
using UnityEngine;

namespace Sudoku.Scripts.Game
{
    public class GameManager : SingletonComponent<GameManager>, ISaveable
    {

        #region Inspector Variables

        [Header("Data")] [SerializeField] private List<PuzzleGroupData> puzzleGroups = null;

        [Header("Values")] [SerializeField] private int hintsPerCompletedPuzzle = 1;
        [SerializeField] private int numLevelsBetweenAds = 3;
        [SerializeField] private bool numberMistake = false;

        public int checkMistake = 0;

        [Header("Game Settings")] [SerializeField]
        private bool showIncorrectNumbers = true;

        [SerializeField] private bool removeNumbersFromNotes = true;
        [SerializeField] private bool hideAllPlacedNumbers = true;

        [Header("Components")] [SerializeField]
        private PuzzleBoard puzzleBoard = null;

        #endregion

        #region Properties

        public string SaveId
        {
            get { return "game_manager"; }
        }

        public bool IsPaused { get; private set; }
        public PuzzleData ActivePuzzleData { get; private set; }
        public int NumLevelsTillAdd { get; private set; }

        public List<PuzzleGroupData> PuzzleGroupDatas
        {
            get { return puzzleGroups; }
        }

        public bool ShowIncorrectNumbers
        {
            get { return showIncorrectNumbers; }
        }

        public bool RemoveNumbersFromNotes
        {
            get { return removeNumbersFromNotes; }
        }

        public bool HideAllPlacedNumbers
        {
            get { return hideAllPlacedNumbers; }
        }

        public int HintsPerCompletedPuzzle
        {
            get { return hintsPerCompletedPuzzle; }
        }

        public bool NumberMistake
        {
            get { return numberMistake; }
        }

        public System.Action<string> OnGameSettingChanged { get; set; }

        public string ActivePuzzleDifficultyStr
        {
            get
            {
                if (ActivePuzzleData == null) return "";

                PuzzleGroupData puzzleGroupData = GetPuzzleGroup(ActivePuzzleData.groupId);

                if (puzzleGroupData == null) return "";

                return puzzleGroupData.displayName.GetLocalizedString();
            }
        }

        public string ActivePuzzleTimeStr
        {
            get
            {
                if (ActivePuzzleData == null) return "00:00";

                return Utilities.FormatTimer(ActivePuzzleData.elapsedTime);
            }
        }

        #endregion

        public void PauseGame()
        {
            if (!IsPaused)
            {
                IsPaused = true;
                //Time.timeScale = 0; // Останавливаем все, что зависит от времени
                SoundManager.Instance.PauseMusic(); // Если нужно приостановить музыку
                // Дополнительная логика при паузе
            }
        }

        public void RestartGame()
        {              
            IsPaused = false;

            var puzzleGroup = GetPuzzleGroup(ActivePuzzleData.groupId);

            //SoundManager.Instance.Play("lose");

            object[] popupData =
            {
                puzzleGroup.displayName,
            };
            // If the popup was closed without the cancelled flag being set then the player selected New Game
            if (puzzleGroup != null)
            {
                PlayNewGame(puzzleGroup);
            }

            ActivePuzzleData.elapsedTime = 0;
            //SoundManager.Instance.PlayAtStart(SoundManager.SoundType.Music);
            SoundManager.Instance.ResumeMusic(); 
        }

        public void ResumeGame()
        {
            if (IsPaused)
            {
                IsPaused = false;
                SoundManager.Instance.ResumeMusic(); // Включаем музыку заново, если она была приостановлена
                // Дополнительная логика при возобновлении игры
            }
        }


        #region Unity Methods

        protected override void Awake()
        {
           // API.ShowBanner(BannerPosition.Bottom, BannerType.Adaptive);

            base.Awake();

            SaveManager.Instance.Register(this);

            puzzleBoard.Initialize();

            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                puzzleGroups[i].Load();
            }

            LoadSave();
        }

        private void Update()
        {
            if (!IsPaused && ActivePuzzleData != null && ScreenManager.Instance.CurrentScreenId == "game")
            {
                ActivePuzzleData.elapsedTime += Time.deltaTime;
            }
        }

        #endregion

        #region Public Methods

        public void PlayNewGame(int groupIndex)
        {
            // Make sure the groupIndex is within the bounds of puzzleGroups
            if (groupIndex >= 0 && groupIndex < puzzleGroups.Count)
            {
                PlayNewGame(puzzleGroups[groupIndex]);

                return;
            }

            Debug.LogErrorFormat(
                "[GameManager] PlayNewGame(int groupIndex) : The given groupIndex ({0}) is out of bounds for the puzzleGroups of size {1} \"{0}\"",
                groupIndex, puzzleGroups.Count);
        }

        public void PlayNewGame(string groupId)
        {
            // Get the PuzzleGroupData for the given groupId
            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroupData = puzzleGroups[i];

                if (groupId == puzzleGroupData.groupId)
                {
                    PlayNewGame(puzzleGroupData);

                    return;
                }
            }

            Debug.LogErrorFormat(
                "[GameManager] PlayNewGame(string groupId) : Could not find a PuzzleGroupData with the given id \"{0}\"",
                groupId);
        }

        public void ContinueActiveGame()
        {
            PlayGame(ActivePuzzleData);
        }

        public void SetGameSetting(string setting, bool value)
        {
            switch (setting)
            {
                case "mistakes":
                    showIncorrectNumbers = value;
                    //Debug.Log("Dem so lan sai");
                    break;
                case "notes":
                    removeNumbersFromNotes = value;
                    break;
                case "numbers":
                    hideAllPlacedNumbers = value;
                    break;
            }

            if (OnGameSettingChanged != null)
            {
                OnGameSettingChanged(setting);
            }
        }


        public void ActiveGameOverPopup()
        {
            var puzzleGroup = GetPuzzleGroup(ActivePuzzleData.groupId);

            Debug.Log("NAME :" + puzzleGroup.displayName);


            SoundManager.Instance.Play("lose");

            object[] popupData =
            {
                puzzleGroup.displayName,
            };

            // Show the puzzle complete popup
            PopupManager.Instance.Show("game_over", popupData, (bool cancelled, object[] outData) =>
            {
                // If the popup was closed without the cancelled flag being set then the player selected New Game
                if (!cancelled && puzzleGroup != null)
                {
                    PlayNewGame(puzzleGroup);
                }

                // Else go back to the main screen
                else
                {
                    PopupManager.Instance.CloseActivePopup();

                    //ScreenManager.Instance.Back();
                }
            });
        }

        public void ActivePuzzleCompleted()
        {
            // Get the PuzzleGroupData for the puzzle
            PuzzleGroupData puzzleGroup = GetPuzzleGroup(ActivePuzzleData.groupId);
            float elapsedTime = ActivePuzzleData.elapsedTime;

            // Set the puzzle data to null now so the game can't be continued
            ActivePuzzleData = null;

            puzzleGroup.PuzzlesCompleted += 1;
            puzzleGroup.TotalTime += elapsedTime;

            bool newBest = false;

            if (puzzleGroup.MinTime == 0 || elapsedTime < puzzleGroup.MinTime)
            {
                newBest = true;
                puzzleGroup.MinTime = elapsedTime;
            }

            // Award the player their hint
            CurrencyManager.Instance.Give("hints", hintsPerCompletedPuzzle);

            object[] popupData =
            {
                puzzleGroup.displayName,
                elapsedTime,
                puzzleGroup.MinTime,
                newBest
            };

            // Show the puzzle complete popup
            PopupManager.Instance.Show("puzzle_complete", popupData, (bool cancelled, object[] outData) =>
            {
                // If the popup was closed without the cancelled flag being set then the player selected New Game
                if (!cancelled && puzzleGroup != null)
                {
                    PlayNewGame(puzzleGroup);
                }
                // Else go back to the main screen
                else
                {
                    //ScreenManager.Instance.Back();
                }
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a new PuzzleData from the given PuzzleGroupData and sets up the game to play it
        /// </summary>
        private void PlayNewGame(PuzzleGroupData puzzleGroupData)
        {
            // Get a puzzle that has not yet been played by the user
            PuzzleData puzzleData = puzzleGroupData.GetPuzzle();

            // Play the game using the new puzzle data
            PlayGame(puzzleData);
        }

        /// <summary>
        /// Starts the game using the given PuzzleData
        /// </summary>
        private void PlayGame(PuzzleData puzzleData)
        {
            // Set the active puzzle dat
            ActivePuzzleData = puzzleData;

            // Setup the puzzle board to display the numbers
            puzzleBoard.Setup(puzzleData);

            // Show the game screen
            ScreenManager.Instance.Show("game");

            NumLevelsTillAdd++;

            if (NumLevelsTillAdd > numLevelsBetweenAds)
            {
                //	API.ShowInterstitial();
                NumLevelsTillAdd = 0;
            }

            SoundManager.Instance.StartPlayMusic();
        }

       
        /// <summary>
        /// Gets the puzzle group with the given id
        /// </summary>
        private PuzzleGroupData GetPuzzleGroup(string id)
        {
            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroup = puzzleGroups[i];

                if (id == puzzleGroup.groupId)
                {
                    return puzzleGroup;
                }
            }

            return null;
        }

        #endregion

        #region Save Methods

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();

            // Save the active puzzle if there is one
            if (ActivePuzzleData != null)
            {
                saveData["activePuzzle"] = ActivePuzzleData.Save();
            }

            // Save all the puzzle groups data
            List<object> savedPuzzleGroups = new List<object>();

            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroup = puzzleGroups[i];
                Dictionary<string, object> savedPuzzleGroup = new Dictionary<string, object>();

                savedPuzzleGroup["id"] = puzzleGroup.groupId;
                savedPuzzleGroup["data"] = puzzleGroup.Save();

                savedPuzzleGroups.Add(savedPuzzleGroup);
            }

            saveData["savedPuzzleGroups"] = savedPuzzleGroups;

            // Save the game settings
            saveData["showIncorrectNumbers"] = showIncorrectNumbers;
            saveData["removeNumbersFromNotes"] = removeNumbersFromNotes;
            saveData["hideAllPlacedNumbers"] = hideAllPlacedNumbers;
            saveData["NumLevelsTillAdd"] = NumLevelsTillAdd;

            return saveData;
        }

        private bool LoadSave()
        {
            JSONNode json = SaveManager.Instance.LoadSave(this);

            if (json == null)
            {
                return false;
            }

            // If there is a saved active puzzle load it
            if (json.AsObject.HasKey("activePuzzle"))
            {
                ActivePuzzleData = new PuzzleData(json["activePuzzle"]);
            }

            // Load the saved group data
            JSONArray savedPuzzleGroups = json["savedPuzzleGroups"].AsArray;

            for (int i = 0; i < savedPuzzleGroups.Count; i++)
            {
                JSONNode savedPuzzleGroup = savedPuzzleGroups[i];
                PuzzleGroupData puzzleGroup = GetPuzzleGroup(savedPuzzleGroup["id"].Value);

                if (puzzleGroup != null)
                {
                    puzzleGroup.Load(savedPuzzleGroup["data"]);
                }
            }

            // Load the game settings
            showIncorrectNumbers = json["showIncorrectNumbers"].AsBool;
            removeNumbersFromNotes = json["removeNumbersFromNotes"].AsBool;
            hideAllPlacedNumbers = json["hideAllPlacedNumbers"].AsBool;
            NumLevelsTillAdd = json["NumLevelsTillAdd"].AsInt;

            return true;
        }

        #endregion
    }
}