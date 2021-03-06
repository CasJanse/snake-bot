﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    private bool[,] partOfTreeArray;
    public const int sortingOrderDefault = 5000;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];
        partOfTreeArray = new bool[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                debugTextArray[x, y] = CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 5, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
        else
        {
            //Debug.LogWarning("Value could not be set at: [" + x + ", " + y + "]. Index does not exist in grid.");
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            //Debug.LogWarning("Could not retrieve value at: [" + x + ", " + y + "]. Index does not exist in grid.");
            return default(TGridObject);
        }
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public void SetAllValues(TGridObject value)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = value;
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }
        }
    }

    public Vector3 GetCenterPositionOfCell(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return GetWorldPosition(x, y) + (new Vector3(cellSize, cellSize) * 0.5f);
        }
        else
        {
            //Debug.LogWarning("Could not get center position at: [" + x + ", " + y + "]. Index does not exist in grid.");
            return Vector3.zero;
        }
    }

    public Vector2[] GetAdjacentTiles(int x, int y) {
        Vector2[] adjacentTiles = new Vector2[4];

        if (x + 1 < width)
        {
            adjacentTiles[0] = new Vector2(x + 1, y);
        }
        else {
            adjacentTiles[0] = new Vector2(-1, -1);
        }

        if (x - 1 >= 0)
        {
            adjacentTiles[1] = new Vector2(x - 1, y);
        }
        else
        {
            adjacentTiles[1] = new Vector2(-1, -1);
        }

        if (y + 1 < height)
        {
            adjacentTiles[2] = new Vector2(x, y + 1);
        }
        else
        {
            adjacentTiles[2] = new Vector2(-1, -1);
        }

        if (y - 1 >= 0)
        {
            adjacentTiles[3] = new Vector2(x, y - 1);
        }
        else
        {
            adjacentTiles[3] = new Vector2(-1, -1);
        }

        return adjacentTiles;
    }

    // Create text in the game world
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
