using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Mode
{
    Easy = 0,
    Medium,
    Impossible
}

public enum TileValue
{
    none = 0,
    X,
    O
}

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GridTile> gridTiles;

    public delegate void GameOverEvent(TileValue[] data);
    public GameOverEvent gameOverEvent;

    int usedGridTiles = 0;
    float delay = 1.0f;
    float startedWaiting = 0.0f;
    bool gameOver = false;
    bool _npcTurn = false;

    TileValue[,] gridArray;
    TileValue _activePlayer = TileValue.X;
    TileValue _aiPlayer = TileValue.O;
    HashSet<Vector2Int> playerHotShots;
    HashSet<Vector2Int> aiHotShots;
    Mode _dificulty = Mode.Easy;
    
    static GameManager _instance;

#region Getters/Setters

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Mode Dificulty
    {
        get
        {
            return _dificulty;
        }
        set 
        {
            _dificulty = value;
        }
    }

    public TileValue AIPlayer
    {
        get
        {
            return _aiPlayer;
        }
        set
        {
            _aiPlayer = value;
        }
    }

    public bool IsNpcTurn
    {
        get
        {
            return _npcTurn;
        }
        set
        {
            _npcTurn = value;
        }
    }

    public bool IsGameStarted
    {
        get
        {
            return usedGridTiles>0 ? true : false;
        }
    }

    public TileValue Player
    {
        get
        {
            return _activePlayer;
        }
    }

#endregion

    void Awake() 
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)    
        {
            Destroy(gameObject);
        }
    }

    void Start() 
    {
        InitGrid();
    }

    List<GridTile> GetAvailableTiles()
    {
        List<GridTile> availableTiles = gridTiles.Where(x => x.Value == TileValue.none).ToList();

        return availableTiles;
    }

#region AI Play Modes

    void NpcEasyPlay()//Random Play
    {
        List<GridTile> availableTiles = GetAvailableTiles();

        int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, availableTiles.Count-1));

        availableTiles[index].SetTileValue(_activePlayer);
        TilePressed(availableTiles[index].Position);
    }

    void NpcMediumPlay()//Medium Play
    {
        List<GridTile> availableTiles = GetAvailableTiles();

        //Get the middle tile if it is available
        GridTile middleTile = availableTiles.Find(x => x.Position == new Vector2Int(1,1));

        //First play after player move
        if(availableTiles.Count == 8)
        {
            //If middle tile is available exclude it from options
            if(middleTile)
            {
                availableTiles.Remove(middleTile);
            }

            int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, availableTiles.Count-1));

            availableTiles[index].SetTileValue(_activePlayer);
            TilePressed(availableTiles[index].Position);
        }
        else if (aiHotShots.Count>0) //Win
        {
            GridTile theTile = availableTiles.Find(x => x.Position == aiHotShots.ElementAt(0));
            theTile.SetTileValue(_activePlayer);
            TilePressed(aiHotShots.ElementAt(0));
        }
        else if (playerHotShots.Count>0) //Win
        {
            GridTile theTile = availableTiles.Find(x => x.Position == playerHotShots.ElementAt(0));
            theTile.SetTileValue(_activePlayer);
            TilePressed(playerHotShots.ElementAt(0));
        }
        else
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, availableTiles.Count-1));

            availableTiles[index].SetTileValue(_activePlayer);
            TilePressed(availableTiles[index].Position);
        }
    }

    void NpcHardPlay()//Hard Play
    {
        List<GridTile> availableTiles = GetAvailableTiles();

        //Get the middle tile if it is available
        GridTile middleTile = availableTiles.Find(x => x.Position == new Vector2Int(1,1));

        //First play
        if(availableTiles.Count == 9)
        {
            GridTile rightBottomTile = availableTiles.Find(x => x.Position == new Vector2Int(2,2));
                rightBottomTile.SetTileValue(_activePlayer);
                TilePressed(rightBottomTile.Position);            
        }
        else if(availableTiles.Count == 8)
        {
            //If middle tile is available exclude it from options
            if(middleTile)
            {
                middleTile.SetTileValue(_activePlayer);
                TilePressed(middleTile.Position);
            }
            else
            {
                GridTile rightBottomTile = availableTiles.Find(x => x.Position == new Vector2Int(2,2));
                rightBottomTile.SetTileValue(_activePlayer);
                TilePressed(rightBottomTile.Position);
            }
            
        }
        else if (aiHotShots.Count>0) //Win
        {
            GridTile theTile = availableTiles.Find(x => x.Position == aiHotShots.ElementAt(0));
            theTile.SetTileValue(_activePlayer);
            TilePressed(aiHotShots.ElementAt(0));
        }
        else if (playerHotShots.Count>0) //Block 
        {
            GridTile theTile = availableTiles.Find(x => x.Position == playerHotShots.ElementAt(0));
            theTile.SetTileValue(_activePlayer);
            TilePressed(playerHotShots.ElementAt(0));
        }
        else
        {
            GridTile theMiddleTile = gridTiles.Find(x => x.Position == new Vector2Int(1,1));

            if(theMiddleTile.Value == TileValue.O)//If center tile is AI
            {
                if(gridArray[1,0] == TileValue.none && gridArray[1,2] == TileValue.none)
                {
                    GridTile theTileToGet = gridTiles.Find(x => x.Position == new Vector2Int(1,0));
                    theTileToGet.SetTileValue(_activePlayer);
                    TilePressed(theTileToGet.Position);
                }
                else if(gridArray[0,1] == TileValue.none && gridArray[2,1] == TileValue.none)
                {
                    GridTile theTileToGet = gridTiles.Find(x => x.Position == new Vector2Int(0,1));
                    theTileToGet.SetTileValue(_activePlayer);
                    TilePressed(theTileToGet.Position);
                }
                else
                {
                    int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, availableTiles.Count-1));

                    availableTiles[index].SetTileValue(_activePlayer);
                    TilePressed(availableTiles[index].Position);
                }
            }
            else
            {
                int index = Mathf.RoundToInt(UnityEngine.Random.Range(0, availableTiles.Count-1));

                availableTiles[index].SetTileValue(_activePlayer);
                TilePressed(availableTiles[index].Position);
            }
        }
    }

#endregion

    private void Update() 
    {
        if(gameOver)
            return;

        if(_npcTurn && Time.time > startedWaiting + delay)
        {
            switch(_dificulty)
            {
                case Mode.Easy:
                    NpcEasyPlay();
                    break;
                case Mode.Medium:
                    NpcMediumPlay();
                    break;
                case Mode.Impossible:
                    NpcHardPlay();
                    break;
            }

            _npcTurn = false;
        }
    }

    void InitGrid()
    {
        gridArray = new TileValue[3,3];
        ResetGridValues();
        ResetGridTiles();
    }

    void ResetGridValues()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                gridArray[i,j] = TileValue.none;
            }
        }
    }
    
    void ResetGridTiles()
    {
        for (int i=0; i<gridTiles.Count; i++)
        {
            gridTiles[i].SetTileValue(TileValue.none);
        }
    }

    void ResetHotShots()
    {
        aiHotShots = new HashSet<Vector2Int>();
        playerHotShots = new HashSet<Vector2Int>();
    }

    IEnumerator CheckMove(Action<bool> onComplete)
    {
        bool next = false;
        bool won = false;

        ResetHotShots();

        CheckVertical((success, line, value)=>{
            if(success)
                won = true;
            next = true;
        });

        while(!next)
            yield return null;

        if(!won)
        {
            next = false;

            CheckHorizontal((success, line, value)=>{
                if(success)
                    won = true;
                next = true;
            });

            while(!next)
            yield return null;
        }

        if(!won)
        {
            CheckDiagonal((success, line, value)=>{
                if(success)
                    won = true;
            });
        }

        onComplete.Invoke(won);
    }

    void CheckDiagonal(Action<bool, int, TileValue> onComplete)
    {
        if(gridArray[0,0] == gridArray[1,1] && gridArray[1,1] == gridArray[2,2]  && gridArray[1,1] != TileValue.none)//Check full line
        {
            onComplete(true,0,gridArray[0,0]);
        }
        else if (gridArray[2,0] == gridArray[1,1] && gridArray[1,1] == gridArray[0,2] && gridArray[1,1] != TileValue.none)//Check full line
        {
            onComplete(true,1,gridArray[2,0]);
        }
        else if(gridArray[0,0] == gridArray[1,1] && gridArray[2,2] == TileValue.none && gridArray[0,0] != TileValue.none)
        {
            if(gridArray[0,0] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(2,2));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(2,2));
            }
        }
        else if(gridArray[0,0] == gridArray[2,2] && gridArray[1,1] == TileValue.none && gridArray[0,0] != TileValue.none)
        {
            if(gridArray[0,0] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(1,1));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(1,1));
            }
        }
        else if(gridArray[1,1] == gridArray[2,2] && gridArray[0,0] == TileValue.none && gridArray[1,1] != TileValue.none)
        {
            if(gridArray[1,1] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(0,0));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(0,0));
            }
        }
        else if(gridArray[0,2] == gridArray[1,1] && gridArray[2,0] == TileValue.none && gridArray[0,2] != TileValue.none)
        {
            if(gridArray[0,2] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(2,0));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(2,0));
            }
        }
        else if(gridArray[0,2] == gridArray[2,0] && gridArray[1,1] == TileValue.none && gridArray[0,2] != TileValue.none)
        {
            if(gridArray[0,2] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(1,1));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(1,1));
            }
        }
        else if(gridArray[2,0] == gridArray[1,1] && gridArray[0,2] == TileValue.none && gridArray[2,0] != TileValue.none)
        {
            if(gridArray[2,0] != _aiPlayer)//If not AI
            {
                playerHotShots.Add(new Vector2Int(0,2));
            }
            else
            {
                aiHotShots.Add(new Vector2Int(0,2));
            }
        }
        
        onComplete(false, -1, TileValue.none);
    }

    void CheckVertical(Action<bool, int, TileValue> onComplete)
    {
        for(int i=0; i<3; i++)
        {
            if(gridArray[i,0] == gridArray[i,1] && gridArray[i,1] == gridArray[i,2] && gridArray[i,0] != TileValue.none)//Check full line
            {
                onComplete(true,i+1,gridArray[i,0]);
                break;
            }
            else if(gridArray[i,0] == gridArray[i,1] && gridArray[i,2] == TileValue.none && gridArray[i,0] != TileValue.none)//Check for hotshots (1 to win)
            {
                if(gridArray[i,0] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(i,2));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(i,2));
                }
            }
            else if(gridArray[i,0] == gridArray[i,2] && gridArray[i,1] == TileValue.none && gridArray[i,0] != TileValue.none)
            {
                if(gridArray[i,0] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(i,1));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(i,1));
                }
            }
            else if(gridArray[i,1] == gridArray[i,2] && gridArray[i,0] == TileValue.none && gridArray[i,1] != TileValue.none)
            {
                if(gridArray[i,1] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(i,0));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(i,0));
                }
            }
        }

        onComplete(false, -1, TileValue.none);
    }

    void CheckHorizontal(Action<bool, int, TileValue> onComplete)
    {
        for(int j=0; j<3; j++)
        {
            if(gridArray[0,j] == gridArray[1,j] && gridArray[1,j] == gridArray[2,j] && gridArray[0,j] != TileValue.none)
            {
                onComplete(true,j+1,gridArray[0,j]);
                break;
            }
            else if(gridArray[0,j] == gridArray[1,j] && gridArray[2,j] == TileValue.none && gridArray[0,j] != TileValue.none)//Check for hotshots (1 to win)
            {
                if(gridArray[0,j] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(2,j));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(2,j));
                }
            }
            else if(gridArray[0,j] == gridArray[2,j] && gridArray[1,j] == TileValue.none && gridArray[0,j] != TileValue.none)
            {
                if(gridArray[0,j] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(1,j));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(1,j));
                }
            }
            else if(gridArray[1,j] == gridArray[2,j] && gridArray[0,j] == TileValue.none && gridArray[1,j] != TileValue.none)
            {
                if(gridArray[1,j] != _aiPlayer)//If not AI
                {
                    playerHotShots.Add(new Vector2Int(0,j));
                }
                else
                {
                    aiHotShots.Add(new Vector2Int(0,j));
                }
            }
        }
        onComplete(false, -1, TileValue.none);
    }

    void SetGridValue(Vector2Int position)
    {
        gridArray[position.x, position.y] = _activePlayer;

        StartCoroutine(CheckMove((won)=>
        {
            usedGridTiles++;

            if(usedGridTiles<gridTiles.Count && !won)
            {
                _activePlayer = _activePlayer == TileValue.X ? TileValue.O : TileValue.X;
            }
            else
            {
                GameOver(won);
            }
        }));
    }

    void GameOver(bool won)
    {
        gameOver = true;
        TileValue[] values;
        _aiPlayer = TileValue.O;
        _npcTurn = false;

        if(won)
        {
            values = new TileValue[] { _activePlayer };
        }
        else //Draw
        {
            values = new TileValue[] { TileValue.X, TileValue.O };
        }
        gameOverEvent?.Invoke(values);
    }

    public void TilePressed(Vector2Int position)
    {
        if(!_npcTurn)
            startedWaiting = Time.time;

        _npcTurn = !_npcTurn;

        SetGridValue(position);
    }

    public void RestartGame(bool resetNPC = true)
    {
        ResetHotShots();

        if(resetNPC)
        {
            _aiPlayer = TileValue.O;
            _npcTurn = false;
        }
        _activePlayer = TileValue.X;
        usedGridTiles = 0;
        InitGrid();
        gameOver = false;
    }
}
