using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchHandler : MonoBehaviour
{

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    //private List<GameObject> IsAdjacentBomb(GemManager gem1, GemManager gem2, GemManager gem3)
    //{
    //    List<GameObject> currentDots = new List<GameObject>();
    //    if (gem1.isAdjacentBomb)
    //    {
    //        currentMatches.Union(GetAdjacentPieces(gem1.column, gem1.row));
    //    }

    //    if (gem2.isAdjacentBomb)
    //    {
    //        currentMatches.Union(GetAdjacentPieces(gem2.column, dogem2t2.row));
    //    }

    //    if (gem3.isAdjacentBomb)
    //    {
    //        currentMatches.Union(GetAdjacentPieces(gem3.column, gem3.row));
    //    }
    //    return currentDots;
    //}

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
                            if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {

                                currentMatches.Union(IsRowBomb(_leftGem, _currentGemPiece, _rightGem));

                                currentMatches.Union(IsColumnBomb(_leftGem, _currentGemPiece, _rightGem));

                                //currentMatches.Union(IsAdjacentBomb(_leftGem, currentDotDot, rightDotDot));


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
                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {

                                currentMatches.Union(IsColumnBomb(_upGemPiece, _currentGemPiece, _downGemPiece));

                                currentMatches.Union(IsRowBomb(_upGemPiece, _currentGemPiece, _downGemPiece));

                                //currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));


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
                //Check if that piece exists
                if (board.allGems[i, j] != null)
                {
                    //Check the tag on that dot
                    if (board.allGems[i, j].tag == color)
                    {
                        //Set that dot to be matched
                        board.allGems[i, j].GetComponent<GemManager>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //Check if the piece is inside the board
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allGems[i, j] != null)
                    {
                        gems.Add(board.allGems[i, j]);
                        board.allGems[i, j].GetComponent<GemManager>().isMatched = true;
                    }
                }
            }
        }
        return gems;
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
        //Did the player move something?
        if (board.selectedGem != null)
        {
            //Is the piece they moved matched?
            if (board.selectedGem.isMatched)
            {
                //make it unmatched
                board.selectedGem.isMatched = false;
                //Decide what kind of bomb to make
                /*
                int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50){
                    //Make a row bomb
                    board.currentDot.MakeRowBomb();
                }else if(typeOfBomb >= 50){
                    //Make a column bomb
                    board.currentDot.MakeColumnBomb();
                }
                */
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
            //Is the other piece matched?
            else if (board.selectedGem.sideGem != null)
            {
                GemManager sideGem = board.selectedGem.sideGem.GetComponent<GemManager>();
                //Is the other Dot matched?
                if (sideGem.isMatched)
                {
                    //Make it unmatched
                    sideGem.isMatched = false;
                    /*
                    //Decide what kind of bomb to make
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        //Make a row bomb
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        //Make a column bomb
                        otherDot.MakeColumnBomb();
                    }
                    */
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