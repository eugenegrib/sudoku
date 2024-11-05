using dotmob;
using Sudoku.Framework.Scripts.Popup;
using Sudoku.Framework.Scripts.Utilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Sudoku.Scripts.Game
{
	public class PuzzleCompletePopup : Popup
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private Text		difficultyNameText	= null;
		[SerializeField] private Text		puzzleTimeText		= null;
		[SerializeField] private Text		bestTimeText		= null;
		[SerializeField] private GameObject	newBestIndicator	= null;
		[SerializeField] private Text		awardedHintsText	= null;

		#endregion

		#region Public Methods

		public override void OnShowing(object[] inData)
		{
			base.OnShowing(inData);

			var	groupName	= (LocalizedString)inData[0];
			var	puzzleTime	= (float)inData[1];
			var	bestTime	= (float)inData[2];
			var	newBest		= (bool)inData[3];

			difficultyNameText.text	= groupName.GetLocalizedString();
			puzzleTimeText.text		= Utilities.FormatTimer(puzzleTime);
			bestTimeText.text		= Utilities.FormatTimer(bestTime);

			newBestIndicator.SetActive(newBest);

			awardedHintsText.text = "+" + GameManager.Instance.HintsPerCompletedPuzzle;
		}

		#endregion
	}
}
