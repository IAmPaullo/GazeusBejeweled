using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchHandler : MonoBehaviour
{

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();


    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }


    private List<GameObject> IsRowBomb(GemManager gem1, GemManager gem2, GemManager gem3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (gem1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem1.row));
        }

        if (gem2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem2.row));
        }

        if (gem3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(GemManager gem1, GemManager gem2, GemManager gem3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (gem1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem1.column));
        }

        if (gem2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem2.column));
        }

        if (gem3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject _gem)
    {
        if (!currentMatches.Contains(_gem))
        {
            currentMatches.Add(_gem);
        }
        _gem.GetComponent<GemManager>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
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
                            GemManager _rightGem = rightGem.GetComponent<GemManager>();
                            GemManager _leftGem = leftGem.GetComponent<GemManager>();
                            if (leftGem.CompareTag(currentGem.tag) && rightGem.CompareTag(currentGem.tag))
                            {

                                currentMatches.Union(IsRowBomb(_leftGem, _currentGemPiece, _rightGem));

                                currentMatches.Union(IsColumnBomb(_leftGem, _currentGemPiece, _rightGem));


                                GetNearbyPieces(leftGem, currentGem, rightGem);

                            }
                        }

                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upGem = board.allGems[i, j + 1];

                        GameObject downGem = board.allGems[i, j - 1];


                        if (upGem != null && downGem != null)
                        {
                            GemManager _downGemPiece = downGem.GetComponent<GemManager>();
                            GemManager _upGemPiece = upGem.GetComponent<GemManager>();
                            if (upGem.CompareTag(currentGem.tag) && downGem.CompareTag(currentGem.tag))
                            {

                                currentMatches.Union(IsColumnBomb(_upGemPiece, _currentGemPiece, _downGemPiece));

                                currentMatches.Union(IsRowBomb(_upGemPiece, _currentGemPiece, _downGemPiece));

                                GetNearbyPieces(upGem, currentGem, downGem);

                            }
                        }
                    }
                }
            }
        }

    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {

                if (board.allGems[i, j] != null)
                {

                    if (board.allGems[i, j].tag == color)
                    {

                        board.allGems[i, j].GetComponent<GemManager>().isMatched = true;
                    }
                }
            }
        }
    }

  

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allGems[column, i] != null)
            {
                GemManager _gem = board.allGems[column, i].GetComponent<GemManager>();
                if (_gem.isRowBomb)
                {
                    gems.Union(GetRowPieces(i)).ToList();
                }

                gems.Add(board.allGems[column, i]);
                _gem.isMatched = true;
            }
        }
        return gems;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allGems[i, row] != null)
            {
                GemManager _gem = board.allGems[i, row].GetComponent<GemManager>();
                if (_gem.isColumnBomb)
                {
                    gems.Union(GetColumnPieces(i)).ToList();
                }
                gems.Add(board.allGems[i, row]);
                _gem.isMatched = true;
            }
        }
        return gems;
    }

    public void CheckBombs()
    {

        if (board.selectedGem != null)
        {

            if (board.selectedGem.isMatched)
            {

                board.selectedGem.isMatched = false;

                if ((board.selectedGem.swipeAngle > -45 && board.selectedGem.swipeAngle <= 45)
                   || (board.selectedGem.swipeAngle < -135 || board.selectedGem.swipeAngle >= 135))
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
                GemManager sideGem = board.selectedGem.sideGem.GetComponent<GemManager>();

                if (sideGem.isMatched)
                {

                    sideGem.isMatched = false;

                    if ((board.selectedGem.swipeAngle > -45 && board.selectedGem.swipeAngle <= 45)
                   || (board.selectedGem.swipeAngle < -135 || board.selectedGem.swipeAngle >= 135))
                    {
                        sideGem.RowBombSpawner();
                    }
                    else
                    {
                        sideGem.ColumnBombSpawner();
                    }
                }
            }

        }
    }

}