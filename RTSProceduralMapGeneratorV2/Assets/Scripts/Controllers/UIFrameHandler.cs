using System;
using MapGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class UIFrameHandler : MonoBehaviour
    {
        public MapGenerator MapGenerator;
        public InputField seedInput;
        
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
    }
}
