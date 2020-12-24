using UnityEngine;

namespace MapGeneration
{
    public class NoiseArrayGenerator
    {
        public float[,] GenerateNoiseArray(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseArray = new float[height, width];

            System.Random randomGen = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = randomGen.Next(-100000, 100000) + offset.x;
                float offsetY = randomGen.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float noise = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += noise * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if(noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }else if(noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseArray[y, x] = noiseHeight;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseArray[y, x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseArray[y, x]);
                }

            }
            return noiseArray;
        }

        public float[,] GenerateBlueNoiseArray(int width, int height, int seed, float frequency)
        {
            float[,] noiseArray = new float[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sampleX = (x + seed) / width - 0.5f;
                    float sampleY = (y + seed) / height - 0.5f;
                    float noise = Mathf.PerlinNoise(50 * sampleX, 50 * sampleY);
                    noiseArray[y, x] = noise;
                }
            }

            return noiseArray;


        }
    }
}
