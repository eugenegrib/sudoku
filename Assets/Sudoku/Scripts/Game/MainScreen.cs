using UnityEngine;
using UnityEngine.UI;
using Screen = Sudoku.Framework.Scripts.Screen.Screen;

namespace Sudoku.Scripts.Game
{
	public class MainScreen : Screen
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private GameObject	continueButton			= null;
		[SerializeField] private Text		continueText	= null;
		[SerializeField] private Text		continueTimeText		= null;

        #endregion

        #region Public Methods


        

		public override void Show(bool back, bool immediate)
		{
			base.Show(back, immediate);

			continueButton.SetActive(GameManager.Instance.ActivePuzzleData != null);

			if (GameManager.Instance.ActivePuzzleData != null)
			{
				continueText.text	= " : " + GameManager.Instance.ActivePuzzleDifficultyStr;
				continueTimeText.text		= "" +GameManager.Instance.ActivePuzzleTimeStr;
			}
		}

		#endregion
	}
}
