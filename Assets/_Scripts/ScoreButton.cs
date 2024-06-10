using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreButton : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text npcText;

    Button button;

    void Awake() 
    {
        button = GetComponent<Button>();    
    }

    public void SetScoreText(string text)
    {
        scoreText.text = text;
    }

    public string GetScoreText()
    {
        return scoreText.text;
    }

    public void EnableNPCTag(bool enable)
    {
        npcText.gameObject.SetActive(enable);
    }

    public void AddListener(Action callback)
    {
        button.onClick.AddListener(()=>
        { 
            callback?.Invoke();
        });
    }
}
