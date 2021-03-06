﻿using System;
using System.Collections;
using System.Collections.Generic;
using MapGeneration;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenController : MonoBehaviour
{
    public MapGenerator MapGenerator;
    public MapVisualizer MapVisualizer;

    public InputField seedInput;
    public Toggle SymmToggle;
    public Toggle TestMode;

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

        MapGenerator.GenerateMap(seed, IsMapSymmetric(), IsTestModeOn());
    }

    public void ClearMap()
    {
        MapVisualizer.ClearGridMap();
    }

    public bool IsMapSymmetric()
    {
        return SymmToggle.isOn;
    }

    public bool IsTestModeOn()
    {
        return TestMode.isOn;
    }
}