using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindEveryMatch()
    {
        StartCoroutine(FindEveryMatchCoroutine());
    }


    void AddListAndMatch(GameObject gem)
    {
        if (!currentMatches.Contains(gem))
        {
            currentMatches.Add(gem);
        }
        gem.GetComponent<GemManager>().isMatched = true;
    }

    private void GetNearGem(GameObject gem1, GameObject gem2, GameObject gem3)
    {

        AddListAndMatch(gem1);
        AddListAndMatch(gem2);
        AddListAndMatch(gem3);

    }

    private IEnumerator FindEveryMatchCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentGem = board.allGems[i, j];

                if (currentGem != null)
                {
                    GemManager _currentGemPiece = currentGem.GetComponent<GemManager>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftGem = board.allGems[i - 1, j];
                        

                        GameObject rightGem = board.allGems[i + 1, j];
                        
                        if (leftGem != null && rightGem != null)
                        {
                            GemManager _leftGemPiece = leftGem.GetComponent<GemManager>();
                            GemManager _rightGemPiece = rightGem.GetComponent<GemManager>();
                            if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {

                                currentMatches.Union(IsRowBomb(_leftGemPiece, _currentGemPiece, _rightGemPiece));

                                currentMatches.Union(IsColumnBomb(_leftGemPiece, _currentGemPiece, _rightGemPiece));

                                GetNearGem(leftGem, currentGem, currentGem);



                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upGem = board.allGems[i, j + 1];


                        GameObject downGem = board.allGems[i, j - 1];



                        if (upGem != null && downGem != null)
                        {
                            GemManager _upGemPiece = upGem.GetComponent<GemManager>();
                            GemManager _downGemPiece = downGem.GetComponent<GemManager>();
                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {
                                currentMatches.Union(IsColumnBomb(_upGemPiece, _currentGemPiece, _downGemPiece));
                                currentMatches.Union(IsRowBomb(_upGemPiece, _currentGemPiece, _downGemPiece));

                                GetNearGem(upGem, currentGem, downGem);

                            }
                        }
                    }
                }
            }

        }
    }

    public void CheckBombs()
    {
        if (board.selectedGem != null)
        {
            if (board.selectedGem.isMatched)
            {
                board.selectedGem.isMatched = false;
                
                if ((board.selectedGem.swipeAngle > -45 && board.selectedGem.swipeAngle <= 45) ||
                    (board.selectedGem.swipeAngle > -135 || board.selectedGem.swipeAngle >= 135))
                {
                    board.selectedGem.RowBombSpawner();

                }
                else
                {
                    board.selectedGem.ColumnBombSpawner();

                }

            }
            else if (board.selectedGem.sideGem != null)
            {
                GemManager _sideGem = board.selectedGem.sideGem.GetComponent<GemManager>();
                if (_sideGem.isMatched)
                {
                    _sideGem.isMatched = false;

                    if ((board.selectedGem.swipeAngle > -45 && board.selectedGem.swipeAngle <= 45) ||
                    (board.selectedGem.swipeAngle > -135 || board.selectedGem.swipeAngle >= 135))
                    {
                        _sideGem.RowBombSpawner();

                    }
                    else
                    {
                        _sideGem.ColumnBombSpawner();

                    }

                }
            }
        }


    }

    public void DetectSameColorGem(string gemColor)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allGems[i, j] != null)
                {
                    if (board.allGems[i, j].tag == gemColor)
                    {
                        board.allGems[i, j].GetComponent<GemManager>().isMatched = true;
                    }
                }
            }
        }
    }



    #region Listas



    private List<GameObject> IsRowBomb(GemManager gem1, GemManager gem2, GemManager gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();
        if (gem1.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem1.row));
        }

        if (gem2.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem2.row));
        }

        if (gem3.isRowBomb)
        {
            currentMatches.Union(GetRowGem(gem3.row));
        }
        return currentGems;
    }


    private List<GameObject> IsColumnBomb(GemManager gem1, GemManager gem2, GemManager gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();
        if (gem1.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem1.column));
        }

        if (gem2.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem2.column));
        }

        if (gem3.isColumnBomb)
        {
            currentMatches.Union(GetColumnGem(gem3.column));
        }
        return currentGems;
    }



    List<GameObject> GetColumnGem(int column)
    {
        List<GameObject> gemsListed = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allGems[column, i] != null)
            {
                gemsListed.Add(board.allGems[column, i]);
                board.allGems[column, i].GetComponent<GemManager>().isMatched = true;
            }
        }

        return gemsListed;
    }

    List<GameObject> GetRowGem(int row)
    {
        List<GameObject> gemsListed = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allGems[i, row] != null)
            {
                gemsListed.Add(board.allGems[i, row]);
                board.allGems[i, row].GetComponent<GemManager>().isMatched = true;
            }
        }

        return gemsListed;
    }
    #endregion 
}


