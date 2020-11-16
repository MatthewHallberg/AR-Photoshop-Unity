﻿using System.Collections;
using UnityEngine;

public class CreateImage : Singleton<CreateImage> {

    public GameObject imageObject;

    DocumentInfo currDoc;

    void Start() {
        WebSocketConnection.messageRecieved += OnImageReceived;
        WebSocketConnection.messageStarted += OnNewMessageIncoming;
    }

    void OnApplicationQuit() {
        WebSocketConnection.messageRecieved -= OnImageReceived;
        WebSocketConnection.messageStarted -= OnNewMessageIncoming;
    }

    public void OnNewMessageIncoming(DocumentInfo docInfo) {
        currDoc = docInfo;
        //clear error state
        TargetController.Instance.imageTransferError = false;
        TargetController.Instance.imageTrackerCreated = false;
        MoveHandle.Instance.EnableImageVisuals(true);
        //delete old images
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public void OnImageReceived(ImageMessage currImage) {

        if (TargetController.Instance.imageTransferError) {
            return;
        }

        StartCoroutine(LoadImageRoutine(currImage));
    }

    IEnumerator LoadImageRoutine(ImageMessage currImage) {

        //create image plane
        GameObject image = Instantiate(imageObject, transform);

        //set layering
        image.transform.SetAsLastSibling();
        image.GetComponent<Renderer>().material.renderQueue = 3000 + currDoc.layers - transform.childCount;

        //convert photoshop coords to unity
        if (currImage.top == 0 && currImage.bottom == currDoc.height && currImage.left == 0 && currImage.right == currDoc.width) {
            //full screen image dont move on x or y
            image.transform.localPosition = Vector3.zero;
        } else {
            float xPos = PhotoshopLeftToUnityXPos(currImage.left, currImage.width);
            float yPos = PhotoshopTopToUnityYPos(currImage.top, currImage.height);
            image.transform.localPosition = new Vector3(xPos, yPos, 0);
        }

        //scale object based on size of photoshop layer
        image.transform.localScale = new Vector3((float)currImage.width / currDoc.width, (float)currImage.height / currDoc.height, 1);

        //create texture from pixels
        Texture2D tex = new Texture2D(currImage.width, currImage.height, TextureFormat.ARGB32, false);

        try {
            tex.LoadRawTextureData(currImage.pixels.data);
        } catch (UnityException ex) {
            Debug.Log("Image send error: " + ex);
            TargetController.Instance.imageTransferError = true;
            ConnectionManager.Instance.SendMessageToPhotoshop("Image send error: please modify the document and try again.");
        }

        tex.Apply();
        //apply texture to material instance
        Renderer rend = image.GetComponent<Renderer>();
        rend.material.mainTexture = tex;

        //set layer to image target for getting tracker image
        image.layer = 8;

        Debug.Log(currImage.pixels.data.Length);

        yield return null;
    }

    float PhotoshopLeftToUnityXPos(float left, float width) {

        //images coming in mirrored on x? (I inverted the scale on the material)
        float leftAmount = left / currDoc.width;
        float halfImageWidth = width / currDoc.width / 2f;
        return leftAmount - .5f + halfImageWidth;
    }

    float PhotoshopTopToUnityYPos(float top, float height) {

        float topAmount = top / currDoc.height;
        float halfImageHeight = height / currDoc.height / 2f;
        return -topAmount + .5f - halfImageHeight;
    }
}
