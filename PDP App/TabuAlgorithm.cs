namespace PDP_App
{
    class TabuAlgorithm(int tabuListSize, int neighbourhoodPercent, int restarts, int iterations, Form1 form1)
    {
        private readonly Form1 form1 = form1;

        private readonly List<int> D = form1.GetMultiSet();  // multizbiór wejściowy
        private List<int> Solution = [];                     // rozwiązanie P
        private List<int> BestSolution = [];

        // wartość do obliczenia funkcji celu 
        private readonly int maxCutsCount = form1.GetMaxElementsSolutionCount();
        private float objectiveFunction;
        private float finalObjectiveFunction;

        private readonly int tabuListSize = tabuListSize;
        private readonly int neighbourhoodPercent = neighbourhoodPercent;
        private readonly int restartCount = restarts;
        private readonly int iterations = iterations;


        /// <summary>
        /// Calculating objective function
        /// </summary>
        public void CalculateObjectiveFunction()
        {
            objectiveFunction = (float)Solution.Count / maxCutsCount;
            form1.textBox1.Text = objectiveFunction.ToString();
        }

        /// <summary>
        /// Calculating final objective function - for BestSolution
        /// </summary>
        public void CalculateFinalObjectiveFunctionValue() 
        {
            finalObjectiveFunction = (float)BestSolution.Count / maxCutsCount;
            form1.textBox2.Text = finalObjectiveFunction.ToString();
        }

        /// <summary>
        /// Generating initial solution
        /// </summary>
        public void GenerateInitialSolution()
        {
            form1.richTextBox5.Clear();

            // wygenerowanie rozwiązania początkowego
            List<int> addedNumbers = GenerateInitialNumbers();
            Solution = [0, .. addedNumbers];

            BestSolution = Solution;
            form1.richTextBox5.AppendText(string.Join(", ", Solution));
            CalculateObjectiveFunction();
        }

        /// <summary>
        /// Returns values added to initial solution
        /// </summary>
        /// <returns></returns>
        private List<int> GenerateInitialNumbers()
        {
            List<int> addedNumbers = [];

            if (D.Count >= 2)
            {
                addedNumbers.Add(D[0]);

                int secondNumberIndex = 1;
                
                // wartość inna niż już obecna w Solution i różnica w D
                while (D[secondNumberIndex] == D[0] || !D.Contains(D[secondNumberIndex] - D[0]))
                {
                    secondNumberIndex++;
                }

                if (secondNumberIndex < D.Count)
                {
                    addedNumbers.Add(D[secondNumberIndex]);
                }
            }

            return addedNumbers;
        }

        /// <summary>
        /// Stopping tabu search
        /// </summary>
        public void StopTabu()
        {
            CalculateFinalObjectiveFunctionValue();
            form1.richTextBox3.Clear();
            form1.richTextBox3.AppendText(string.Join(", ", BestSolution));

            //chart
        }

    }
}
