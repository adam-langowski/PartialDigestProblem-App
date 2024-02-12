using System.Windows.Forms.DataVisualization.Charting;

namespace PDP_App
{
    class TabuAlgorithm(int tabuListSize, int percentOfIterations, int restarts, int iterations, int deletedPercentOfSolution, Form1 form1)
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
        private readonly int percentOfIterations = percentOfIterations;
        private readonly int restartCount = restarts;
        private readonly int iterations = iterations;
        private readonly int deletedPercentOfSolution = deletedPercentOfSolution;

        private readonly Random random = new();
        private readonly List<float> objectiveFunctionValues = [];

        public int progressBarUpdateIncrement; //progres bar update 

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
            Solution = addedNumbers;

            BestSolution = Solution;
            form1.richTextBox5.AppendText(string.Join(", ", Solution));
            float startingValue = CalculateObjectiveFunction();
            objectiveFunctionValues.Add(startingValue);
            form1.textBox1.Text = startingValue.ToString();
        }

        /// <summary>
        /// Generating new unitial Solution after each restart
        /// </summary>
        public void GenerateNewInitialSolution()
        {
            List<int> addedNumbers = GenerateInitialNumbers();
            Solution = addedNumbers;
            objectiveFunctionValues.Add(CalculateObjectiveFunction());
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
                addedNumbers.Add(0); // dodaj 0 do rozwiązania

                // utwórz listę indeksów elementów z D, które nie są równe 0
                List<int> indices = [];
                for (int i = 0; i < D.Count; i++)
                {
                    if (D[i] != 0)
                    {
                        indices.Add(i);
                    }
                }

                // losowo wymieszaj listę indeksów
                Random random = new();
                for (int i = 0; i < indices.Count; i++)
                {
                    int j = random.Next(i, indices.Count);
                    int temp = indices[i];
                    indices[i] = indices[j];
                    indices[j] = temp;
                }

                // iteruj po liście indeksów i próbuj dodać elementy z D do rozwiązania
                int k = 0; // licznik dodanych elementów
                foreach (int index in indices)
                {
                    // sprawdź, czy dodanie elementu nie spowoduje powtórzenia się jakiejś różnicy
                    bool isDuplicate = false;
                    foreach (int number in addedNumbers)
                    {
                        int difference = D[index] - number; // różnica między elementami
                        if (addedNumbers.Count(x => x == difference) >= D.Count(x => x == difference)) // jeśli liczba wystąpień różnicy w rozwiązaniu jest równa lub większa niż liczba wystąpień różnicy w D
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate) // jeśli nie ma powtórzenia się różnicy
                    {
                        addedNumbers.Add(D[index]); // dodaj element do rozwiązania
                        k++; // zwiększ licznik
                        if (k == 2) // jeśli dodano już 2 elementy
                        {
                            break; // zakończ pętlę
                        }
                    }
                }
            }

            return addedNumbers;
        }


        /// <summary>
        /// Start tabu
        /// </summary>
        public void SearchSolutionSpace()
        {
            if (restartCount > 0)
            {
                progressBarUpdateIncrement = restartCount / 100;
            }

            int count = 0;
            for (int r = 0; r < restartCount + 1; r++)
            {
                if (r != 0)
                    GenerateNewInitialSolution();

                List<List<int>> tabuList = [new List<int>(Solution)];

                int iter = 0;               //iteration counter
                int noImprovementCount = 0; //number of iterations without improvement

                while (iter < iterations)
                {
                    count++;
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
                    objectiveFunctionValues.Add(currentValue);
                    form1.textBox1.Text = currentValue.ToString();

                    // update the best solution if it's better 
                    if (objectiveFunction > finalObjectiveFunction)
                    {
                        BestSolution = Solution;
                        finalObjectiveFunction = objectiveFunction;
                        noImprovementCount = 0;
                    }
                    else
                    {
                        noImprovementCount++; //increment the counter
                    }

                    // Add the current solution to the tabu list and remove the oldest one if the tabu list is full
                    tabuList.Add(Solution);
                    if (tabuList.Count > tabuListSize)
                    {
                        tabuList.RemoveAt(0);
                    }

                    float numberOfIterationsBeforeDiversifying = (percentOfIterations / 100.0f) * iterations;
                    int iterationsWithoutDiversification = (int)Math.Round(numberOfIterationsBeforeDiversifying);
                    // Check if the diversification condition is met
                    if (noImprovementCount >= iterationsWithoutDiversification)
                    {
                        // Diversify the current solution
                        Diversify();
                        // Reset the counter
                        noImprovementCount = 0;
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
                if (!Solution.Contains(element))
                {
                    List<int> newSolution = new(Solution)
                    {
                        element
                    };

                    newSolution.Sort();
                    // Check if the copy is feasible, i.e. the differences of every pair of elements are in D
                    // and do not exceed their occurrences in D

                    bool isFeasible = true;

                    // A dictionary to store the differences and their counts
                    Dictionary<int, int> differences = [];

                    for (int i = 0; i < newSolution.Count - 1; i++)
                    {
                        for (int j = i + 1; j < newSolution.Count; j++)
                        {
                            int difference = newSolution[j] - newSolution[i];
                            // If the difference is not in D or exceeds its count in D
                            if (!D.Contains(difference) || (differences.ContainsKey(difference) && differences[difference] >= D.Count(x => x == difference)))
                            {
                                isFeasible = false;
                                break;
                            }
                            else // If the difference is in D and does not exceed its count in D
                            {
                                if (differences.ContainsKey(difference)) // If the difference is already in the dictionary
                                {
                                    differences[difference]++; // Increment its count
                                }
                                else // If the difference is not in the dictionary
                                {
                                    differences.Add(difference, 1); // Add it with count 1
                                }
                            }
                        }
                        if (!isFeasible)
                        {
                            break;
                        }
                    }
                    if (isFeasible)
                    {
                        neighbourhood.Add(newSolution);
                    }
                }

                if (Solution.Contains(element))
                {
                    List<int> newSolution = new(Solution);
                    newSolution.Remove(element);
                    neighbourhood.Add(newSolution);
                }
            }
            return neighbourhood;
        }


        /// <summary>
        /// Diversyfying - deleting some part of current Solution (set to 25%)
        /// </summary>
        private void Diversify()
        {
            float deletedPercent = (deletedPercentOfSolution / 1.0f); 
            int deletedValueCount = (int)Math.Round(deletedPercent / 100 * Solution.Count);

            // copy of the current solution
            List<int> newSolution = new(Solution);

            for (int i = 0; i < deletedValueCount; i++)
            {
                // Randomly select an element from the current solution, ommiting starting '0'
                int element = newSolution[random.Next(1, newSolution.Count)];

                // Remove the element 
                newSolution.Remove(element);
            }

            newSolution.Sort();

            Solution = newSolution;
        }

        /// <summary>
        /// Checks tabu list
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="tabuList"></param>
        /// <returns></returns>
        private bool IsTabu(List<int> solution, List<List<int>> tabuList)
        {
            return tabuList.Any(tabuSolution => tabuSolution.SequenceEqual(solution));
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
        /// Draw chart of objective function value changes in time
        /// </summary>
        public void DrawChart()
        {
            form1.chart1.Series.Clear();
            form1.chart1.Titles.Clear();

            Series series = new("ObjectiveFunction")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 1,
            };
            form1.chart1.Series.Add(series);

            double[] xAxisValues = Enumerable.Range(1, objectiveFunctionValues.Count).Select(x => (double)x).ToArray();
            double[] yAxisValues = objectiveFunctionValues.Select(y => (double)y).ToArray();

            series.Points.DataBindXY(xAxisValues, yAxisValues);

            Title chartTitle = new("Osiągane wartości funkcji celu w kolejnych iteracjach");
            chartTitle.Font = new Font("Arial", 11, FontStyle.Bold);
            form1.chart1.Titles.Add(chartTitle);

            form1.chart1.ChartAreas[0].AxisX.Title = "Iteracja";
            form1.chart1.ChartAreas[0].AxisY.Title = "Wartość funkcji celu";
        }


    }
}
