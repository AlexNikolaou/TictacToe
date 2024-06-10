using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class GridTile : MonoBehaviour
{
    [SerializeField] Vector2Int _position;

    [Header("Sprites")]
    [SerializeField] Sprite xSprite;
    [SerializeField] Sprite oSprite;
    [SerializeField] Sprite emptySprite;

    TileValue _value = TileValue.none;
    SpriteRenderer spriteRenderer;

#region Getters/Setters

    public TileValue Value
    {
        get
        {
            return _value;
        }
    }

    public Vector2Int Position
    {
        get
        {
            return _position;
        }
    }

#endregion

    void Awake() 
    {
        Init();
    }

    void Init()
    {
        _value = TileValue.none;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown() 
    {
        HandleClick();   
    }

    void HandleClick()
    {
        if(_value == TileValue.none && !GameManager.Instance.IsNpcTurn)
        {
            SetTileValue(GameManager.Instance.Player);
            GameManager.Instance.TilePressed(_position);
        }
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
}
