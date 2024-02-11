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
        public float CalculateObjectiveFunction()
        {
            objectiveFunction = (float)Solution.Count / maxCutsCount;
            return objectiveFunction;
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
            float startingValue = CalculateObjectiveFunction();
            form1.textBox1.Text = startingValue.ToString();
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
        /// Start tabu
        /// </summary>
        public void SearchSolutionSpace()
        {
            for (int r = 0; r < restartCount; r++)
            {
                List<List<int>> tabuList = [new List<int>(Solution)];

                int iter = 0; //iteration counter

                while (iter < iterations)
                {
                    List<List<int>> neighbourhood = GenerateNeighbourhood();

                    // Find the best candidate
                    List<int>? bestCandidate = null;
                    float bestCandidateFitness = -1;
                    foreach (List<int> candidate in neighbourhood)
                    {
                        if ((!IsTabu(candidate, tabuList) || SatisfiesAspirationCriterion(candidate)) && Fitness(candidate) > bestCandidateFitness)
                        {
                            bestCandidate = candidate;
                            bestCandidateFitness = Fitness(candidate);
                        }
                    }

                    if (bestCandidate == null)
                    {
                        break; //returns BestSolution
                    }

                    Solution = bestCandidate;
                    float currentValue = CalculateObjectiveFunction();
                    form1.textBox1.Text = currentValue.ToString();

                    // update the best solution if it's better 
                    if (objectiveFunction > finalObjectiveFunction)
                    {
                        BestSolution = Solution;
                        finalObjectiveFunction = objectiveFunction;
                    }

                    // Add the current solution to the tabu list and remove the oldest one if the tabu list is full
                    tabuList.Add(Solution);
                    if (tabuList.Count > tabuListSize)
                    {
                        tabuList.RemoveAt(0);
                    }

                    iter++;
                }
            }
            CalculateFinalObjectiveFunctionValue();
            form1.richTextBox3.Clear();
            form1.richTextBox3.AppendText(string.Join(", ", BestSolution));

            DrawChart();
        }

        /// <summary>
        /// Generating neighbourhood from current solution
        /// </summary>
        /// <returns></returns>
        private List<List<int>> GenerateNeighbourhood()
        {
            List<List<int>> neighbourhood = [];

            foreach (int element in D)
            {
                // Try to add the element to the current solution, if it is not already in it
                if (!Solution.Contains(element))
                {
                    // Create a copy of the current solution
                    List<int> newSolution = new(Solution)
                    {
                        // Add the element to the copy
                        element
                    };

                    // Sort the copy in ascending order
                    newSolution.Sort();

                    // Check if the copy is feasible, i.e. the differences of every pair of elements are in D
                    bool isFeasible = true;
                    for (int i = 0; i < newSolution.Count - 1; i++)
                    {
                        for (int j = i + 1; j < newSolution.Count; j++)
                        {
                            if (!D.Contains(newSolution[j] - newSolution[i]))
                            {
                                isFeasible = false;
                                break;
                            }
                        }
                        if (!isFeasible)
                        {
                            break;
                        }
                    }

                    // If the copy is feasible, add it to the neighbourhood
                    if (isFeasible)
                    {
                        neighbourhood.Add(newSolution);
                    }
                }

                // Try to remove the element from the current solution, if it is in it
                if (Solution.Contains(element))
                {
                    // Create a copy of the current solution
                    List<int> newSolution = new(Solution);

                    // Remove the element from the copy
                    newSolution.Remove(element);

                    // Add the copy to the neighbourhood
                    neighbourhood.Add(newSolution);
                }
            }

            // Return the neighbourhood list
            return neighbourhood;
        }

        /// <summary>
        /// Checks tabu list
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="tabuList"></param>
        /// <returns></returns>
        private bool IsTabu(List<int> solution, List<List<int>> tabuList)
        {
            // Loop through all solutions on the tabu list
            foreach (List<int> tabuSolution in tabuList)
            {
                // Compare the solution with the tabu solution
                bool isEqual = true;
                for (int i = 0; i < solution.Count; i++)
                {
                    if(solution.Count > tabuSolution.Count || solution[i] != tabuSolution[i])
                    {
                        isEqual = false; 
                        break;
                    }
                }

                // If the solution is equal to the tabu solution, return true
                if (isEqual)
                {
                    return true;
                }
            }

            // If the solution is not equal to any tabu solution, return false
            return false;
        }

        /// <summary>
        /// Calculates the value of the objective function for a given solution
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        private float Fitness(List<int> solution)
        {
            return (float)solution.Count / maxCutsCount;
        }

        /// <summary>
        /// Compare the value of the objective function for the solution and the best solution
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        private bool SatisfiesAspirationCriterion(List<int> solution)
        {
            return Fitness(solution) > finalObjectiveFunction;
        }

        /// <summary>
        /// Stopping tabu search
        /// </summary>
        public void StopTabu()
        {
            CalculateFinalObjectiveFunctionValue();
            form1.richTextBox3.Clear();
            form1.richTextBox3.AppendText(string.Join(", ", BestSolution));

            DrawChart();
        }

        /// <summary>
        /// Draw chart of objective function value change in time
        /// </summary>
        public void DrawChart()
        {

        }

    }
}
