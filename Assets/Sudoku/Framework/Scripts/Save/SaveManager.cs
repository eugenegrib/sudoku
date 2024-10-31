using System.Collections.Generic;
using System.Linq;
using dotmob;
using UnityEngine;

namespace Sudoku.Framework.Scripts.Save
{
	public class SaveManager : SingletonComponent<SaveManager>
	{
		#region Member Variables

		private List<ISaveable>	saveables;
		private JSONNode		loadedSave;

		#endregion

		#region Properties

		/// <summary>
		/// Path to the save file on the device
		/// </summary>
		public static string SaveFilePath => Application.persistentDataPath + "/save.json";

		/// <summary>
		/// List of registered saveables
		/// </summary>
		private List<ISaveable> Saveables
		{
			get
			{
				if (saveables == null)
				{
					saveables = new List<ISaveable>();
				}

				return saveables;
			}
		}

		#endregion

		#region Unity Methods

		private void Start()
		{
			Debug.Log("Save file path: " + SaveFilePath);
		}

		private void OnDestroy()
		{
			Save();
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				Save();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registers a saveable to be saved
		/// </summary>
		public void Register(ISaveable saveable)
		{
			Saveables.Add(saveable);
		}

		/// <summary>
		/// Loads the save data for the given saveable
		/// </summary>
		public JSONNode LoadSave(ISaveable saveable)
		{
			return LoadSave(saveable.SaveId);
		}

		/// <summary>
		/// Loads the save data for the given save id
		/// </summary>
		public JSONNode LoadSave(string saveId)
		{
			// Check if the save file has been loaded and if not try and load it
			if (loadedSave == null && !LoadSave(out loadedSave))
			{
				return null;
			}

			// Check if the loaded save file has the given save id
			if (!loadedSave.AsObject.HasKey(saveId))
			{
				return null;
			}

			// Return the JSONNode for the save id
			return loadedSave[saveId];
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Saves all registered saveables to the save file
		/// </summary>
		public void Save()
		{
			var saveJson = saveables.ToDictionary<ISaveable, string, object>(t => t.SaveId, t => t.Save());

			System.IO.File.WriteAllText(SaveFilePath, Utilities.ConvertToJsonString(saveJson));
		}

		/// <summary>
		/// Tries to load the save file
		/// </summary>
		private bool LoadSave(out JSONNode json)
		{
			json = null;

			if (!System.IO.File.Exists(SaveFilePath))
			{
				return false;
			}

			json = JSON.Parse(System.IO.File.ReadAllText(SaveFilePath));

			return json != null;
		}

		#endregion
	}
}
