using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    [Space(20)]
    public GameObject tilePrefab;
    [Space(20)]
    public GameObject[] gems;
    
    private TileBackground[,] allTiles;
    public GameObject[,] allGems;




    void Start()
    {
        //allTiles = new TileBackground[width, height];
        allGems = new GameObject[width, height];
        SetUp();
    }


    void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Acomodando os espaços
                Vector2 tmpPstn = new Vector2(i, j);
                GameObject bgTile = Instantiate(tilePrefab, tmpPstn, Quaternion.identity, this.gameObject.transform);
                bgTile.name = "(" + i + "," + j + ")";
                //Instanciando as jóias
                int gemsAvailable = Random.Range(0, gems.Length);
                GameObject gem = Instantiate(gems[gemsAvailable], tmpPstn, Quaternion.identity, this.transform);
                gem.name = "(" + i + "," + j + ")";
                allGems[i, j] = gem;

            }
        }
    }
}
