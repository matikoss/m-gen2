using System;
using System.Collections;
using System.Collections.Generic;
using MapGeneration;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenController : MonoBehaviour
{
    public MapGenerator MapGenerator;
    public InputField seedInput;
    public MapVisualizer MapVisualizer;
    
    public void GenerateMapOnClick()
    {
        string seedString = seedInput.text;
        if (seedString == null)
        {
            return;
        }

        int seed;
        try
        {
            seed = int.Parse(seedString);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
        MapGenerator.GenerateMap(seed);
    }
    
    public void ClearMap()
    {
        MapVisualizer.ClearGridMap();
    }
}
