﻿using System.Collections;
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
    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPos;
    private Vector2 tempPos;
    [Space(10)]
    public float swipeAngle;
    public float swipeDiff = 1f;
    public float offsetMove;
    [SerializeField] float smoothLerp =.6f;
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


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ColorBombSpawner();
            //this.gameObject.GetComponent<SpriteRenderer>().color = color;
            //GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity, this.gameObject.transform);
            //board.allGems[column, row].tag = "Color Bomb";
            //this.gameObject.GetComponent<SpriteRenderer>().material = colorMaterial;
        }
    }
    void CalculateSwipeAngle()
    {
        if (Mathf.Abs(lastTouchPos.y - firstTouchPosition.y) > swipeDiff || Mathf.Abs(lastTouchPos.x - firstTouchPosition.x) > swipeDiff)
        {
            //angulo da direção que foi arrastado
            swipeAngle = Mathf.Atan2(lastTouchPos.y - firstTouchPosition.y, lastTouchPos.x - firstTouchPosition.x) * 180 / Mathf.PI;
            CalculateGemMove();
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


    void MoveGem(Vector2 dir)
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
        
        
    }

    public void ColumnBombSpawner()
    {
        isColumnBomb = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = color;
        Debug.Log("boi");
        
        
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
            matchHandler.MatchPiecesOfColor(sideGem.tag);
            isMatched = true;

        }else if(sideGem != null)
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
