using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class Snake : MonoBehaviour
{
    //public float timeScale = 1;

    // Food Prefab
    public GameObject foodPrefab;

    public GameObject GridObject;
    private Grid<int> Grid;

    // Borders
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderLeft;
    public Transform borderRight;

    GameObject currentFood;

    bool turned = false;

    string turnDirection = "";
    string previousTurnDirection = "";

    // Current Movement Direction
    // (by default it moves to the right)
    Vector2 dir = Vector2.right;

    int nextDirection = 0;
    int widthOffset;
    int heightOffset;

    // Keep Track of Tail
    List<Transform> tail = new List<Transform>();

    // Did the snake eat something?
    bool ate = false;

    // Tail Prefab
    public GameObject tailPrefab;

    // Use this for initialization
    void Start()
    {
        // Speed up time
        Time.timeScale = 50.0f;

        Grid = GridObject.GetComponent<TestingGrid>().GetGrid();

        widthOffset = GridObject.GetComponent<TestingGrid>().width / 2;
        heightOffset = GridObject.GetComponent<TestingGrid>().height / 2;

        SpawnFood();

        // Move the Snake every 300ms
        InvokeRepeating("Move", 0.3f, 0.3f);
    }

    // Update is called once per Frame
    void FixedUpdate()
    {
        //previousTurnDirection = turnDirection;
        // Manual controls
        if (Input.GetKey(KeyCode.RightArrow)) {
            if (Vector2.right != -dir) {
                dir = Vector2.right;
                turnDirection = "right";
                turned = true;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (-Vector2.up != -dir)
            {
                dir = -Vector2.up;    // '-up' means 'down'
                turnDirection = "down";
                turned = true;
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (-Vector2.right != -dir)
            {
                dir = -Vector2.right; // '-right' means 'left'
                turnDirection = "left";
                turned = true;
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Vector2.up != -dir)
            {
                dir = Vector2.up;
                turnDirection = "up";
                turned = true;
            }
        }

        
        if (currentFood != null)
        {
            ChooseDirection();
            if (nextDirection == 0)
            {
                turnDirection = "right";
                turned = true;
                dir = Vector2.right;
            }
            if (nextDirection == 1)
            {
                turnDirection = "left";
                turned = true;
                dir = -Vector2.right;
            }
            if (nextDirection == 2)
            {
                turnDirection = "up";
                turned = true;
                dir = Vector2.up;
            }
            if (nextDirection == 3)
            {
                turnDirection = "down";
                turned = true;
                dir = -Vector2.up;
            }
        }

        //// Move in a new Direction?
        //if (currentFood != null) {
        //    if ((int)gameObject.transform.position.x < (int)currentFood.gameObject.transform.position.x && dir != Vector2.right)
        //    {
        //        if (Vector2.right != -dir)
        //        {
        //            turnDirection = "right";
        //            turned = true;
        //        }
        //    }
        //    if ((int)gameObject.transform.position.x > (int)currentFood.gameObject.transform.position.x && dir != -Vector2.right)
        //    {
        //        if (-Vector2.right != -dir)
        //        {
        //            turnDirection = "left";
        //            turned = true;
        //        }
        //    }
        //    if ((int)gameObject.transform.position.y > (int)currentFood.gameObject.transform.position.y && dir != -Vector2.up)
        //    {
        //        if (-Vector2.up != -dir)
        //        {
        //            turnDirection = "down";
        //            turned = true;
        //        }
        //    }
        //    if ((int)gameObject.transform.position.y < (int)currentFood.gameObject.transform.position.y && dir != Vector2.up)
        //    {
        //        if (Vector2.up != -dir)
        //        {
        //            turnDirection = "up";
        //            turned = true;
        //        }
        //    }

        //    switch (turnDirection)
        //    {
        //        case "right":
        //            dir = Vector2.right;
        //            break;
        //        case "left":
        //            dir = -Vector2.right; // '-right' means 'left'
        //            break;
        //        case "up":
        //            dir = Vector2.up;
        //            break;
        //        case "down":
        //            dir = -Vector2.up;    // '-up' means 'down'
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    void ChooseDirection()
    {
        int FoodLocationNumber = Grid.GetValue(currentFood.transform.position);
        int HeadLocationNumber = Grid.GetValue(gameObject.transform.position);

        int[] AdjacentTilesNumbers = GetAdjacentTilesValues(gameObject.transform.position);
        List<int> UnavailableTileIndex = new List<int>();

        // Find unavailable adjacent tiles
        for (int i = 0; i < tail.Count; i++)
        {
            for (int j = 0; j < AdjacentTilesNumbers.Length; j++)
            {
                if (Grid.GetValue(tail[i].transform.position) == AdjacentTilesNumbers[j])
                    //(Grid.GetValue(tail[i].transform.position) > AdjacentTilesNumbers[j] && AdjacentTilesNumbers[j] > Grid.GetValue(gameObject.transform.position)) ||
                    //(Grid.GetValue(tail[i].transform.position) < AdjacentTilesNumbers[j] && AdjacentTilesNumbers[j] < Grid.GetValue(gameObject.transform.position)))
                {
                    UnavailableTileIndex.Add(j);
                }
            }
        }

        // Get the tile numbers of all the available tiles
        List<int> AvailableTilesNumbers = new List<int>();

        for (int i = 0; i < AdjacentTilesNumbers.Length; i++)
        {
            bool TileAvailable = true;
            for (int j = 0; j < UnavailableTileIndex.Count; j++)
            {
                if (i == UnavailableTileIndex[j])
                {
                    TileAvailable = false;
                }
            }

            if (TileAvailable)
            {
                AvailableTilesNumbers.Add(AdjacentTilesNumbers[i]);
            }
        }

        //for (int i = 0; i < AvailableTilesNumbers.Count; i++)
        //{
        //    Debug.Log("Available Tile: " + AvailableTilesNumbers[i]);
        //}


        int closestTile = 0;
        int shortestDistance = 1000000000;

        for (int i = 0; i < AvailableTilesNumbers.Count; i++)
        {
            if (Mathf.Abs(AvailableTilesNumbers[i] - FoodLocationNumber) < shortestDistance)
            {
                closestTile = AvailableTilesNumbers[i];
                shortestDistance = Mathf.Abs(AvailableTilesNumbers[i] - FoodLocationNumber);
                //Debug.Log("Shortest Distance: " + shortestDistance);
            }
        }

        nextDirection = System.Array.IndexOf(AdjacentTilesNumbers, closestTile);
        //Debug.Log(nextDirection);

        //Debug.Log(shortestDistance);
        //Debug.Log(closestTile);
    }

    void Move()
    {
        // Save current position (gap will be here)
        Vector2 v = transform.position;
        bool spawnNewFood = true;

        // Move head into new direction (now there is a gap)
        transform.Translate(dir);

        if (tail.Count < 2) {
            ate = true;
            spawnNewFood = false;
        }

        // Ate something? Then insert new Element into gap
        if (ate)
        {
            // Load Prefab into the world
            GameObject g = (GameObject)Instantiate(tailPrefab,
                                                  v,
                                                  Quaternion.identity);

            // Keep track of it in our tail list
            tail.Insert(0, g.transform);

            // Reset the flag
            ate = false;


            if (spawnNewFood) {
                SpawnFood();
            }
           
        }
        // Do we have a Tail?
        else if (tail.Count > 0)
        {
            // Move last Tail Element to where the Head was
            tail.Last().position = v;

            // Add to front of list, remove from the back
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }

        //Debug.Log("Head: " + GridObject.GetComponent<TestingGrid>().GetGrid().GetValue(gameObject.transform.position));
        //Debug.Log(GetAdjacentTilesValues(gameObject.transform.position)[0]);
        //Debug.Log(GetAdjacentTilesValues(gameObject.transform.position)[1]);
        //Debug.Log(GetAdjacentTilesValues(gameObject.transform.position)[2]);
        //Debug.Log(GetAdjacentTilesValues(gameObject.transform.position)[3]);
        //Debug.Log("Food: " + GridObject.GetComponent<TestingGrid>().GetGrid().GetValue(currentFood.transform.position));
        WriteString();
        previousTurnDirection = turnDirection;
    }

    // Spawn one piece of food
    void SpawnFood()
    {
        // x position between left & right border
        int x = (int)Random.Range(borderLeft.position.x + 1f,
                                  borderRight.position.x - 1f);

        // y position between top & bottom border
        int y = (int)Random.Range(borderBottom.position.y + 1f,
                                  borderTop.position.y - 1f);

        // Instantiate the food at (x, y)
        currentFood = Instantiate(foodPrefab,
                    new Vector2(x, y),
                    Quaternion.identity); // default rotation
    }

    int[] GetAdjacentTilesValues(Vector3 currentPosition) {
        Vector2[] adjacentTiles;
        int[] adjacentValues = new int[4]; 
        Grid<int> grid = GridObject.GetComponent<TestingGrid>().GetGrid();

        int x, y;
        grid.GetXY(currentPosition, out x, out y);

        adjacentTiles = grid.GetAdjacentTiles(x, y);

        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            adjacentValues[i] = grid.GetValue((int)adjacentTiles[i].x, (int)adjacentTiles[i].y);
        }
        return adjacentValues;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // Food?
        if (coll.name.StartsWith("FoodPrefab"))
        {
            // Get longer in next Move call
            ate = true;

            // Remove the Food
            Destroy(coll.gameObject);
        }
        // Collided with Tail or Border
        else
        {
            // ToDo 'You lose' screen
            SceneManager.LoadScene("Game");
        }
    }

    void WriteString()
    {
        string path = "Assets/Text/snake_gameplay.txt";
        string turnDirectionString = "";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);

        string tailPositions = "";

        for (int i = 0; i < tail.Count; i++)
        {
            tailPositions += (int)(tail[i].transform.position.x + widthOffset) + "," + (int)(tail[i].transform.position.y + heightOffset) + ":";
        }

        if (previousTurnDirection == turnDirection)
        {
            turnDirectionString = "none";
        }
        else
        {
            turnDirectionString = turnDirection;
        }

        writer.WriteLine(turnDirectionString + ":" + (int)(transform.position.x + widthOffset) + "," +  (int)(transform.position.y + heightOffset) + ":" + (int)(currentFood.transform.position.x + widthOffset) + "," + (int)(currentFood.transform.position.y + heightOffset) + ":" + tailPositions);
        writer.Close();

        turned = false;
    }
}
