using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [Header("Variaveis do Board")]
    public int column;
    public int row;
    public int prevColumn;
    public int prevRow;
    public int targetX;
    public int targetY;
    public bool isMatched;
    [Space(20)]
    [Tooltip("Tempo pra verificar se foi possível o match")]
    public float checkMatchTimer = .5f;
    public float canMoveTime = .5f;
    [Space(10)]

    [Header("Power Ups")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    public GameObject colorBomb;



    private MatchHandler matchHandler;
    private Board board;
    public GameObject sideGem;
    private Vector2 frstTchPstn;
    private Vector2 lstTchPstn;
    private Vector2 tmpPstn;
    [Space(10)]
    public float swipeAngle;
    public float swipeDiff = 1f;
    public float offsetMove;

    public Color color;




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


        //ChangeColor();
        GeneralMove();
    }


    private void GeneralMove()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tmpPstn = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tmpPstn, .6f);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            matchHandler.FindEveryMatch();
        }
        else
        {
            tmpPstn = new Vector2(targetX, transform.position.y);
            transform.position = tmpPstn;

        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tmpPstn = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tmpPstn, .6f);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            matchHandler.FindEveryMatch();

        }
        else
        {
            tmpPstn = new Vector2(transform.position.x, targetY);
            transform.position = tmpPstn;

        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameStates.move)
        {
            frstTchPstn = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    private void OnMouseUp()
    {
        if (board.currentState == GameStates.move)
        {
            lstTchPstn = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateSwipeAngle();
        }

    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //isColumnBomb = true;
            //this.gameObject.GetComponent<SpriteRenderer>().color = color;
            //GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity, this.gameObject.transform);
            //board.allGems[column, row].tag = "Color Bomb";
            //this.gameObject.GetComponent<SpriteRenderer>().material = colorMaterial;
        }
    }
    void CalculateSwipeAngle()
    {
        if (Mathf.Abs(lstTchPstn.y - frstTchPstn.y) > swipeDiff || Mathf.Abs(lstTchPstn.x - frstTchPstn.x) > swipeDiff)
        {
            //angulo da direção que foi arrastado
            swipeAngle = Mathf.Atan2(lstTchPstn.y - frstTchPstn.y, lstTchPstn.x - frstTchPstn.x) * 180 / Mathf.PI;
            MoveGems();
            board.selectedGem = this;

            //if (sideGem != null)

            //{
            //    board.currentState = GameStates.wait;
            //}
            //board.selectedGem = this;
        }
        else
        {

            board.currentState = GameStates.move;
        }
        

        
    }


    void MoveGems2_0(Vector2 dir)
    {
        sideGem = board.allGems[column + (int)dir.x, row + (int)dir.y];
        prevRow = row;
        prevColumn = column;
        if(sideGem != null)
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


    void MoveGems()
    {

        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MoveGems2_0(Vector2.right);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MoveGems2_0(Vector2.left);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MoveGems2_0(Vector2.up);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MoveGems2_0(Vector2.down);
        }
        else
        {
            board.currentState = GameStates.move;
        }
        //StartCoroutine(CheckMovePossibilities());
    }


    void DetectMatch()
    {
        //Detecção Horizontal
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftGem1 = board.allGems[column - 1, row];
            GameObject rightGem1 = board.allGems[column + 1, row];
            if (leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<GemManager>().isMatched = true;
                    rightGem1.GetComponent<GemManager>().isMatched = true;
                    isMatched = true;
                }

            }

        }

        //Detecção Vertical
        if (row > 0 && row < board.height - 1)
        {
            GameObject upGem1 = board.allGems[column, row + 1];
            GameObject downGem1 = board.allGems[column, row - 1];
            if (upGem1 != null && downGem1 != null)
            {
                if (upGem1.tag == this.gameObject.tag && downGem1.gameObject.tag == this.gameObject.tag)
                {
                    upGem1.GetComponent<GemManager>().isMatched = true;
                    downGem1.GetComponent<GemManager>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    void ChangeColor()
    {
        if (isMatched)
        {
            SpriteRenderer gemSprite = GetComponent<SpriteRenderer>();
            gemSprite.color = new Color(1, 1, 1, .2f);
        }
    }



    public void RowBombSpawner()
    {
        isRowBomb = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = color;
        
        // this.gameObject.GetComponent<Animator>().enabled = true;
        //this.gameObject.GetComponent<Animator>().Play("blue_flash");
        //board.selectedGem.GetComponent<Animation>().Play("blue_flash");
        //MakeLineBomb();
    }

    public void ColumnBombSpawner()
    {
        isColumnBomb = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = color;
        
        //this.gameObject.GetComponent<Animator>().enabled = true;
        //this.gameObject.GetComponent<Animator>().Play("blue_column");
        //board.selectedGem.GetComponent<Animation>().Play("blue_flash");
        //MakeLineBomb();
    }

    public void ColorBombSpawner()
    {
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity, this.gameObject.transform);
    }

    public IEnumerator CheckMovePossibilities()
    {
        if (isColorBomb)
        {
            matchHandler.DetectSameColorGem(sideGem.tag);
            isMatched = true;

        }else if(sideGem != null)
        {
            if (sideGem.GetComponent<GemManager>().isColorBomb)
            {
                matchHandler.DetectSameColorGem(this.gameObject.tag);
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
            //sideGem = null;
        }

    }



}
