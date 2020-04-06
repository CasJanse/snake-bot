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

    // Borders
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderLeft;
    public Transform borderRight;

    GameObject currentFood;

    bool turned = false;

    string turnDirection = "";

    // Current Movement Direction
    // (by default it moves to the right)
    Vector2 dir = Vector2.right;

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
        Time.timeScale = 5.0f;

        SpawnFood();

        // Move the Snake every 300ms
        InvokeRepeating("Move", 0.3f, 0.3f);
    }

    // Update is called once per Frame
    void Update()
    {
        // Move in a new Direction?
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

        WriteString();
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

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log(coll.name);
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
        string path = "Assets/Text/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);

        string tailPositions = "";

        for (int i = 0; i < tail.Count; i++)
        {
            tailPositions += (int)tail[i].transform.position.x + "," + (int)tail[i].transform.position.y + ",";
        }

        if (!turned) {
            turnDirection = "none";
        }

        writer.WriteLine(turnDirection + "," + (int)transform.position.x + "," +  (int)transform.position.y + "," + (int)currentFood.transform.position.x + "," + (int)currentFood.transform.position.y + "," + tailPositions);
        writer.Close();

        turned = false;

        ////Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = Resources.Load("test");

        ////Print the text from the file
        //Debug.Log(asset.text);
    }
}
