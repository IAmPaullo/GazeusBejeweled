using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{

    public int column;
    public int row;
    [Space(10)]
    public int targetX;
    public int targetY;
    
    private Board board;
    private GameObject sideGem;
    private Vector2 frstTchPstn;
    private Vector2 lstTchPstn;
    private Vector2 tmpPstn;
    [Space(10)]
    public float swipeAngle;
    public float offsetMove;



    
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;

    }

    
    void Update()
    {
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
        //angulo da direção que foi arrastado
        swipeAngle = Mathf.Atan2(lstTchPstn.y - frstTchPstn.y, lstTchPstn.x - frstTchPstn.x) * 180 / Mathf.PI;
        MoveGems();
    }

    void MoveGems()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
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
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            //Arrastando pra cima
            sideGem = board.allGems[column , row + 1];
            sideGem.GetComponent<GemManager>().row -= 1;
            row += 1;
        }
        else if (swipeAngle > -45 && swipeAngle >= -135 && row > 0)
        {
            //Arrastando pra baixo
            sideGem = board.allGems[column, row - 1 ];
            sideGem.GetComponent<GemManager>().row += 1;
            row -= 1;
        }
        
    }

}
