using System.Collections;
using System.Collections.Generic;
using Sudoku.Framework.Scripts.Screen;
using Sudoku.Scripts.Game;
using Sudoku.Scripts.Theme;
using UnityEngine;

namespace dotmob.Sudoku
{
    public class StatsScreen : Screen
    {
        #region Inspector Variables

        [Space] [SerializeField] private StatsListItem statsListItemPrefab = null;
        [SerializeField] private Transform statsListContainer = null;

        #endregion

        #region Member Variables

        private ObjectPool statsListItemPool;

        #endregion

        [SerializeField] private GameObject newGame; // Верхняя панель
        [SerializeField] private GameObject pause; // Верхняя панель
        [SerializeField] private GameObject back; // Верхняя панель

        #region Public Methods

        public void Back()
        {
            newGame.gameObject.SetActive(true);
            pause.gameObject.SetActive(true);
            back.gameObject.SetActive(false);

            ScreenManager.Instance.Back();
        }

        public override void Initialize()
        {
            base.Initialize();
            statsListItemPool = new ObjectPool(statsListItemPrefab.gameObject, 4, statsListContainer);
        }

        public override void Show(bool back1, bool immediate)
        {         
            newGame.gameObject.SetActive(false);
            pause.gameObject.SetActive(false);
            back.gameObject.SetActive(true);

            base.Show(back1, immediate);

            statsListItemPool.ReturnAllObjectsToPool();

            for (int i = 0; i < GameManager.Instance.PuzzleGroupDatas.Count; i++)
            {
                // Получаем объект из пула
                var statsItem = statsListItemPool.GetObject<StatsListItem>();

                // Настраиваем элемент
                statsItem.Setup(GameManager.Instance.PuzzleGroupDatas[i]);

                // Регистрируем элемент в ThemeManager
                ThemeManager.Instance.Register(statsItem);
            }
        }

        #endregion
    }
}