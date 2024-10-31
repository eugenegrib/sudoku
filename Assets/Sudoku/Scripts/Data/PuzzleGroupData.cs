using System.Collections;
using System.Collections.Generic;
using Sudoku.Scripts.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace dotmob.Sudoku
{
	[System.Serializable]
	public class PuzzleGroupData
	{
		#region Переменные для инспектора

		public string groupId;
		public string displayName;
		public LocalizedString displayNameLocalized;

		/*
		public string displayName;
		*/
		public List<TextAsset> puzzleFiles;

		#endregion

		#region Переменные-члены класса

		private HashSet<int> playedPuzzles; // Содержит индексы всех головоломок, которые были сыграны/начаты пользователем
		private int shiftAmount; // Сдвиг чисел в головоломке, чтобы она выглядела как другая

		#endregion

		#region Свойства

		public int PuzzlesCompleted { get; set; }
		public float MinTime { get; set; }
		public float TotalTime { get; set; }

		#endregion

		#region Публичные методы

		/// <summary>
		/// Возвращает объект, представляющий данные сохранения этой группы
		/// </summary>
		/// <returns>Сохраненные данные.</returns>
		public Dictionary<string, object> Save()
		{
			Dictionary<string, object> groupSaveData = new Dictionary<string, object>();

			groupSaveData["played"] = new List<int>(playedPuzzles);
			groupSaveData["shift_amount"] = shiftAmount;
			groupSaveData["puzzles_completed"] = PuzzlesCompleted;
			groupSaveData["min_time"] = MinTime;
			groupSaveData["total_time"] = TotalTime;

			return groupSaveData;
		}

		/// <summary>
		/// Загружает данные PuzzleGroupData и устанавливает значения из сохранения
		/// </summary>
		public void Load(JSONNode groupSaveData = null)
		{
			playedPuzzles = new HashSet<int>();

			if (groupSaveData != null)
			{
				// Загрузка сыгранных головоломок
				JSONArray playedPuzzlesJson = groupSaveData["played"].AsArray;

				for (int i = 0; i < playedPuzzlesJson.Count; i++)
				{
					playedPuzzles.Add(playedPuzzlesJson[i].AsInt);
				}

				shiftAmount = groupSaveData["shift_amount"].AsInt;
				PuzzlesCompleted = groupSaveData["puzzles_completed"].AsInt;
				MinTime = groupSaveData["min_time"].AsFloat;
				TotalTime = groupSaveData["total_time"].AsFloat;
			}
		}

		/// <summary>
		/// Возвращает данные головоломки для случайной головоломки, которая еще не была сыграна
		/// </summary>
		public PuzzleData GetPuzzle()
		{
			// Проверка, пуста ли коллекция, иначе это может привести к бесконечной рекурсии
			if (puzzleFiles.Count == 0)
			{
				return null;
			}

			for (int i = 0; i < puzzleFiles.Count; i++)
			{
				// Получаем случайный индекс головоломки для следующей игры
				int puzzleIndex = Random.Range(i, puzzleFiles.Count);

				// Проверяем, что головоломка еще не была сыграна
				if (!playedPuzzles.Contains(puzzleIndex))
				{
					// Добавляем индекс головоломки в список сыгранных
					playedPuzzles.Add(puzzleIndex);

					return new PuzzleData(puzzleFiles[puzzleIndex], shiftAmount, groupId);
				}
			}

			/* Если мы здесь, значит, все головоломки были сыграны (или хотя бы начаты) пользователем */

			// Увеличиваем сдвиг, чтобы при повторной игре головоломка казалась другой
			shiftAmount++;

			// Очищаем список сыгранных головоломок, чтобы можно было использовать их снова
			playedPuzzles.Clear();

			// Снова вызываем метод, теперь он точно вернет головоломку
			return GetPuzzle();
		}

		#endregion
	}
}
