using System.Collections;
using System.Collections.Generic;
using Sudoku.Framework.Scripts.Save;
using UnityEngine;
using UnityEditor;

namespace dotmob
{
	[CustomEditor(typeof(SaveManager))]
	public class SaveManagerEditor : Editor
	{
		#region Public Methods

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Delete Save File"))
			{
				DeleteSaveData();
			}

			if (GUILayout.Button("Print Save Data To Console"))
			{
				PrintSaveFileToConsole();
			}
		}

		#endregion

		#region Menu Items

		[UnityEditor.MenuItem("Dotmob/Delete Save Data", priority = 0)]
		public static void DeleteSaveData()
		{
			if (!System.IO.File.Exists(SaveManager.SaveFilePath))
			{
				UnityEditor.EditorUtility.DisplayDialog("Delete Save File", "There is no save file.", "Ok");

				return;
			}

			bool delete = UnityEditor.EditorUtility.DisplayDialog("Delete Save File", "Delete the save file located at " + SaveManager.SaveFilePath, "Yes", "No");

			if (delete)
			{
				System.IO.File.Delete(SaveManager.SaveFilePath);

				Debug.Log("Save file deleted");
			}
		}

		[UnityEditor.MenuItem("Dotmob/Print Save Data To Console", priority = 1)] 
		public static void PrintSaveFileToConsole()
		{
			if (!System.IO.File.Exists(SaveManager.SaveFilePath))
			{
				UnityEditor.EditorUtility.DisplayDialog("Delete Save File", "There is no save file.", "Ok");

				return;
			}

			string contents = System.IO.File.ReadAllText(SaveManager.SaveFilePath);

			Debug.Log(contents);
		}

		#endregion
	}
}
