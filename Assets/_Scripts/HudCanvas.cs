using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudCanvas : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dificultySellectionDropDown;
    [SerializeField] Button startBtn;
    [SerializeField] GameOverView gameOverView;
    [SerializeField] ScoreBoard scoreBoard;

    string[] dificulties; 

    void Start()
    {
        InitDificultyDropDown();

        startBtn.onClick.AddListener(StartButtonCallback);

        GameManager.Instance.gameOverEvent += GameOver;
    }

    void InitDificultyDropDown()
    {
        dificulties = new string[]{"Easy", "Medium", "Impossible"};

        dificultySellectionDropDown.ClearOptions();
        dificultySellectionDropDown.AddOptions(GetDificultiesList());
        dificultySellectionDropDown.value = (int)GameManager.Instance.dificulty;

        dificultySellectionDropDown.onValueChanged.AddListener(DificultyDropdownValueChanged);
    }

    void DificultyDropdownValueChanged(int value)
    {
        GameManager.Instance.dificulty = (Dificulty)value;
    }

    void StartButtonCallback() 
    {
        GameManager.Instance.RestartGame();
        Restart();
    }

    List<TMP_Dropdown.OptionData> GetDificultiesList()
    {
        List<TMP_Dropdown.OptionData> list= new List<TMP_Dropdown.OptionData>();

        for(int i=0; i<dificulties.Length; i++)
        {
            list.Add(new TMP_Dropdown.OptionData(dificulties[i]));
        }

        return list;
    }

    private void OnDisable() 
    {
        GameManager.Instance.gameOverEvent -= GameOver;
    }

    void GameOver(TileValue[] playerTypes)
    {
        gameOverView.gameObject.SetActive(true);
        gameOverView.Set(playerTypes);
        gameOverView.Animate(true);
    }

    void Restart()
    {
        scoreBoard.UpdateScoreButtonsNPCTags();

        if(gameOverView.gameObject.activeInHierarchy)
        {
            gameOverView.Animate(false, ()=>{
                gameOverView.Reset();
            });
        }
    }
}
