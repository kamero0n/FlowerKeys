using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private WordBank wordBank;
    [SerializeField] private GameObject endScreenUI;
    [SerializeField] private TextMeshProUGUI _endScoreText;

    [SerializeField] private TextMeshProUGUI _flowersReleasedScore;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxPendingTiles = 2;
    [SerializeField] private int maxDeaths = 3;

    private int _flowersReleased = 0;
    private int _deaths = 0;
    private bool _gameOver = false;


    private void Start()
    {
        StartCoroutine(SpawnRoutine());
        endScreenUI.SetActive(false);
    }

    public void OnFlowerCollected()
    {
        _flowersReleased++;
        _flowersReleasedScore.text = $"x{_flowersReleased}";
    }

    public void OnFlowerDied()
    {
        if (_gameOver) return;

        _deaths++;
        if(_deaths >= maxDeaths)
        {
            _gameOver = true;
            _endScoreText.text = $"total: {_flowersReleased}";
            endScreenUI.SetActive(true);
            
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if(gridManager.CountPendingTiles() <  maxPendingTiles )
            {
                Tile tile = gridManager.GetRandomInactiveTile();
                if(tile != null)
                {
                    tile.SetPending(wordBank.GetWord());
                }
            }
        }
    }
}
