namespace PDP_App
{
    class TabuAlgorithm
    {
        private List<int> D;        // multizbiór wejściowy
        private List<int> Solution; // rozwiązanie P
        private int tabuListSize;
        private int neighbourhoodPercent;
        private int restartCount;
        private int iterations;
        private readonly Form1 form1 = new();
        private float objectiveFunction;

        public TabuAlgorithm()
        {
            Solution = [];
            D = form1.GetMultiSet();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CalculateObjectiveFunction()
        {
            objectiveFunction = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="D"></param>
        public void GenerateInitialSolution(List<int> D)
        {
            
            //Solution
            //objectiveFunction 
        } 

    }
}
