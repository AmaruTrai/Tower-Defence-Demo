using Game.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tools.Liquidated;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private TMP_Text livesText;
        [SerializeField]
        private TMP_Text goldText;
        [SerializeField]
        private int lives = 10;
        [SerializeField]
        private int score;
        [SerializeField]
        private int gold;

        [SerializeField]
        private ButtonPanel panel;

        [SerializeField]
        private GameObject loosePanel;
        [SerializeField]
        private GameObject winPanel;


        private TowerSpawnPoint spawnPoint;
        private List<SpawnPoint> spawns;
        private int ActiveEnemyCount;

        public void Awake()
        {
            SetText();
            panel.OnButtonClick += OnButtonClick;
        }

        [Inject]
        public void Constract(
            LiquidatedObjectPool pool,
            List<TowerSpawnPoint> points,
            List<SpawnPoint> spawnPoints
        ) {
            pool.OnLiquidatedObjectCreated += OnLiquidatedObjectCreated;
            foreach(var value in points)
            {
                value.OnPointClicked += OnSpawnPointClicked;
            }
            spawns = spawnPoints;
        }

        private void OnSpawnPointClicked(TowerSpawnPoint point, PointerEventData eventData)
        {
            var info = point.TowerInfo;
            spawnPoint = point;
            panel.Show(info, false);
            panel.transform.position = eventData.position;
        }

        private void OnButtonClick(TowerInfo info)
        {
            if (info != null && spawnPoint != null && gold >= info.Cost)
            {
                gold -= info.Cost;
                SetText();
                spawnPoint.SetTower(info);
            }
        }

        private void OnLiquidatedObjectCreated(LiquidatedObject obj)
        {
            if (obj is Enemy enemy)
            {
                enemy.OnDead += (_) => {
                    score++;
                    gold += 10;
                    ActiveEnemyCount--;
                    SetText();
                    ChechGameEnd();
                };

                enemy.OnEndPointReached += (_) => {
                    lives--;       
                    if (lives <= 0)
                    {
                        loosePanel.SetActive(true);
                        Debug.Log("Game End");
                        Time.timeScale = 0;
                    }
                    ActiveEnemyCount--;
                    SetText();
                    ChechGameEnd();
                };

                enemy.OnRestart += (_) => {
                    ActiveEnemyCount++;
                };
            }
        }

        private void SetText()
        {
            livesText.text = $"Lives: {lives}";
            scoreText.text = $"Score: {score}";
            goldText.text = $"Gold: {gold}";
        }

        private void ChechGameEnd()
        {
            if (ActiveEnemyCount == 0 && spawns.All(spawn => spawn.SpawnEnded))
            {
                Win();
            }
        }

        private void Win()
        {
            Debug.Log("Win");
            winPanel.SetActive(true);
        }

        public void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
