using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [Header("Variaveis do Board")]
    public int column;
    public int row;
    private int prevColumn;
    private int prevRow;
    private int targetX;
    private int targetY;
    public bool isMatched;
    [Space(20)]
    [Tooltip("Tempo pra verificar se foi possível o match")]
    [SerializeField] float checkMatchTimer = .4f;
    [SerializeField] float canMoveTime = .5f;
    [Space(10)]

    [Header("Power Ups")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    [SerializeField] Animator anim;



    private MatchHandler matchHandler;
    private Board board;
    public GameObject sideGem;
    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPos;
    private Vector2 tempPos;
    [Space(10)]
    public float swipeAngle;
    [SerializeField] float swipeDiff = 1f;
    [SerializeField] float offsetMove;
    [SerializeField] float smoothLerp = .6f;




    void Start()
    {

        matchHandler = FindObjectOfType<MatchHandler>();
        board = FindObjectOfType<Board>();
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;


    }


    void Update()
    {
        GeneralMove();
    }


    private void GeneralMove()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, smoothLerp);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            matchHandler.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;

        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, smoothLerp);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            matchHandler.FindAllMatches();

        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;

        }
    }


    private void OnMouseDown()
    {
        if (board.currentState == GameStates.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    private void OnMouseUp()
    {
        if (board.currentState == GameStates.move)
        {
            lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateSwipeAngle();
        }

    }

    void CalculateSwipeAngle()
    {
        if (Mathf.Abs(lastTouchPos.y - firstTouchPosition.y) > swipeDiff || Mathf.Abs(lastTouchPos.x - firstTouchPosition.x) > swipeDiff)
        {

            swipeAngle = Mathf.Atan2(lastTouchPos.y - firstTouchPosition.y, lastTouchPos.x - firstTouchPosition.x) * 180 / Mathf.PI;
            CalculateGemMove();
            board.selectedGem = this;

        }
        else
        {

            board.currentState = GameStates.move;
        }



    }


    void MoveGem(Vector2 dir)
    {
        sideGem = board.allGems[column + (int)dir.x, row + (int)dir.y];
        prevRow = row;
        prevColumn = column;
        if (sideGem != null)
        {
            sideGem.GetComponent<GemManager>().column += -1 * (int)dir.x;
            sideGem.GetComponent<GemManager>().row += -1 * (int)dir.y;
            column += (int)dir.x;
            row += (int)dir.y;

            StartCoroutine(CheckMovePossibilities());
        }
        else
        {
            board.currentState = GameStates.move;
        }


    }


    void CalculateGemMove()
    {

        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MoveGem(Vector2.right);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MoveGem(Vector2.left);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MoveGem(Vector2.up);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MoveGem(Vector2.down);
        }
        else
        {
            board.currentState = GameStates.move;
        }

    }








    public void RowBombSpawner()
    {
        isRowBomb = true;
        anim.SetBool("IsRowBomb", true);
    }

    public void ColumnBombSpawner()
    {
        isColumnBomb = true;
        anim.SetBool("IsColumnBomb", true);
    }

    public void ColorBombSpawner()
    {
        isColorBomb = true;
        anim.SetBool("IsColorBomb", true);
    }



    public IEnumerator CheckMovePossibilities()
    {
        if (isColorBomb)
        {
            matchHandler.MatchPiecesOfColor(sideGem.tag);
            isMatched = true;

        }
        else if (sideGem != null)
        {
            if (sideGem.GetComponent<GemManager>().isColorBomb)
            {
                matchHandler.MatchPiecesOfColor(this.gameObject.tag);
                sideGem.GetComponent<GemManager>().isMatched = true;
            }
        }
        yield return new WaitForSeconds(checkMatchTimer);
        if (sideGem != null)
        {
            if (!isMatched && !sideGem.GetComponent<GemManager>().isMatched)
            {
                sideGem.GetComponent<GemManager>().row = row;
                sideGem.GetComponent<GemManager>().column = column;
                row = prevRow;
                column = prevColumn;
                yield return new WaitForSeconds(canMoveTime);
                board.selectedGem = null;
                board.currentState = GameStates.move;
            }
            else
            {
                board.DestroyActualMatches();
            }
        }

    }



}
