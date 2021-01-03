namespace MapGeneration
{
    public class MapParameters
    {
        private float waterParam;
        private float mountainParam;
        private float treeParam;

        public MapParameters(float waterParam, float mountainParam, float treeParam)
        {
            this.waterParam = waterParam;
            this.mountainParam = mountainParam;
            this.treeParam = treeParam;
        }

        public float WaterParam
        {
            get => waterParam;
            set => waterParam = value;
        }

        public float MountainParam
        {
            get => mountainParam;
            set => mountainParam = value;
        }

        public float TreeParam
        {
            get => treeParam;
            set => treeParam = value;
        }
    }
}