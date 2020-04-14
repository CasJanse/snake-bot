using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{

    private Grid<int> grid;
    public int width = 66;
    public int height = 48;

    private void Awake()
    {
        grid = new Grid<int>(width, height, 1f, new Vector3(-width / 2.0f, -height / 2.0f));
        grid.SetAllValues(-1);
        for (int j = 0; j < height; j++)
        {
            grid.SetValue(0, j, width * height - j);
            if (j % 2 == 0)
            {
                for (int i = 1; i < width; i++)
                {
                    if (j == 0)
                    {
                        grid.SetValue(i, j, i + j * (width - 1));
                    }
                    else
                    {
                        grid.SetValue(i, j, i + j * (width - 1));
                    }
                }
            }
            else
            {
                for (int i = width - 1; i >= 1; i--)
                {
                    grid.SetValue(i, j, width - i + j * (width - 1));
                }
            }
            
        }
    }

    private void Update()
    {

    }

    public Grid<int> GetGrid()
    {
        return grid;
    }

    //public Vector2 ChooseRandomTile(Vector2[] adjacentTiles)
    //{
    //    bool validTile = false;
    //    Vector2 chosenTile;

    //    while (!validTile)
    //    {
    //        chosenTile = adjacentTiles[(int)Random.Range(0, 3)];
    //        if (chosenTile != new Vector2(-1, -1))
    //        {
    //            validTile = true;
    //        }
    //    }
        
    //}


}
