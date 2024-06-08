using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField] TMP_Text playerText;
    [SerializeField] TMP_Text wonDrawText;

    CanvasGroup canvasGroup;
    bool activated = false;

    public bool IsActivated
    {
        get{
            return activated;
        }
    }

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        wonDrawText.text = "";
        playerText.text = "";
    }

    public void Set(TileValue[] tileValues)
    {
        wonDrawText.text = "";
        playerText.text = "";

        wonDrawText.text = tileValues.Length > 1 ? "DRAW" : "WON";

        for(int i=0; i< tileValues.Length; i++)
        {
            playerText.text += i>0 ? $" | {tileValues[i]}" : tileValues[i]; 
        }
    }

    public void Animate(bool activate, Action onComplete = null)
    {
        activated = activate;

        if(canvasGroup)
            canvasGroup.DOFade(activate ? 0.9f : 0.0f, 0.3f);

        onComplete?.Invoke();
    }

    public void Reset()
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
