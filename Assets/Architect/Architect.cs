﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Architect : Player
{

    public List<GameObject> blocks = new List<GameObject>();
    public Material tilePreviewMaterial;

    private Camera camera;
    private AudioSource placeSound;

    public Vector3 destination = new Vector3(0, 0, 0);

    private int currentX;
    private int currentZ;
    private string direction;
    private string lastDirection;
    private int rotation;

    private System.Random random = new System.Random();
    private float lastActionTime;
    private GameObject currentBlock = null;

    private int blockSize = 16;
    private int placeDelay = 1;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        camera = GameObject.Find("architectCamera").GetComponent<Camera>();
        placeSound = GameObject.Find("placeSound").GetComponent<AudioSource>();

        currentX = 0;
        currentZ = 0;
        direction = "forward";
        rotation = 0;
        lastActionTime = 0;
    }

    [Command] private void CmdPlaceFinalBlock()
    {
        // Make it a final platform (solid)
        RpcPlaceFinalBlock(currentBlock);

        // Update UI/Sounds...
        placeSound.Play();
        CmdAddScore();

        // Update variables
        lastDirection = direction;
        lastActionTime = Time.time;
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
        currentBlock = null;
    }
    [ClientRpc] private void RpcPlaceFinalBlock(GameObject obj)
    {
        // Make the preview platform solid
        MeshCollider[] bc = obj.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < bc.Length; i++)
        {
            bc[i].enabled = true;
        }

        // Remove the green tint
        MeshRenderer[] mr = obj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mr.Length; i++)
        {
            Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length - 1];
            for (int j = 0; j < newMaterials.Length; j++)
            {
                newMaterials[j] = oldMaterials[j];
            }
            mr[i].materials = newMaterials;
        }

        destination = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
    }

    [Command] public void CmdPlacePreviewBlock()
    {
        // Spawn platform
        int randomBlockID = random.Next(0, blocks.Count);
        currentBlock = Instantiate(
            blocks[randomBlockID],
            new Vector3(currentX, blocks[randomBlockID].transform.position.y, currentZ),
            Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(currentBlock, this.gameObject);

        // Make it a preview platform (not solid)
        RpcPlacePreviewBlock(currentBlock);
    }
    [ClientRpc] public void RpcPlacePreviewBlock(GameObject obj)
    {
        // Make the preview platform not solid
        MeshCollider[] bc = obj.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < bc.Length; i++)
        {
            bc[i].enabled = false;
        }

        // Make the preview platform have a green tint
        MeshRenderer[] mr = obj.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < mr.Length; i++)
        {
            Material[] oldMaterials = mr[i].materials;
            Material[] newMaterials = new Material[oldMaterials.Length + 1];
            for (int j = 0; j < oldMaterials.Length; j++)
            {
                newMaterials[j] = oldMaterials[j];
            }
            newMaterials[oldMaterials.Length] = tilePreviewMaterial;
            mr[i].materials = newMaterials;
        }
        
    }

    void Update()
    {
        base.Update();

        // Update Architect position (to make driverCamera move)
        transform.position = Vector3.Lerp(transform.position, destination, 0.1f);

        if (!isLocalPlayer || !GameObject.Find("Driver(Clone)") || gamePaused)
            return;

        if (currentBlock != null)
        {
            // Update position and rotation of the preview platform
            updatePreview();

            // Make it a final platform when clicking
            if (Input.GetMouseButtonDown(0))
            {
                CmdPlaceFinalBlock();
            }
        }
        
       
        if(currentBlock == null)
        {
            Vector3 carPosition = GameObject.Find("Driver(Clone)").transform.position;
            float distance = Vector3.Distance(new Vector3(currentX, 0, currentZ), new Vector3(carPosition.x, 0, carPosition.z));

            if (distance < 28)
            {
                CmdPlacePreviewBlock();
            }
        }
        
    }

    void updatePreview()
    {
        // Update position
        camera = GameObject.Find("architectCamera").GetComponent<Camera>();
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

        // Update rotation
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
    }

}