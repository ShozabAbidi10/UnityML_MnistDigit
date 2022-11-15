using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will allow us to draw on top of the blank shadder and give that drawed shadder as input into our model.

public class DrawOnTexture : MonoBehaviour
{
    public Texture2D baseTexture;
   
    // Update is called once per frame
    void Update()
    {
        DoMouseDrawing();    
    }

    /// <summary>
    /// This will allow drawing on the texture using mouse
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void DoMouseDrawing()
    {
        if (Camera.main == null)
        {
            throw new Exception(message:"Cannot find main Camera");
        }


        //Is mouse being pressed?
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // do nothing if we aren't hitting anything.
        if (!Physics.Raycast(mouseRay, out hit)) return;

        // do nothing if we didn't hit anything.
        if (hit.collider.transform != transform) return;


        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= baseTexture.width;
        pixelUV.y *= baseTexture.height;

        // If left mouse button is pressed then color is white. If right mouse button is pressed then color is black.
        Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;

        baseTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorToSet);
        baseTexture.Apply();
    }
}  
