using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    wait,
    move
}


public class Board : MonoBehaviour
{
    private MatchHandler matchHandler;
    public GameStates currentState = GameStates.move;


    public int width;
    public int height;
    public float slideOffset;
    public float fallTime = .3f;
    public float refillTime = .5f;
    [Space(20)]
    public GameObject tilePrefab;
    [Space(20)]
    public GameObject[] gems;
    public GameObject destroyFX;

    public GameObject[,] allGems;
    public GemManager selectedGem;
    public Animator animator;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private ProgressBarHandler progressBar;
    public int baseGemValue = 10;
    private int streakValue = 1;



    void Start()
    {
        
        soundManager = FindObjectOfType<SoundManager>();
        matchHandler = FindObjectOfType<MatchHandler>();
        scoreManager = FindObjectOfType<ScoreManager>();
        progressBar = FindObjectOfType<ProgressBarHandler>();
        allGems = new GameObject[width, height];
        FillBoard();
    }


    void FillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                

                Vector2 tmpPstn = new Vector2(i, j + slideOffset);

                GameObject bgTile = Instantiate(tilePrefab, tmpPstn, Quaternion.identity, this.gameObject.transform);
                bgTile.name = "(" + i + "," + j + ")";
                
                int gemsAvailable = Random.Range(0, gems.Length);

                
                int maxLoop = 0;
                while (MatchesAtBoard(i, j, gems[gemsAvailable]) && maxLoop < 100)
                {
                    
                    gemsAvailable = Random.Range(0, gems.Length);
                    maxLoop++;
                }
                maxLoop = 0;

                GameObject gem = Instantiate(gems[gemsAvailable], tmpPstn, Quaternion.identity, this.transform);
                gem.GetComponent<GemManager>().row = j;
                gem.GetComponent<GemManager>().column = i;
                gem.name = "(" + i + "," + j + ")";
                allGems[i, j] = gem;

            }
        }
    }
    private bool MatchesAtBoard(int column, int row, GameObject gemPiece)
    {
        if (column > 1 && row > 1)
        {
            if ((allGems[column - 1, row] != null && allGems[column - 2, row] != null))
            {

                if (allGems[column - 1, row].tag == gemPiece.tag &&
                    allGems[column - 2, row].tag == gemPiece.tag)
                {
                    return true;
                }
            }
            if (allGems[column, row - 1] != null && allGems[column, row - 2] != null)
            {
                if (allGems[column, row - 1].tag == gemPiece.tag &&
                    allGems[column, row - 2].tag == gemPiece.tag)
                {
                    return true;
                }
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allGems[column, row - 1] != null && allGems[column, row - 2] != null)
                {
                    if (allGems[column, row - 1].tag == gemPiece.tag && allGems[column, row - 2].tag == gemPiece.tag)
                    {
                        return true;
                    }
                }

            }
            if (column > 1)
            {
                if ((allGems[column - 1, row] != null && allGems[column - 2, row] != null))
                {
                    if (allGems[column - 1, row].tag == gemPiece.tag && allGems[column - 2, row].tag == gemPiece.tag)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }


    private bool BetterDetectMatchesBoard(int column, int row, GameObject gemPiece)
    {
        if (column > 1)         

        {

        

            if (allGems[column - 1, row].GetComponent<GemManager>().tag == gemPiece.GetComponent<GemManager>().tag &&
                allGems[column - 2, row].GetComponent<GemManager>().tag == gemPiece.GetComponent<GemManager>().tag)

            {

                return true;

            }

        }



        if (row > 1)

        {

            //if the pieces below me (already generated) are both of the same type as me then ...

            if (allGems[column, row - 1].GetComponent<GemManager>().tag == gemPiece.GetComponent<GemManager>().tag &&
                allGems[column, row - 2].GetComponent<GemManager>().tag == gemPiece.GetComponent<GemManager>().tag)

            {

                return true;

            }

        }

        return false;
    }


    private bool ColumnOrRow()
    {
        int numHorizontal = 0;
        int numVertical = 0;
        GemManager firstGem = matchHandler.currentMatches[0].GetComponent<GemManager>();
        if (firstGem != null)
        {
            foreach (GameObject currentGem in matchHandler.currentMatches)
            {
                GemManager _gem = currentGem.GetComponent<GemManager>();
                if (_gem.row == firstGem.row)
                {
                    numHorizontal++;
                }
                if (_gem.column == firstGem.column)
                {
                    numVertical++;
                }
            }
        }
        return (numVertical == 5 || numHorizontal == 5);
    }

    private void BombSpawnerCheck()
    {
        if (matchHandler.currentMatches.Count == 4 || matchHandler.currentMatches.Count == 7)
        {

            matchHandler.CheckBombs();
        }
        if (matchHandler.currentMatches.Count == 5 || matchHandler.currentMatches.Count == 8)
        {
          
            if(ColumnOrRow() && selectedGem != null && selectedGem.isMatched)
            {
                if (!selectedGem.isColorBomb)
                    {
                        selectedGem.isMatched = false;
                        selectedGem.ColorBombSpawner();
                }
                else
                {
                    if (selectedGem.sideGem != null)
                    {
                        GemManager _otherGem = selectedGem.sideGem.GetComponent<GemManager>();
                        if(_otherGem.isMatched && !_otherGem.isColorBomb)
                        {
                            _otherGem.isMatched = false;
                            _otherGem.ColorBombSpawner();
                        }
                    }
                }
            }



            else
            {
                //outra bomba?
            }
        }
    }

    public void DestroyMatchLocation(int column, int row)
    {
        if (allGems[column, row].GetComponent<GemManager>().isMatched)
        {
            if (matchHandler.currentMatches.Count >= 4)
            {
                BombSpawnerCheck();
            }


            if(soundManager != null)
            {
                soundManager.PlayRandomSound();
            }

            if( progressBar != null)
            {
                progressBar.AddProgress(baseGemValue * streakValue);
            }

            Destroy(allGems[column, row]);
            Instantiate(destroyFX, allGems[column, row].transform.position, Quaternion.identity);
            scoreManager.IncreaseScore(baseGemValue * streakValue);
            allGems[column, row] = null;
        }
    }
    public void DestroyActualMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    DestroyMatchLocation(i, j);
                }
            }
        }
        matchHandler.currentMatches.Clear();
        StartCoroutine(FallRowGems());
    }

    #region Helpers

    private void Refill()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    Vector2 tmpPstn = new Vector2(i, j + slideOffset);
                    int newGem = Random.Range(0, gems.Length);
                    int maxIterations = 0;

                    while(MatchesAtBoard(i, j, gems[newGem]) && maxIterations < 100)
                    {
                        maxIterations++;
                        int gemToUse = Random.Range(0, gems.Length);
                    }

                    GameObject gemPiece = Instantiate(gems[newGem], tmpPstn, Quaternion.identity);
                    allGems[i, j] = gemPiece;
                    gemPiece.GetComponent<GemManager>().row = j;
                    gemPiece.GetComponent<GemManager>().column = i;
                    gemPiece.transform.parent = this.transform;
                    gemPiece.name = "( " + i + ", " + j + " )";
                }
            }
        }
    }

    void ShuffleGems()
    {
        List<GameObject> shuffledBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    shuffledBoard.Add(allGems[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int tileToUse = Random.Range(0, shuffledBoard.Count);
               
                int maxLoop = 0;
                while (MatchesAtBoard(i, j, shuffledBoard[tileToUse]) && maxLoop < 100)
                {
                    tileToUse = Random.Range(0, shuffledBoard.Count);
                    maxLoop++;
                }
                GemManager gem = shuffledBoard[tileToUse].GetComponent<GemManager>();
                maxLoop = 0;
                gem.column = i;
                gem.row = j;
                allGems[i, j] = shuffledBoard[tileToUse];
                shuffledBoard.Remove(shuffledBoard[tileToUse]);
            }
        }
        if (isDeadLocked())
        {
            if (width <= 2 && height <= 2)
            {
                ShuffleGems();
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (allGems[i, j].GetComponent<GemManager>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    #region DeadLockDetection Helpers

    private void SwitchPieces(int column, int row, Vector2 dir)
    {
        GameObject holder = allGems[column + (int)dir.x, row + (int)dir.y];
        allGems[column + (int)dir.x, row + (int)dir.y] = allGems[column, row];
        allGems[column, row] = holder;
    }

    bool CheckForMatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (i < width - 2)
                    {

                        if (allGems[i + 1, j] != null && allGems[i + 2, j] != null)
                        {
                            if (allGems[i + 1, j].tag == allGems[i, j].tag &&
                                allGems[i + 2, j].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                    {

                        if (allGems[i, j + 1] != null && allGems[i, j + 2] != null)
                        {
                            if (allGems[i, j + 1].tag == allGems[i, j].tag &&
                                allGems[i, j + 2].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }

                    }

                }
            }
        }

        return false;
    }


    bool ChecknSwitch(int column, int row, Vector2 dir)
    {
        SwitchPieces(column, row, dir);
        if (CheckForMatchesOnBoard())
        {
            SwitchPieces(column, row, dir);
            return true;
        }
        SwitchPieces(column, row, dir);
        return false;
    }

    bool isDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (ChecknSwitch(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < height - 1)
                    {
                        if (ChecknSwitch(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    #endregion





    #endregion

    #region Corountines
    private IEnumerator FallRowGems()
    {
        int nullCounter = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[i, j].GetComponent<GemManager>().row -= nullCounter;
                    allGems[i, j] = null;
                }
            }
            nullCounter = 0;
        }
        yield return new WaitForSeconds(fallTime);
        StartCoroutine(FillBoardCounter());
    }

    private IEnumerator FillBoardCounter()
    {
        Refill();
        yield return new WaitForSeconds(refillTime);

        while (MatchesOnBoard())
        {
            streakValue++;
            yield return new WaitForSeconds(refillTime);
            DestroyActualMatches();
        }
        matchHandler.currentMatches.Clear();
        selectedGem = null;
        yield return new WaitForSeconds(refillTime);
        if (isDeadLocked())
        {
            Debug.Log("travou tudo aqui meu");


            ShuffleGems();
        }
        currentState = GameStates.move;
        streakValue = 1;
    }

    #endregion

}
