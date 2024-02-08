namespace PDP_App
{
    class TabuAlgorithm(int tabuListSize, int neighbourhoodPercent, int restarts, int iterations, Form1 form1)
    {
        private readonly List<int> D = form1.GetMultiSet();  // multizbiór wejściowy
        private List<int> Solution = [];                     // rozwiązanie P
        private readonly int tabuListSize = tabuListSize;
        private readonly int neighbourhoodPercent = neighbourhoodPercent;
        private readonly int restartCount = restarts;
        private readonly int iterations = iterations;
        private float objectiveFunction;
        private readonly Form1 form1 = form1;

        /// <summary>
        /// Calculating objective function
        /// </summary>
        public void CalculateObjectiveFunction()
        {
            objectiveFunction = 0;
            form1.textBox1.Text = objectiveFunction.ToString();
        }

        /// <summary>
        /// Generating initial solution
        /// </summary>
        /// <param name="D"></param>
        public void GenerateInitialSolution()
        {
            form1.richTextBox5.Clear();

            Solution = [1, 2, 3];
            form1.richTextBox5.AppendText(string.Join(", ", Solution));
            CalculateObjectiveFunction();
        }

        /// <summary>
        /// Stopping tabu search
        /// </summary>
        public void StopTabu()
        {
            form1.textBox2.Text = objectiveFunction.ToString();
            form1.richTextBox3.Clear();
            form1.richTextBox3.AppendText(string.Join(", ", Solution));
        }

    }
}
