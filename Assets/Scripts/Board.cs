using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private TileBackground[,] allTiles;
    



    void Start()
    {
        allTiles = new TileBackground[width, height];
        SetUp();
    }

    void Update()
    {
        
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tmpPstn = new Vector2(i, j);
                Instantiate(tilePrefab, tmpPstn, Quaternion.identity);
            }
        }
    }
}
