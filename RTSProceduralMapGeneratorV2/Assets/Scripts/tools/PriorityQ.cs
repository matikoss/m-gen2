using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace tools
{
    public class PriorityQ<T> where T : MapNode, IComparable<T>
    {
        private List<T> data;

        private int heapSize = -1;

        public PriorityQ()
        {
            this.data = new List<T>();
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool isEmpty()
        {
            return data.Count == 0;
        }

        public void AddToQueue(T obj)
        {
            data.Add(obj);
            heapSize++;
            BuildHeapMax(heapSize);
        }

        public T PopFromQueue()
        {
            if (heapSize > -1)
            {
                var returnVal = data[0];
                data[0] = data[heapSize];
                data.RemoveAt(heapSize);
                heapSize--;
                Heapify(0);
                return returnVal;
            }
            else
                throw new Exception("Queue is empty");
        }

        public T ShowFirst()
        {
            return data[0];
        }

        public void UpdatePriority(T obj, int newDistance)
        {
            int i = 0;
            for (; i <= heapSize; i++)
            {
                T element = data[i];
                if (element.Equals(obj))
                {
                    element.DistanceFromStart = newDistance;
                    BuildHeapMax(i);
                    Heapify(i);
                }
            }
        }

        public void RestoreQueue()
        {
            List<T> tmp = data;
            data = new List<T>();
            heapSize = -1;
            foreach (var item in tmp)
            {
                AddToQueue(item);
            }
        }

        public void RestoreQueueV2(List<MapNode> changed)
        {
            for (int i = 0; i <= heapSize; i++)
            {
                T elemToCheck = data[i];
                foreach (var test in changed)
                {
                    if (elemToCheck.Equals(test))
                    {
                        BuildHeapMax(i);
                        Heapify(i);
                    }
                }
            }
        }

        private void BuildHeapMax(int i)
        {
            while (i >= 0 && data[(i - 1) / 2].CompareTo(data[i]) < 0)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void Heapify(int i)
        {
            int left = ChildL(i);
            int right = ChildR(i);

            int height = i;

            if (left <= heapSize && data[height].CompareTo(data[left]) < 0)
                height = left;
            if (right <= heapSize && data[height].CompareTo(data[right]) < 0)
                height = right;

            if (height != i)
            {
                Swap(height, i);
                Heapify(height);
            }
        }

        private void Swap(int i, int j)
        {
            var temp = data[i];
            data[i] = data[j];
            data[j] = temp;
        }

        private int ChildL(int i)
        {
            return i * 2 + 1;
        }

        private int ChildR(int i)
        {
            return i * 2 + 2;
        }
    }
}