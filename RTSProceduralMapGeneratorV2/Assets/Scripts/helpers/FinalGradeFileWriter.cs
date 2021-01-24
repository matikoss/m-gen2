using System;
using System.IO;
using MapGeneration;

namespace helpers
{
    public class FinalGradeFileWriter
    {
        public static void CreateGradesFile(EvoMapWrapper wrappedMap, string mapName)
        {
            string defaultMapName = "MapaTestowaGrades";
            if (mapName == "")
            {
                mapName = defaultMapName;
            }

            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileName = mapName + "Grades.txt";
            using (StreamWriter file = new StreamWriter(Path.Combine(dir, fileName)))
            {
                file.WriteLine("Seed: " + wrappedMap.Map.Seed);
                file.WriteLine("Final grade: " + wrappedMap.Rating);
                file.WriteLine("Avg distance grade: " + wrappedMap.AvgResRating);
                file.WriteLine("Height grade: " + wrappedMap.HeightRating);
                file.WriteLine("Humidity grade: " + wrappedMap.HumidityRating);
                file.WriteLine("Min max resources grade: " + wrappedMap.MINMaxRating);
                file.WriteLine("Second resource distance grade " + wrappedMap.SecondResourceRating);
                file.WriteLine("Player distance grade: " + wrappedMap.PDistanceRating);
                file.WriteLine("Absolute distance grade: " + wrappedMap.AbsDistRating);
                file.WriteLine("Resources availability grade: " + wrappedMap.ResAvailRating);
            }
        }
    }
}