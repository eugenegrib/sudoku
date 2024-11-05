using Sudoku.Scripts.Theme;
using UnityEditor;
using UnityEngine;

namespace Sudoku.Editor
{
	[CustomEditor(typeof(ThemeManager))]
	public class ThemeManagerEditor : UnityEditor.Editor
	{
		#region Member Variables

		private Texture2D lineTexture;

		#endregion

		#region Properties

		private Texture2D LineTexture
		{
			get
			{
				if (lineTexture == null)
				{
					lineTexture = new Texture2D(1, 1);
					lineTexture.SetPixel(0, 0, new Color(37f/255f, 37f/255f, 37f/255f));
					lineTexture.Apply();
				}

				return lineTexture;
			}
		}

		#endregion

		#region Unity Methods

		private void OnDisable()
		{
			DestroyImmediate(LineTexture);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("themesEnabled"));

			GUI.enabled = serializedObject.FindProperty("themesEnabled").boolValue;

			EditorGUILayout.Space();

			DrawThemeIds();

			DrawSpriteIds(); // Новый метод для отображения spriteIds

			DrawThemes();

			EditorGUILayout.Space();

			GUI.enabled = true;

			serializedObject.ApplyModifiedProperties();
		}
		#endregion

		#region Private Methods

		
		private void DrawThemeIds()
		{
			SerializedProperty itemIdsProp = serializedObject.FindProperty("itemIds");

			if (BeginBox(itemIdsProp))
			{
				for (int i = 0; i < itemIdsProp.arraySize; i++)
				{
					SerializedProperty itemIdProp = itemIdsProp.GetArrayElementAtIndex(i);

					EditorGUILayout.BeginHorizontal();

					GUILayout.Space(16f);

					itemIdProp.stringValue = EditorGUILayout.TextField(itemIdProp.stringValue);

					bool removed = GUILayout.Button("Remove", GUILayout.Width(100));

					EditorGUILayout.EndHorizontal();

					if (removed)
					{
						RemoveItemId(itemIdsProp, i);
						i--;
					}
				}
				
				DrawLine();

				if (GUILayout.Button("Add Item Id"))
				{
					AddItemId(itemIdsProp);
				}

				GUILayout.Space(2f);
			}

			EndBox();
		}

		// Новый метод для отображения списка spriteIds в редакторе
		private void DrawSpriteIds()
		{
			SerializedProperty spriteIdsProp = serializedObject.FindProperty("spriteIds");

			if (spriteIdsProp == null)
			{
				Debug.LogWarning("spriteIds property not found. Ensure it is defined and serialized in the ThemeManager script.");
				return;
			}

			if (BeginBox(spriteIdsProp))
			{
				for (int i = 0; i < spriteIdsProp.arraySize; i++)
				{
					SerializedProperty spriteIdProp = spriteIdsProp.GetArrayElementAtIndex(i);

					EditorGUILayout.BeginHorizontal();

					GUILayout.Space(16f);

					spriteIdProp.stringValue = EditorGUILayout.TextField(spriteIdProp.stringValue);

					bool removed = GUILayout.Button("Remove", GUILayout.Width(100));

					EditorGUILayout.EndHorizontal();

					if (removed)
					{
						Debug.Log($"Removing spriteId at index {i}");
						RemoveSpriteId(spriteIdsProp, i);
						i--;
					}
				}

				DrawLine();

				if (GUILayout.Button("Add Sprite Id"))
				{
					Debug.Log("Adding new spriteId");
					AddSpriteId(spriteIdsProp);
				}

				GUILayout.Space(2f);
			}

			EndBox();
		}

		private void AddItemId(SerializedProperty itemIdsProp)
		{
			itemIdsProp.InsertArrayElementAtIndex(itemIdsProp.arraySize);

			SerializedProperty itemIdProp = itemIdsProp.GetArrayElementAtIndex(itemIdsProp.arraySize - 1);

			itemIdProp.stringValue = "";

			SerializedProperty themesProp = serializedObject.FindProperty("themes");

			for (int i = 0; i < themesProp.arraySize; i++)
			{
				SerializedProperty themeProp = themesProp.GetArrayElementAtIndex(i);
				SerializedProperty themeColorsProp = themeProp.FindPropertyRelative("themeColors");

				themeColorsProp.InsertArrayElementAtIndex(themeColorsProp.arraySize);
				themeColorsProp.GetArrayElementAtIndex(themeColorsProp.arraySize - 1).colorValue = Color.white;
			}
		}
		public Sprite defaultSprite; // Убедитесь, что этот спрайт задан в инспекторе или инициализирован

		private void AddSpriteId(SerializedProperty spriteIdsProp)
		{
			// Добавляем новый идентификатор спрайта
			spriteIdsProp.InsertArrayElementAtIndex(spriteIdsProp.arraySize);
			spriteIdsProp.GetArrayElementAtIndex(spriteIdsProp.arraySize - 1).stringValue = "";

			// Получаем ссылки на темы
			SerializedProperty themesProp = serializedObject.FindProperty("themes");
			for (int i = 0; i < themesProp.arraySize; i++)
			{
				SerializedProperty themeProp = themesProp.GetArrayElementAtIndex(i);
				SerializedProperty themeSpritesProp = themeProp.FindPropertyRelative("themeSprite"); // Измените на themeSprites

				// Добавляем новый элемент для спрайта для каждой темы
				themeSpritesProp.InsertArrayElementAtIndex(themeSpritesProp.arraySize);
				// Инициализируем новый спрайт на null (или можно установить на другой спрайт по умолчанию)
				themeSpritesProp.GetArrayElementAtIndex(themeSpritesProp.arraySize - 1).objectReferenceValue = defaultSprite;
			}
		}




		private void RemoveItemId(SerializedProperty itemIdsProp, int index)
		{
			itemIdsProp.DeleteArrayElementAtIndex(index);

			SerializedProperty themesProp = serializedObject.FindProperty("themes");

			for (int i = 0; i < themesProp.arraySize; i++)
			{
				SerializedProperty themeProp = themesProp.GetArrayElementAtIndex(i);
				SerializedProperty themeColorsProp = themeProp.FindPropertyRelative("themeColors");

				themeColorsProp.DeleteArrayElementAtIndex(index);
			}
		}

		// Новый метод для удаления spriteId
		private void RemoveSpriteId(SerializedProperty spriteIdsProp, int index)
		{
			// Удаляем идентификатор спрайта
			spriteIdsProp.DeleteArrayElementAtIndex(index);

			// Получаем ссылки на темы
			SerializedProperty themesProp = serializedObject.FindProperty("themes");

			// Удаляем соответствующий спрайт из каждой темы
			for (int i = 0; i < themesProp.arraySize; i++)
			{
				SerializedProperty themeProp = themesProp.GetArrayElementAtIndex(i);
				SerializedProperty themeSpritesProp = themeProp.FindPropertyRelative("themeSprite");

				// Удаляем спрайт в том же индексе
				if (index < themeSpritesProp.arraySize) // Проверяем, что индекс не выходит за пределы
				{
					themeSpritesProp.DeleteArrayElementAtIndex(index);
				}
			}
		}

		private void DrawThemes()
		{
			SerializedProperty themesProp = serializedObject.FindProperty("themes");

			if (BeginBox(themesProp))
			{
				for (int i = 0; i < themesProp.arraySize; i++)
				{
					SerializedProperty	themeProp	= themesProp.GetArrayElementAtIndex(i);
					SerializedProperty	nameProp	= themeProp.FindPropertyRelative("name");
					SerializedProperty	defaultProp	= themeProp.FindPropertyRelative("defaultTheme");

					string itemLabel = string.IsNullOrEmpty(nameProp.stringValue) ? "<name>" : nameProp.stringValue;

					if (defaultProp.boolValue)
					{
						itemLabel += " [DEFAULT]";
					}

					EditorGUILayout.BeginHorizontal();

					GUILayout.Space(16f);

					EditorGUILayout.PropertyField(themeProp, new GUIContent(itemLabel));

					bool removed = GUILayout.Button("Remove", GUILayout.Width(100));

					EditorGUILayout.EndHorizontal();

					if (themeProp.isExpanded)
					{
						EditorGUI.indentLevel++;

						EditorGUILayout.PropertyField(nameProp);

						bool val = EditorGUILayout.Toggle("Default Theme", defaultProp.boolValue);

						// Check if it was just toggled on
						if (val && val != defaultProp.boolValue)
						{
							// Turn off all other themes active bool
							for (int j = 0; j < themesProp.arraySize; j++)
							{
								if (i != j)
								{
									themesProp.GetArrayElementAtIndex(j).FindPropertyRelative("defaultTheme").boolValue = false;
								}
							}
						}

						defaultProp.boolValue = val;

						EditorGUILayout.Space();

						DrawThemeItems(themeProp);

						EditorGUI.indentLevel--;
					}

					if (removed)
					{
						themesProp.DeleteArrayElementAtIndex(i);
						i--;
					}
				}

				DrawLine();

				if (GUILayout.Button("Add Theme"))
				{
					AddTheme(themesProp);
				}

				GUILayout.Space(2f);
			}

			EndBox();
		}

		private void AddTheme(SerializedProperty themesProp)
		{
			themesProp.InsertArrayElementAtIndex(themesProp.arraySize);

			SerializedProperty themeProp = themesProp.GetArrayElementAtIndex(themesProp.arraySize - 1);

			themeProp.FindPropertyRelative("name").stringValue				= "";
			themeProp.FindPropertyRelative("defaultTheme").boolValue		= false;

			SerializedProperty themeColorsProp = themeProp.FindPropertyRelative("themeColors");

			for (int i = 0; i < themeColorsProp.arraySize; i++)
			{
				themeColorsProp.GetArrayElementAtIndex(i).colorValue = Color.white;
			}
		}

		private void DrawThemeItems(SerializedProperty themeProp)
		{
			SerializedProperty idsProp = serializedObject.FindProperty("itemIds");
			SerializedProperty imageProp = serializedObject.FindProperty("spriteIds");

			SerializedProperty colorsProp = themeProp.FindPropertyRelative("themeColors");
			SerializedProperty spritesProp = themeProp.FindPropertyRelative("themeSprite"); // Используем массив спрайтов

			// Заголовок для цветов
			EditorGUILayout.LabelField("Theme Colors", EditorStyles.boldLabel);

			for (int i = 0; i < idsProp.arraySize; i++)
			{
				SerializedProperty itemIdProp = idsProp.GetArrayElementAtIndex(i);

				// Убедитесь, что цвет для ID существует
				if (colorsProp.arraySize == i)
				{
					colorsProp.InsertArrayElementAtIndex(i);
					colorsProp.GetArrayElementAtIndex(i).colorValue = Color.white;
				}

				SerializedProperty colorProp = colorsProp.GetArrayElementAtIndex(i);
				string id = itemIdProp.stringValue;

				EditorGUILayout.PropertyField(colorProp, new GUIContent(id));
			}

			// Заголовок для спрайтов
			EditorGUILayout.Space(); // Добавляем отступ перед заголовком
			EditorGUILayout.LabelField("Theme Sprites", EditorStyles.boldLabel);

			for (int i = 0; i < imageProp.arraySize; i++)
			{
				SerializedProperty itemIdProp = imageProp.GetArrayElementAtIndex(i);

				// Убедитесь, что спрайт для ID существует
				if (spritesProp.arraySize == i)
				{
					spritesProp.InsertArrayElementAtIndex(i);
					spritesProp.GetArrayElementAtIndex(i).objectReferenceValue = defaultSprite; // Инициализация спрайта
				}

				SerializedProperty spriteProp = spritesProp.GetArrayElementAtIndex(i);
				string id = itemIdProp.stringValue;

				EditorGUILayout.PropertyField(spriteProp, new GUIContent(id)); // Отображение спрайта
			}
		}

		/// <summary>
		/// Begins a new foldout box, must call EndBox
		/// </summary>
		private bool BeginBox(SerializedProperty prop)
		{
			GUIStyle style		= new GUIStyle("HelpBox");
			style.padding.left	= 0;
			style.padding.right	= 0;
			style.margin.left = 0;

			GUILayout.BeginVertical(style);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(16f);

			prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);

			EditorGUILayout.EndHorizontal();

			if (prop.isExpanded)
			{
				DrawLine();
			}

			return prop.isExpanded;
		}

		/// <summary>
		/// Ends the box.
		/// </summary>
		private void EndBox()
		{
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws a simple 1 pixel height line
		/// </summary>
		private void DrawLine()
		{
			GUIStyle lineStyle			= new GUIStyle();
			lineStyle.normal.background	= LineTexture;

			GUILayout.BeginVertical(lineStyle);
			GUILayout.Space(1);
			GUILayout.EndVertical();
		}

		#endregion
	}
}
