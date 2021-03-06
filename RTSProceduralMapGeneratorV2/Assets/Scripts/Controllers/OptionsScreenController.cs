﻿using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using WyrmsunMapExporting;
using static UnityEngine.UI.Dropdown;

public class OptionsScreenController : MonoBehaviour
{
    public Slider MapSizeSlider;
    public Text MapSizeNumber;
    public Dropdown PlayerOneRaceDropdown;
    public Dropdown PlayerTwoRaceDropdown;
    public InputField WoodInput;
    public InputField CopperInput;
    public InputField StoneInput;
    public InputField MapNameInput;

    public void UpdateSliderNumber()
    {
        int sliderValue = (int) MapSizeSlider.value;
        MapSizeNumber.text = sliderValue.ToString();
    }

    public int GetMapSize()
    {
        return (int) MapSizeSlider.value;
    }

    public void LoadRacesToDropdowns(List<WyrmsunRace> factions)
    {
        PlayerOneRaceDropdown.options = new List<OptionData>();
        PlayerTwoRaceDropdown.options = new List<OptionData>();
        foreach (var faction in factions)
        {
            var option = new OptionData();
            option.text = faction.raceName;
            PlayerOneRaceDropdown.options.Add(option);
            PlayerTwoRaceDropdown.options.Add(option);
        }
    }

    public WyrmsunRace GetPlayerRaceById(int id)
    {
        int playerRace;
        if (id == 0)
        {
            playerRace = PlayerOneRaceDropdown.value;
        }
        else
        {
            playerRace = PlayerTwoRaceDropdown.value;
        }

        if (playerRace == (int) RaceDropDownEnum.Dwarves)
        {
            return new WyrmsunRace(0, RaceDropDownEnum.Dwarves.ToString(), WyrmRaceTypes.DWARVES);
        }

        if (playerRace == (int) RaceDropDownEnum.Goblins)
        {
            return new WyrmsunRace(1, RaceDropDownEnum.Goblins.ToString(), WyrmRaceTypes.GOBLINS);
        }

        return new WyrmsunRace(2, RaceDropDownEnum.Germans.ToString(), WyrmRaceTypes.GERMANS);
    }

    public int GetStartWoodAmount()
    {
        string woodString = WoodInput.text;
        if (woodString == null)
        {
            return -1;
        }

        try
        {
            return int.Parse(woodString);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return -1;
        }
    }

    public int GetStartCopperAmount()
    {
        string copperString = CopperInput.text;
        if (copperString == null)
        {
            return -1;
        }

        try
        {
            return int.Parse(copperString);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return -1;
        }
    }

    public int GetStartStoneAmount()
    {
        string stoneString = StoneInput.text;
        if (stoneString == null)
        {
            return -1;
        }

        try
        {
            return int.Parse(stoneString);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return -1;
        }
    }

    public string GetMapName()
    {
        string mapName = MapNameInput.text;
        if (mapName == null)
        {
            return "";
        }

        return mapName;
    }
}