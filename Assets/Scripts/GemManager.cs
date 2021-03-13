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
    public float checkMoveTime;

    [Space(10)]
    private Board board;
    private GameObject sideGem;
    private Vector2 frstTchPstn;
    private Vector2 lstTchPstn;
    private Vector2 tmpPstn;
    [Space(10)]
    public float swipeAngle;
    public float swipeDiff = 1f;
    public float offsetMove;



    
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        prevRow = row;
        prevColumn = column;

    }

    
    void Update()
    {
        DetectMatch();
        ChangeColor();
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tmpPstn = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tmpPstn, .4f);
        }
        else
        {
            tmpPstn = new Vector2(targetX, transform.position.y);
            transform.position = tmpPstn;
            board.allGems[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tmpPstn = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tmpPstn, .4f);
        }
        else
        {
            tmpPstn = new Vector2(transform.position.x, targetY);
            transform.position = tmpPstn;
            board.allGems[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        frstTchPstn = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    private void OnMouseUp()
    {
        lstTchPstn = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MathFunctions();
        
    }

    void MathFunctions()
    {
        if(Mathf.Abs(lstTchPstn.y - frstTchPstn.y) > swipeDiff || Mathf.Abs(lstTchPstn.x - frstTchPstn.x) > swipeDiff)
        {

        //angulo da direção que foi arrastado
        swipeAngle = Mathf.Atan2(lstTchPstn.y - frstTchPstn.y, lstTchPstn.x - frstTchPstn.x) * 180 / Mathf.PI;
        MoveGems();
        }
    }

    void MoveGems()
    {
        
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Arrastando pra direita
            sideGem = board.allGems[column + 1, row ];
            sideGem.GetComponent<GemManager>().column -= 1;
            column += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Arrastando pra esquerda
            sideGem = board.allGems[column - 1, row];
            sideGem.GetComponent<GemManager>().column += 1;
            column -= 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Arrastando pra cima
            sideGem = board.allGems[column , row + 1];
            sideGem.GetComponent<GemManager>().row -= 1;
            row += 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Arrastando pra baixo
            sideGem = board.allGems[column, row - 1 ];
            sideGem.GetComponent<GemManager>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMovePossibilities());
    }


    void DetectMatch()
    {
        //Detecção Horizontal
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftGem1 = board.allGems[column - 1, row];
            GameObject rightGem1 = board.allGems[column + 1, row];
            if(leftGem1 != null && leftGem1 != null)
            {
                if(leftGem1.tag == this.gameObject.tag && rightGem1.gameObject.tag == this.gameObject.tag)
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

    void ChangeColor ()
    {
        if (isMatched)
        {
            SpriteRenderer gemSprite = GetComponent<SpriteRenderer>();
            gemSprite.color = new Color(1, 1, 1, .2f);
        }
    }


    public IEnumerator CheckMovePossibilities()
    {
        yield return new WaitForSeconds(checkMoveTime);
        if(sideGem != null)
        {
            if(!isMatched && !sideGem.GetComponent<GemManager>().isMatched)
            {
                sideGem.GetComponent<GemManager>().row = row;
                sideGem.GetComponent<GemManager>().column = column;
                row = prevRow;
                column = prevColumn;
            }
            else
            {
                board.DestroyActualMatches();
            }
            sideGem = null;
        }
        
    }



}
