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
    
    //private TileBackground[,] allTiles;
    public GameObject[,] allGems;
    public GemManager selectedGem;




    void Start()
    {
        //allTiles = new TileBackground[width, height];
        matchHandler = FindObjectOfType<MatchHandler>();
        allGems = new GameObject[width, height];
        FillBoard();
    }


    void FillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Acomodando os espaços
                Vector2 tmpPstn = new Vector2(i, j + slideOffset);

                GameObject bgTile = Instantiate(tilePrefab, tmpPstn, Quaternion.identity, this.gameObject.transform);
                bgTile.name = "(" + i + "," + j + ")";
                //Instanciando as jóias
                int gemsAvailable = Random.Range(0, gems.Length);

                //check por segurança, depois ver o quão pesado tá esse loop
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
        if(column > 1 && row > 1)
        {
            if(allGems[column -1, row].tag == gemPiece.tag && 
                allGems[column - 2, row].tag == gemPiece.tag)
            {
                return true;
            }
            if (allGems[column, row - 1].tag == gemPiece.tag &&
                allGems[column , row - 2].tag == gemPiece.tag)
            {
                return true;
            }
        }else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allGems[column, row - 1].tag == gemPiece.tag && allGems[column, row - 2].tag == gemPiece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allGems[column - 1, row].tag == gemPiece.tag && allGems[column - 2, row].tag == gemPiece.tag)
                {
                    return true;
                }
            }
        }
        
        return false;
    }


    private bool BetterDetectMatchesBoard(int column, int row, GameObject gemPiece)
    {
        if (column > 1)          //!!! split these into two so it checks them correctly (row vs column)

        {

            //if the pieces to my left (already generated) are both of the same type as me then ...

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
            foreach(GameObject currentGem in matchHandler.currentMatches)
            {
                GemManager _gem = currentGem.GetComponent<GemManager>();
                if(_gem.row == firstGem.row)
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
        if(matchHandler.currentMatches.Count == 4 || matchHandler.currentMatches.Count == 7)
        {

            matchHandler.CheckBombs();
        }
        if(matchHandler.currentMatches.Count == 5 || matchHandler.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if (selectedGem != null)
                {
                    if (selectedGem.isMatched)
                    {
                        if (!selectedGem.isColorBomb)
                        {
                            selectedGem.isMatched = false;
                            selectedGem.ColorBombSpawner();
                        }
                        else
                        {
                            if(selectedGem.sideGem != null)
                            {
                                GemManager _otherGem = selectedGem.sideGem.GetComponent<GemManager>();
                                if (_otherGem.isMatched)
                                {
                                    if (!_otherGem.isColorBomb)
                                    {
                                        _otherGem.isMatched = false;
                                        _otherGem.ColorBombSpawner();
                                    }
                                }
                            }
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
        if(allGems[column, row].GetComponent<GemManager>().isMatched)
        {
            if(matchHandler.currentMatches.Count >= 4 )
            {
                BombSpawnerCheck();
            }

            

            Instantiate(destroyFX, allGems[column, row].transform.position, Quaternion.identity);
            Destroy(allGems[column, row]);
            allGems[column, row] = null;
        }
    }
    public void DestroyActualMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allGems[i, j] != null)
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
                if(allGems[i,j] == null)
                {
                    Vector2 tmpPstn = new Vector2(i, j + slideOffset);
                    int newGem = Random.Range(0, gems.Length);
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

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allGems[i, j] != null)
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



    

    #endregion

    #region Corountines
    ///
    /// ////////////////////////
   /////
    private IEnumerator FallRowGems()
    {
        int nullCounter = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allGems[i, j] == null)
                {
                    nullCounter++;
                }else if(nullCounter > 0)
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
            yield return new WaitForSeconds(refillTime);
            DestroyActualMatches();
        }
        matchHandler.currentMatches.Clear();
        selectedGem = null;
        yield return new WaitForSeconds(refillTime);
        currentState = GameStates.move;
    }

    #endregion

}
