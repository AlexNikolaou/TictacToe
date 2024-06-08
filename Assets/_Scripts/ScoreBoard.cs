using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] ScoreButton xPlayerBtn;
    [SerializeField] ScoreButton oPlayerBtn;

    private void Start() 
    {   
        xPlayerBtn.AddListener(()=>ButtonPressed(TileValue.X));
        oPlayerBtn.AddListener(()=>ButtonPressed(TileValue.O));
        UpdateScoreButtonsNPCTags();
        ResetScores();

        GameManager.Instance.gameOverEvent += AddWinPoint;
    }

    public void UpdateScoreButtonsNPCTags()
    {
        if(GameManager.Instance.aiPlayer == TileValue.X)
        {
            xPlayerBtn.EnableNPCTag(true);
            oPlayerBtn.EnableNPCTag(false);
        }
        else
        {
            xPlayerBtn.EnableNPCTag(false);
            oPlayerBtn.EnableNPCTag(true);
        }
    } 

    private void OnDisable() 
    {
        GameManager.Instance.gameOverEvent -= AddWinPoint;
    }

    void ButtonPressed(TileValue player)
    {
        if(GameManager.Instance.IsGameStarted)
            return;

        GameManager.Instance.isNpcTurn = player == TileValue.X ? false : true;
        GameManager.Instance.aiPlayer = player == TileValue.X ? TileValue.O : TileValue.X;
        UpdateScoreButtonsNPCTags();
        GameManager.Instance.RestartGame(false);
    }

    public void ResetScores()
    {
        SetScore(TileValue.X, 0);
        SetScore(TileValue.O, 0);
    }

    public void SetScore(TileValue player, int value)
    {
        switch(player)
        {
            case TileValue.X:
                xPlayerBtn.SetScoreText($"X : {value}");
            break;
            case TileValue.O:
                oPlayerBtn.SetScoreText($"O : {value}");
            break;
        }
    }

    public void AddScore(TileValue player, int value)
    {
        int currentScore = 0;

        switch(player)
        {
            case TileValue.X:
            {
                int.TryParse(xPlayerBtn.GetScoreText().Split(" ")[2], out currentScore);
                xPlayerBtn.SetScoreText($"X : {currentScore + value}");
            }
            break;
            case TileValue.O:
            {
                int.TryParse(oPlayerBtn.GetScoreText().Split(" ")[2], out currentScore);
                oPlayerBtn.SetScoreText($"O : {currentScore + value}");
            }
            break;
        }
    }

    void AddWinPoint(TileValue[] values)
    {
        if(values.Length == 0 || values.Length > 1)
            return;

        AddScore(values[0], 1);

        UpdateScoreButtonsNPCTags();
    }
}
