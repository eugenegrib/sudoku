using System.Collections;
using System.Collections.Generic;
using Sudoku.Framework.Scripts.Currency;
using Sudoku.Framework.Scripts.Screen;
using Sudoku.Framework.Scripts.Sound;
using Sudoku.Scripts.Game;
using Sudoku.Scripts.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace dotmob.Sudoku
{
	public class PuzzleBoard : MonoBehaviour
	{
		#region Enums

		private enum CellState
		{
			Blank,
			Clue,
			Placed,
			Hint,
			Incorrect
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private PuzzleBoardCell	puzzleBoardCellPrefab		= null;
		[SerializeField] private Text				noteTextPrefab				= null;
		[SerializeField] private NumberButton		numberButtonPrefab			= null;
		[SerializeField] private GridLayoutGroup	puzzleBoardCellContainer	= null;
		[SerializeField] private Transform			numberButtonContainer		= null;

		[Space]

		[SerializeField] private Text				noteOnOffText				= null;
		[SerializeField] private Image				noteOnOffBkgImage			= null;
		[SerializeField] private Color				noteOnColor					= Color.white;
		[SerializeField] private Color				noteOffColor				= Color.white;
		//List<int> ListMistake = new List<int>(3);
		//int check = 0;
		#endregion

		#region Member Variables

		private PuzzleData					activePuzzleData;
		private ObjectPool					puzzleBoardCellPool;
		private ObjectPool					noteTextPool;
		private ObjectPool					numberButtonPool;

		private List<List<PuzzleBoardCell>>	activeCells;
		private List<NumberButton>			numberButtons;
		private int							selectedCellRow;
		private int							selectedCellCol;
		private bool						isNotesOn;

		#endregion

		#region Public Methods

		/// <summary>
		/// Initialize the puzzle board
		/// </summary>
		public void Initialize()
		{
			puzzleBoardCellPool	= new ObjectPool(puzzleBoardCellPrefab.gameObject, 1, puzzleBoardCellContainer.transform);
			noteTextPool		= new ObjectPool(noteTextPrefab.gameObject, 1, ObjectPool.CreatePoolContainer(transform, "note_pool"));
			numberButtonPool	= new ObjectPool(numberButtonPrefab.gameObject, 1, numberButtonContainer);
			activeCells			= new List<List<PuzzleBoardCell>>();
			numberButtons		= new List<NumberButton>();

			GameManager.Instance.OnGameSettingChanged	+= OnGameSettingChanged;
			ThemeManager.Instance.OnThemeChanged		+= OnThemeChanged;
		}

		/// <summary>
		/// Sets up the puzzle board for the given puzzle data
		/// </summary>
		public void Setup(PuzzleData puzzleData)
		{
			// Set the active puzzle data
			activePuzzleData = puzzleData;

			// Clear the board
			Clear();

			// Create a number button for each number in the level
			SetupNumberButtons();

			// Set the size of the cells
			SetGridCellSize();

			// Setup all the cells on the board
			SetupCells();

			// Set the first cell as the current selected one
			SetCellsSelected(0, 0);

			// Enable all number buttons then call CheckBoard to disable any number buttons that should be disabled
			EnableNumberButtons();
			CheckBoard();
		}

		/// <summary>
		/// Clears all cells from the board
		/// </summary>
		public void Clear()
		{
			puzzleBoardCellPool.ReturnAllObjectsToPool();
			noteTextPool.ReturnAllObjectsToPool();
			numberButtonPool.ReturnAllObjectsToPool();

			activeCells.Clear();
			numberButtons.Clear();

			selectedCellRow = -1;
			selectedCellCol = -1;
		}

		/// <summary>
		/// Sets the selected cells number to the given number
		/// </summary>
		public void SetNumber(int number)
		{
			if (activePuzzleData == null ||
			    selectedCellRow == -1 ||
			    selectedCellCol == -1 ||
			    selectedCellRow >= activeCells.Count ||
			    selectedCellCol >= activeCells[selectedCellRow].Count)
			{
				Debug.LogError("[PuzzleBoard] SetNumber(int number) : Could not set number.");

				return;
			}

			if (number < 1 || number > activePuzzleData.boardSize)
			{
				Debug.LogErrorFormat("[PuzzleBoard] SetNumber(int number) : The given number is out of bounds for the board, number: {0}, bounds:[1,{1}]", number, activePuzzleData.boardSize);
				return;
			}

			// Get the current cell type for the selected cell
			PuzzleData.CellType cellType = activePuzzleData.GetCellType(selectedCellRow, selectedCellCol);

			// Check if the selected cell is a default clue or a hint, those numbers cannot be changed
			if (cellType == PuzzleData.CellType.Clue || cellType == PuzzleData.CellType.Hint)
			{
				return;
			}

			// Start logging moves to undo
			activePuzzleData.BeginUndo();

			if (isNotesOn)
			{
				// Toggle the note and check if it was added or removed
				bool noteAdded = activePuzzleData.ToggleNote(selectedCellRow, selectedCellCol, number);

				if (noteAdded)
				{
					SoundManager.Instance.Play("number-placed");
				}
				else
				{
					SoundManager.Instance.Play("erased");
				}
			}
			else
			{
				// Set the number as the placed number
				bool wasPlaced = activePuzzleData.SetPlacedNumber(selectedCellRow, selectedCellCol, number);
                bool mistake = GameManager.Instance.NumberMistake;
                
				
                // Check if the number was actually placed and changed the state of the board
                if (wasPlaced)
				{
					// Now that a number has been placed, remove any notes
					RemoveNotesForPlacedNumber(selectedCellRow, selectedCellCol, number);

					// Check if the board is now complete
					CheckBoard();
					
					if (GameManager.Instance.ShowIncorrectNumbers &&
					    activePuzzleData.GetCellType(selectedCellRow, selectedCellCol) == PuzzleData.CellType.Incorrect)
					{
                        
						mistake = true;
						//ListMistake.Add(1);
						SoundManager.Instance.Play("number-incorrect");
					}
					else
					{
						SoundManager.Instance.Play("number-placed");
					}
                    
					
				}
                if (mistake)
                {
                    ++GameManager.Instance.checkMistake;
                    if(GameManager.Instance.checkMistake == 2)
                    {
	                    GameManager.Instance.ActiveGameOverPopup();

						GameManager.Instance.checkMistake = 0;
						Debug.Log("Game OVER");
                    }
				}
				
			}
			
			// End logging undos
			activePuzzleData.EndUndo();
			
			// Reset the cell so it displays the correct numbers
			ResetCell(selectedCellRow, selectedCellCol);
		}

		/// <summary>
		/// Toggles setting notes on/off
		/// </summary>
		public void ToggleNotes()
		{
			isNotesOn = !isNotesOn;

			noteOnOffText.text		= isNotesOn ? "ON" : "OFF";
			noteOnOffBkgImage.color	= isNotesOn ? noteOnColor : noteOffColor;

			SoundManager.Instance.Play("note");
		}

		/// <summary>
		/// Undos the last move
		/// </summary>
		public void Undo()
		{
			List<int[]> changedCells;

			if (activePuzzleData.UndoLastMove(out changedCells))
			{
				for (int i = 0; i < changedCells.Count; i++)
				{
					int row = changedCells[i][0];
					int col = changedCells[i][1];

					if (i == 0)
					{
						selectedCellRow = row;
						selectedCellCol = col;
					}

					ResetCell(row, col);
				}

				// Run CheckBoard to re-enable number buttons
				CheckBoard();

				SoundManager.Instance.Play("erased");
			}
		}

		/// <summary>
		/// Removes all numbers on the selected cell
		/// </summary>
		public void Erase()
		{
			// Get the current cell type for the selected cell
			PuzzleData.CellType cellType = activePuzzleData.GetCellType(selectedCellRow, selectedCellCol);

			// Check if the selected cell is a default clue or a hint, those numbers cannot be changed
			if (cellType == PuzzleData.CellType.Clue || cellType == PuzzleData.CellType.Hint)
			{
				return;
			}

			// Begin a new undo to log the erase
			activePuzzleData.BeginUndo();

			// Remove the number from the cell
			activePuzzleData.RemoveNumbers(selectedCellRow, selectedCellCol);

			// End the undo
			activePuzzleData.EndUndo();

			// Reset the numbers on the cell
			ResetCell(selectedCellRow, selectedCellCol);

			// Run CheckBoard to re-enable number buttons
			CheckBoard();

			SoundManager.Instance.Play("erased");
		}

		public void Hint()
		{
			// Получить текущий тип ячейки для выбранной ячейки
			PuzzleData.CellType cellType = activePuzzleData.GetCellType(selectedCellRow, selectedCellCol);

			// Проверить, является ли выбранная ячейка стандартной подсказкой или подсказкой (Hint), такие числа не могут быть изменены
			if (cellType == PuzzleData.CellType.Clue || cellType == PuzzleData.CellType.Hint)
			{
				return;
			}

			// Если удалось потратить одну подсказку, то
			if (CurrencyManager.Instance.TrySpend("hints", 1))
			{
				// Показать подсказку, затем установить число ячейки на то, что было задано
				activePuzzleData.ShowHint(selectedCellRow, selectedCellCol);

				// Сбросить ячейку, чтобы обновить новое число подсказки и стиль
				ResetCell(selectedCellRow, selectedCellCol);

				// Проверить, завершена ли доска
				CheckBoard();

				// Проиграть звук установки подсказки
				SoundManager.Instance.Play("hint-placed");
			}
		}


		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the number buttons needed for the level
		/// </summary>
		private void SetupNumberButtons()
		{
			for (int number = 1; number <= activePuzzleData.boardSize; number++)
			{
				NumberButton numberButton = numberButtonPool.GetObject<NumberButton>();

				numberButtons.Add(numberButton);

				numberButton.Setup(number);

				numberButton.Data				= number;
				numberButton.OnListItemClicked	= OnNumberButtonClicked;
			}
		}

		/// <summary>
		/// Invoked when a number button is clicked
		/// </summary>
		private void OnNumberButtonClicked(int index, object data)
		{
			SetNumber((int)data);
		}

		/// <summary>
		/// Sets the size of the cells on the puzzles grid container
		/// </summary>
		private void SetGridCellSize()
		{
			float gridSize	= (puzzleBoardCellContainer.transform as RectTransform).rect.width;
			float cellSize	= gridSize / activePuzzleData.boardSize;

			puzzleBoardCellContainer.cellSize = new Vector2(cellSize, cellSize);
		}

		/// <summary>
		/// Add all the cells needed to the board
		/// </summary>
		private void SetupCells()
		{
			for (int r = 0; r < activePuzzleData.boardSize; r++)
			{
				activeCells.Add(new List<PuzzleBoardCell>());

				for (int c = 0; c < activePuzzleData.boardSize; c++)
				{
					PuzzleBoardCell puzzleBoardCell = puzzleBoardCellPool.GetObject<PuzzleBoardCell>();

					activeCells[r].Add(puzzleBoardCell);

					puzzleBoardCell.SetUnselected();
					puzzleBoardCell.SetupNotesContainer(activePuzzleData.boxRows, activePuzzleData.boxCols);

					ResetCell(r, c, false);

					// Set the borders for the cell
					bool leftBorder		= (c == 0 || (c % activePuzzleData.boxCols) == 0);
					bool rightBorder	= (c == activePuzzleData.boardSize - 1);
					bool topBorder		= (r == 0 || (r % activePuzzleData.boxRows) == 0);
					bool bottomBorder	= (r == activePuzzleData.boardSize - 1);


					// Set the grid lines for the cell
					bool leftGridLine	= !leftBorder;
					bool topGridLine	= !topBorder;

					puzzleBoardCell.SetGridLines(leftGridLine, topGridLine);
					puzzleBoardCell.SetBorders(leftBorder, rightBorder, topBorder, bottomBorder);

					// Set the on click data/method
					puzzleBoardCell.Data				= new int[] { r, c };
					puzzleBoardCell.OnListItemClicked	= OnCellClicked;
				}
			}
		}

		/// <summary>
		/// Invoked when a cell is clicked on the board
		/// </summary>
		private void OnCellClicked(int index, object data)
		{
			int cellRow = (data as int[])[0];
			int cellCol = (data as int[])[1];

			SetCellsSelected(cellRow, cellCol);
		}

		/// <summary>
		/// Sets the given cell st row/col as the new selected cell
		/// </summary>
		private void SetCellsSelected(int row, int col)
		{
			if (row == selectedCellRow && col == selectedCellCol)
			{
				// Cell is already selected
				return;
			}

			// Set the new selected row/column
			selectedCellRow = row;
			selectedCellCol = col;

			// Update the selected cells
			UpdateSelectedCells();
		}

		/// <summary>
		/// Sets all cells as un-selected and then selects the current selected cells and highlights the current selected cells number
		/// </summary>
		private void UpdateSelectedCells()
		{
			// Set all cells as un-selected
			SetAllCellsUnselected();

			// Set the cells as selected
			SetCellsSelected(selectedCellRow, selectedCellCol, true);

			// Highlight the numbers for the selected cell
			HighlightCells(activeCells[selectedCellRow][selectedCellCol].Number);
		}

		/// <summary>
		/// Sets all the cells as un-selected
		/// </summary>
		private void SetAllCellsUnselected()
		{
			for (int row = 0; row < activePuzzleData.boardSize; row++)
			{
				for (int col = 0; col < activePuzzleData.boardSize; col++)
				{
					activeCells[row][col].SetUnselected();
				}
			}
		}

		/// <summary>
		/// Sets the cell and all seen cells as selected if isSelected is true and un-selected if isSelected is false
		/// </summary>
		private void SetCellsSelected(int row, int col, bool isSelected)
		{
			SetRowSelected(row, isSelected);
			SetColSelected(col, isSelected);

			int boxRow = Mathf.FloorToInt(row / activePuzzleData.boxRows);
			int boxCol = Mathf.FloorToInt(col / activePuzzleData.boxCols);

			SetBoxSelected(boxRow, boxCol, isSelected);

			if (isSelected)
			{
				// Set the primarcy selected cell
				activeCells[row][col].SetSelectedPrimary();
			}
		}

		/// <summary>
		/// Sets the selected state of the given row cells
		/// </summary>
		private void SetRowSelected(int row, bool isSelected)
		{
			for (int col = 0; col < activePuzzleData.boardSize; col++)
			{
				SetCellSelected(row, col, isSelected);
			}
		}

		/// <summary>
		/// Sets the selected state of the given column cells
		/// </summary>
		private void SetColSelected(int col, bool isSelected)
		{
			for (int row = 0; row < activePuzzleData.boardSize; row++)
			{
				SetCellSelected(row, col, isSelected);
			}
		}

		/// <summary>
		/// Sets the selected state of the given box cells
		/// </summary>
		private void SetBoxSelected(int boxRow, int boxCol, bool isSelected)
		{
			for (int row = 0; row < activePuzzleData.boxRows; row++)
			{
				for (int col = 0; col < activePuzzleData.boxCols; col++)
				{
					SetCellSelected(boxRow * activePuzzleData.boxRows + row, boxCol * activePuzzleData.boxCols + col, isSelected);
				}
			}
		}

		/// <summary>
		/// Sets the cell as a secondary selected cell
		/// </summary>
		private void SetCellSelected(int row, int col, bool isSelected)
		{
			if (isSelected)
			{
				activeCells[row][col].SetSelectedSecondary();
			}
			else
			{
				activeCells[row][col].SetUnselected();
			}
		}

		/// <summary>
		/// Sets the cells with the given number as highlighted
		/// </summary>
		private void HighlightCells(int number)
		{
			if (number == -1)
			{
				return;
			}

			for (int row = 0; row < activePuzzleData.boardSize; row++)
			{
				for (int col = 0; col < activePuzzleData.boardSize; col++)
				{
					PuzzleBoardCell cell = activeCells[row][col];

					if (number == cell.Number)
					{
						cell.SetHighlighted();
					}
				}
			}
		}

		/// <summary>
		/// Clears the cell of all notes and numbers and sets the notes and numbers from the active puzzle data
		/// </summary>
		private void ResetCell(int row, int col, bool updateSelectedCells = true)
		{
			PuzzleBoardCell cell = activeCells[row][col];

			// Remove all text from the cell
			cell.SetBlank();
			cell.RemoveAllNotes();

			// Get the cells type from the puzzle data
			PuzzleData.CellType cellType = activePuzzleData.GetCellType(row, col);

			if (cellType == PuzzleData.CellType.Blank)
			{
				// If the cell is blank set any notes that should be on the cell
				List<int> notes = activePuzzleData.GetNotes(row, col);

				for (int i = 0; i < notes.Count; i++)
				{
					cell.AddNote(notes[i], noteTextPool);
				}
			}
			else
			{
				// Set the number
				cell.SetNumber(activePuzzleData.GetNumber(row, col));

				// Set the text style
				switch (cellType)
				{
					case PuzzleData.CellType.Clue:
						cell.SetTextClue();
						break;
					case PuzzleData.CellType.Hint:
						cell.SetTextHint();
						break;
					case PuzzleData.CellType.Incorrect:
						if (GameManager.Instance.ShowIncorrectNumbers)
						{
							cell.SetTextIncorrect();
						}
						else
						{
							cell.SetTextPlaced();
						}
						break;
					case PuzzleData.CellType.Placed:
						cell.SetTextPlaced();
						break;
				}
			}

			if (updateSelectedCells)
			{
				// Update the cells now that a new number has been placed
				UpdateSelectedCells();
			}
		}

		/// <summary>
		/// Removes notes in seen cells
		/// </summary>
		private void RemoveNotesForPlacedNumber(int row, int col, int number)
		{
			// Check if we should remove the notes
			if (GameManager.Instance.RemoveNumbersFromNotes &&
			    (!GameManager.Instance.ShowIncorrectNumbers || activePuzzleData.GetCellType(row, col) != PuzzleData.CellType.Incorrect))
			{
				RemoveNotesFromRow(row, number);
				RemoveNotesFromCol(col, number);

				int boxRow = Mathf.FloorToInt(row / activePuzzleData.boxRows);
				int boxCol = Mathf.FloorToInt(col / activePuzzleData.boxCols);

				RemoveNotesFromBox(boxRow, boxCol, number);
			}
		}

		/// <summary>
		/// Sets the selected state of the given row cells
		/// </summary>
		private void RemoveNotesFromRow(int row, int number)
		{
			for (int col = 0; col < activePuzzleData.boardSize; col++)
			{
				if (activePuzzleData.RemoveNote(row, col, number))
				{
					ResetCell(row, col, false);
				}
			}
		}

		/// <summary>
		/// Sets the selected state of the given column cells
		/// </summary>
		private void RemoveNotesFromCol(int col, int number)
		{
			for (int row = 0; row < activePuzzleData.boardSize; row++)
			{
				if (activePuzzleData.RemoveNote(row, col, number))
				{
					ResetCell(row, col, false);
				}
			}
		}

		/// <summary>
		/// Sets the selected state of the given box cells
		/// </summary>
		private void RemoveNotesFromBox(int boxRow, int boxCol, int number)
		{
			for (int row = 0; row < activePuzzleData.boxRows; row++)
			{
				for (int col = 0; col < activePuzzleData.boxCols; col++)
				{
					int cellRow = boxRow * activePuzzleData.boxRows + row;
					int cellCol = boxCol * activePuzzleData.boxCols + col;

					if (activePuzzleData.RemoveNote(cellRow, cellCol, number))
					{
						ResetCell(cellRow, cellCol, false);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the board is solved
		/// </summary>
		private void CheckBoard()
		{
			List<int> numberCounts;

			bool isSolved = activePuzzleData.CheckBoard(out numberCounts);

			if (isSolved)
			{
				GameManager.Instance.ActivePuzzleCompleted();

				SoundManager.Instance.Play("puzzle-complete");
			}

			if (GameManager.Instance.HideAllPlacedNumbers)
			{
				for (int i = 0; i < numberCounts.Count; i++)
				{
					numberButtons[i].SetVisible(numberCounts[i] < activePuzzleData.boardSize);
				}
			}
		}

		/// <summary>
		/// Sets all number buttons to active
		/// </summary>
		private void EnableNumberButtons()
		{
			for (int i = 0; i < numberButtons.Count; i++)
			{
				numberButtons[i].SetVisible(true);
			}
		}

		/// <summary>
		/// Invoked when a game setting has changed
		/// </summary>
		private void OnGameSettingChanged(string setting)
		{
			if (activePuzzleData != null)
			{
				Setup(activePuzzleData);
			}
		}

		/// <summary>
		/// Invoked when the active theme in the ThemeManager has changed
		/// </summary>
		private void OnThemeChanged()
		{
			if (activePuzzleData != null)
			{
				Setup(activePuzzleData);
			}
		}

		#endregion

		#region Debug Methods

		#if UNITY_EDITOR

		//[UnityEditor.MenuItem("Dotmob/Complete Active Puzzle", priority = 200)]
		public static void CompleteActivePuzzle()
		{
			PuzzleBoard puzzleBoard = FindObjectOfType<PuzzleBoard>();
			PuzzleData puzzleData = GameManager.Instance.ActivePuzzleData;

			for (int r = 0; r < puzzleData.boardSize; r++)
			{
				for (int c = 0; c < puzzleData.boardSize; c++)
				{
					puzzleBoard.selectedCellRow = r;
					puzzleBoard.selectedCellCol = c;

					puzzleBoard.SetNumber(puzzleData.GetCorrectNumber(r, c));
				}
			}
		}

		//[UnityEditor.MenuItem("Dotmob/Complete Active Puzzle", priority = 200, validate = true)]
		public static bool CompleteActivePuzzleValidate()
		{
			return Application.isPlaying && GameManager.Instance.ActivePuzzleData != null && ScreenManager.Instance.CurrentScreenId == "game";
		}

		#endif

		#endregion
	}
}
