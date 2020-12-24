using System.Collections.Generic;
using UnityEngine;

namespace tools
{
    public class PoissonSampler
    {
        private const int maxAttemps = 30;

        private readonly Rect rect;
        private readonly float squaredR;
        private readonly float cellSize;
        private Vector2[,] grid;
        private List<Vector2> activeSamples = new List<Vector2>();

        public PoissonSampler(float width, float height, float radius)
        {
            rect = new Rect(0, 0, width, height);
            squaredR = radius * radius;
            cellSize = radius / Mathf.Sqrt(2);
            grid = new Vector2[Mathf.CeilToInt(width / cellSize),
                Mathf.CeilToInt(height / cellSize)];
        }

        public List<Vector2Int> GetSamples()
        {
            var samples = GenerateSamples();
            List<Vector2Int> samplesInt = new List<Vector2Int>();
            foreach (var s in samples)
            {
                samplesInt.Add(new Vector2Int((int) s.x, (int) s.y));
            }

            return samplesInt;
        }

        private IEnumerable<Vector2> GenerateSamples()
        {
            yield return AddSample(new Vector2(Random.value * rect.width, Random.value * rect.height));

            while (activeSamples.Count > 0)
            {
                int i = (int) Random.value * activeSamples.Count;
                Vector2 sample = activeSamples[i];

                bool found = false;
                for (int j = 0; j < maxAttemps; ++j)
                {
                    float angle = 2 * Mathf.PI * Random.value;
                    float
                        r = Mathf.Sqrt(Random.value * 3 * squaredR +
                                       squaredR);
                    Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    if (rect.Contains(candidate) && IsFarEnough(candidate))
                    {
                        found = true;
                        yield return AddSample(candidate);
                        break;
                    }
                }

                if (!found)
                {
                    activeSamples[i] = activeSamples[activeSamples.Count - 1];
                    activeSamples.RemoveAt(activeSamples.Count - 1);
                }
            }
        }

        private bool IsFarEnough(Vector2 sample)
        {
            GridPos pos = new GridPos(sample, cellSize);

            int xmin = Mathf.Max(pos.x - 2, 0);
            int ymin = Mathf.Max(pos.y - 2, 0);
            int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
            int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);

            for (int y = ymin; y <= ymax; y++)
            {
                for (int x = xmin; x <= xmax; x++)
                {
                    Vector2 s = grid[x, y];
                    if (s != Vector2.zero)
                    {
                        Vector2 d = s - sample;
                        if (d.x * d.x + d.y * d.y < squaredR) return false;
                    }
                }
            }

            return true;
        }

        private Vector2 AddSample(Vector2 sample)
        {
            activeSamples.Add(sample);
            GridPos pos = new GridPos(sample, cellSize);
            grid[pos.x, pos.y] = sample;
            return sample;
        }

        private struct GridPos
        {
            public int x;
            public int y;

            public GridPos(Vector2 sample, float cellSize)
            {
                x = (int) (sample.x / cellSize);
                y = (int) (sample.y / cellSize);
            }
        }
    }
}