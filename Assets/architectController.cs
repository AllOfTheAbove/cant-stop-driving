using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class architectController : MonoBehaviour
{

    public List<GameObject> blocks = new List<GameObject>();
    public Camera camera;
    public AudioSource placeSound;
    public Text scoreText;
    public Material prepareMaterial;
    public Material placedMaterial;

    private int score;
    private int currentX;
    private int currentZ;
    private string direction;
    private string lastDirection;
    private int rotation;
    private Vector3 cameraVelocity = Vector3.zero;

    private System.Random random = new System.Random();
    private float lastActionTime;
    private int currentBlockID = 0;
    private GameObject currentBlock;

    private int blockSize = 16;
    private int placeDelay = 1;

    void Start()
    {
        score = 0;
        currentX = 0;
        currentZ = 0;
        direction = "forward";
        rotation = 0;
        lastActionTime = 0;

        preparePlacing();
    }

    private void place()
    {
        if (direction == "forward")
        {
            currentZ = currentZ + blockSize;
        }
        else if (direction == "left")
        {
            currentX = currentX - blockSize;
        }
        else if (direction == "right")
        {
            currentX = currentX + blockSize;
        }
        currentBlock.GetComponent<BoxCollider>().isTrigger = false;
        Material[] materials = currentBlock.GetComponent<MeshRenderer>().materials;
        materials[0] = placedMaterial;
        currentBlock.GetComponent<MeshRenderer>().materials = materials;
        placeSound.Play();
        score = score + 5;
        scoreText.text = score.ToString();
        lastDirection = direction;
        lastActionTime = Time.time;

        preparePlacing();
    }

    private void preparePlacing()
    {
        currentBlockID = random.Next(0, blocks.Count);
        currentBlock = Instantiate(blocks[currentBlockID], new Vector3(currentX, 0, currentZ), Quaternion.identity);
        Material[] materials = currentBlock.GetComponent<MeshRenderer>().materials;
        materials[0] = prepareMaterial;
        currentBlock.GetComponent<MeshRenderer>().materials = materials;
        currentBlock.GetComponent<BoxCollider>().isTrigger = true;
    }

    private void LateUpdate()
    {
        Vector3 destination = new Vector3(currentX, 100, currentZ);
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, destination, ref cameraVelocity, 0.25f);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            rotation++;
            if (rotation > 3)
            {
                rotation = 0;
            }
            currentBlock.transform.Rotate(0, 90, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            rotation--;
            if (rotation < 0)
            {
                rotation = 3;
            }
            currentBlock.transform.Rotate(0, -90, 0);
        }

        Vector3 pos = currentBlock.transform.position;
        Vector3 point = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
        if (point.x < currentX && lastDirection != "right")
        {
            pos.x = currentX - blockSize;
            pos.z = currentZ;
            currentBlock.transform.position = pos;
            direction = "left";
        }
        if (point.x > currentX && lastDirection != "left")
        {
            pos.x = currentX + blockSize;
            pos.z = currentZ;
            currentBlock.transform.position = pos;
            direction = "right";
        }
        if (Math.Abs(point.z - currentZ) > Math.Abs(point.x - currentX))
        {
            pos.x = currentX;
            pos.z = currentZ + blockSize;
            currentBlock.transform.position = pos;
            direction = "forward";
        }

        if (Input.GetMouseButtonDown(0) && lastActionTime + placeDelay < Time.time)
        {
            place();
        }
    }
}
