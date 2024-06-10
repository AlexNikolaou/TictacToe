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

    void Start()
    {
        InitDificultyDropDown();
        initButtons();

        GameManager.Instance.gameOverEvent += GameOver;
    }

    void initButtons()
    {
        startBtn.onClick.AddListener(StartButtonCallback);
    }

    void StartButtonCallback() 
    {
        GameManager.Instance.RestartGame();
        Restart();
    }

    void InitDificultyDropDown()
    {
        dificultySellectionDropDown.ClearOptions();
        dificultySellectionDropDown.AddOptions(GetDificultiesList());
        dificultySellectionDropDown.value = (int)GameManager.Instance.Dificulty;

        dificultySellectionDropDown.onValueChanged.AddListener(DificultyDropdownValueChanged);
    }

    void DificultyDropdownValueChanged(int value)
    {
        GameManager.Instance.Dificulty = (Mode)value;
    }

    List<TMP_Dropdown.OptionData> GetDificultiesList()
    {
        string[] dificulties = Enum.GetNames(typeof(Mode));

        List<TMP_Dropdown.OptionData> list= new List<TMP_Dropdown.OptionData>();

        for(int i=0; i<dificulties.Length; i++)
        {
            list.Add(new TMP_Dropdown.OptionData(dificulties[i]));
        }

        return list;
    }

    void OnDisable() 
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
