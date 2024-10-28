using System.Collections.Generic;
using dotmob;
using Sudoku.Framework.Scripts.Popup;
using UnityEngine;
using UnityEngine.Localization;

namespace Sudoku.Framework.Scripts.Currency
{
	public class CurrencyManager : SingletonComponent<CurrencyManager>, ISaveable
	{
		#region Classes

		[System.Serializable]
		private class CurrencyInfo
		{
			public string	id						= "";    // Уникальный идентификатор валюты
			public int		startingAmount			= 0;    // Начальное количество валюты
			public bool		showNotEnoughPopup		= false; // Показывать ли всплывающее окно, если недостаточно валюты
			public LocalizedString popupTitleText			= null;    // Заголовок всплывающего окна
			public LocalizedString	popupMessageText		= null;    // Текст сообщения во всплывающем окне
			public bool		popupHasStoreButton		= false; // Есть ли кнопка перехода в магазин в окне
			public bool		popupHasRewardAdButton	= false; // Есть ли кнопка просмотра рекламы для получения вознаграждения
			public LocalizedString	rewardButtonText		= null;    // Текст кнопки для рекламы с наградой
			public int		rewardAmount			= 0;    // Количество вознаграждения за просмотр рекламы
		}

		#endregion

		#region Inspector Variables

		[SerializeField] private string				notEnoughPopupId	= "";   // ID всплывающего окна при недостатке валюты
		[SerializeField] private List<CurrencyInfo>	currencyInfos		= null; // Список информации о валютах

		#endregion

		#region Member Variables

		private Dictionary<string, int> currencyAmounts;  // Словарь для хранения количества каждой валюты

		#endregion

		#region Properties

		public string SaveId { get { return "currency_manager"; } }  // Уникальный идентификатор для сохранения данных валют

		// Событие, которое срабатывает при изменении количества валюты
		public System.Action<string> OnCurrencyChanged { get; set; }

		#endregion

		#region Unity Methods

		// Вызывается при загрузке объекта, инициализация начальных данных
		protected override void Awake()
		{
			base.Awake();

			// Инициализация словаря для хранения валют
			currencyAmounts = new Dictionary<string, int>();
			
			// Регистрация менеджера в системе сохранений
			SaveManager.Instance.Register(this);

			// Попытка загрузить данные. Если данные не загружены, устанавливаются стартовые значения
			if (!LoadSave())
			{
				SetStartingValues();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Получает количество валюты у игрока по ID валюты
		/// </summary>
		public int GetAmount(string currencyId)
		{
			// Проверяем, существует ли валюта
			if (!CheckCurrencyExists(currencyId))
			{
				return 0;
			}

			// Возвращаем текущее количество валюты
			return currencyAmounts[currencyId];
		}

		/// <summary>
		/// Пытается потратить валюту
		/// </summary>
		public bool TrySpend(string currencyId, int amount)
		{
			// Проверяем, существует ли валюта
			if (!CheckCurrencyExists(currencyId))
			{
				return false;
			}

			// Проверяем, достаточно ли у игрока валюты для траты
			if (currencyAmounts[currencyId] >= amount)
			{
				ChangeCurrency(currencyId, -amount);
				return true;
			}

			// Если недостаточно валюты, проверяем необходимость показать всплывающее окно
			CurrencyInfo currencyInfo = GetCurrencyInfo(currencyId);
			if (currencyInfo.showNotEnoughPopup && PopupManager.Exists())
			{
				// Создаем данные для показа всплывающего окна
				object[] popupData =
				{
					currencyInfo.id,
					currencyInfo.popupTitleText,
					currencyInfo.popupMessageText,
					currencyInfo.popupHasStoreButton,
					currencyInfo.popupHasRewardAdButton,
					currencyInfo.rewardButtonText,
					currencyInfo.rewardAmount
				};

				// Показываем всплывающее окно
				PopupManager.Instance.Show(notEnoughPopupId, popupData);
			}

			return false;
		}

		/// <summary>
		/// Добавляет игроку определенное количество валюты
		/// </summary>
		public void Give(string currencyId, int amount)
		{
			// Проверяем, существует ли валюта
			if (!CheckCurrencyExists(currencyId))
			{
				return;
			}

			// Изменяем количество валюты
			ChangeCurrency(currencyId, amount);
		}

		/// <summary>
		/// Добавляет валюту на основе строки данных, формат: "id;количество"
		/// </summary>
		public void Give(string data)
		{
			string[] stringObjs = data.Trim().Split(';');

			// Проверка на корректный формат строки
			if (stringObjs.Length != 2)
			{
				Debug.LogErrorFormat("[CurrencyManager] Give(string data) : Неверный формат данных: \"{0}\", ожидается формат \"id;количество\"", data);
				return;
			}

			string currencyId	= stringObjs[0];
			string amountStr	= stringObjs[1];

			int amount;

			// Проверяем, является ли количество целым числом
			if (!int.TryParse(amountStr, out amount))
			{
				Debug.LogErrorFormat("[CurrencyManager] Give(string data) : Количество должно быть целым числом, получены данные: \"{0}\"", data);
				return;
			}

			// Проверяем, существует ли валюта
			if (!CheckCurrencyExists(currencyId))
			{
				return;
			}

			// Изменяем количество валюты
			ChangeCurrency(currencyId, amount);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Изменяет количество валюты
		/// </summary>
		private void ChangeCurrency(string currencyId, int amount)
		{
			currencyAmounts[currencyId] += amount;

			// Вызываем событие изменения валюты
			if (OnCurrencyChanged != null)
			{
				OnCurrencyChanged(currencyId);
			}
		}

		/// <summary>
		/// Устанавливает стартовые значения для всех валют
		/// </summary>
		private void SetStartingValues()
		{
			for (int i = 0; i < currencyInfos.Count; i++)
			{
				CurrencyInfo currencyInfo = currencyInfos[i];

				// Устанавливаем стартовое количество валюты
				currencyAmounts[currencyInfo.id] = currencyInfo.startingAmount;
			}
		}

		/// <summary>
		/// Получает CurrencyInfo для указанного ID валюты
		/// </summary>
		private CurrencyInfo GetCurrencyInfo(string currencyId)
		{
			for (int i = 0; i < currencyInfos.Count; i++)
			{
				CurrencyInfo currencyInfo = currencyInfos[i];

				// Возвращаем информацию о валюте, если нашли совпадение по ID
				if (currencyId == currencyInfo.id)
				{
					return currencyInfo;
				}
			}

			// Возвращаем null, если валюта не найдена
			return null;
		}

		/// <summary>
		/// Проверяет, существует ли валюта
		/// </summary>
		private bool CheckCurrencyExists(string currencyId)
		{
			CurrencyInfo currencyInfo = GetCurrencyInfo(currencyId);

			// Если валюта не найдена или её количество не инициализировано, выводим сообщение об ошибке
			if (currencyInfo == null || !currencyAmounts.ContainsKey(currencyId))
			{
				Debug.LogErrorFormat("[CurrencyManager] TrySpend : Введённый currencyId \"{0}\" не существует", currencyId);
				return false;
			}

			return true;
		}

		#endregion

		#region Save Methods

		/// <summary>
		/// Сохраняет текущее состояние валюты
		/// </summary>
		public Dictionary<string, object> Save()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();

			// Сохраняем текущее количество валюты
			saveData["amounts"] = currencyAmounts;

			return saveData;
		}

		/// <summary>
		/// Загружает сохранённое состояние валюты
		/// </summary>
		public bool LoadSave()
		{
			JSONNode json = SaveManager.Instance.LoadSave(this);

			if (json == null)
			{
				return false;
			}

			// Загружаем сохранённые данные по валюте
			foreach (KeyValuePair<string, JSONNode> pair in json["amounts"])
			{
				// Убедимся, что валюта существует
				if (GetCurrencyInfo(pair.Key) != null)
				{
					currencyAmounts[pair.Key] = pair.Value.AsInt;
				}
			}

			return true;
		}

		#endregion
	}
}
