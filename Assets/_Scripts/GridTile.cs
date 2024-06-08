using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class GridTile : MonoBehaviour
{
    [SerializeField] Vector2Int position;

    [Header("Sprites")]
    [SerializeField] Sprite xSprite;
    [SerializeField] Sprite oSprite;
    [SerializeField] Sprite emptySprite;

    TileValue _value = TileValue.none;
    SpriteRenderer spriteRenderer;

    //Private 
    void Awake() 
    {
        Init();
    }

    //Public
    public Vector2Int GetPosition()
    {
        return position;
    }

    public void Init()
    {
        _value = TileValue.none;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileValue(TileValue value)
    {
        _value = value;

        Sprite tileSprite = emptySprite;

        switch(value)
        {
            case TileValue.X:
                tileSprite =  xSprite;
            break;
            case TileValue.O:
                tileSprite =  oSprite;
            break;
        }

        spriteRenderer.sprite = tileSprite;
    }

    public TileValue GetValue()
    {
        return _value;
    }

    private void OnMouseDown() 
    {
        HandleClick();   
    }

    void HandleClick()
    {
        if(_value == TileValue.none && !GameManager.Instance.isNpcTurn)
        {
            SetTileValue(GameManager.Instance.GetPlayer());
            GameManager.Instance.TilePressed(position);
        }
    }
}
