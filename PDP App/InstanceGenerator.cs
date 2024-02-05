namespace PDP_App
{
    class PDPInstanceGenerator
    {
        private readonly Random random;
        private readonly List<int> P;    // fragments
        private readonly List<int> D;    // multiset - tabu input
        private readonly List<int> Cuts; // map of cuts
        public bool errorsSuccessful;

        public PDPInstanceGenerator()
        {
            random = new Random();
            P = [];
            D = [];
            Cuts = [];
            errorsSuccessful = false;
        }

        /// <summary>
        /// Generating map P
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dMin"></param>
        /// <param name="dMax"></param>
        /// <param name="deletions"></param>
        /// <param name="substitutions"></param>
        /// <returns></returns>
        public List<int> GenerateSolution(int m, int dMin, int dMax)
        {
            P.Clear();
            for (int i = 0; i < m; i++)
            {
                int value = random.Next(dMin, dMax + 1);
                P.Add(value);
            }

            return P;
        }

        /// <summary>
        /// Generating map - values are cuts sites
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public List<int> GenerateCutsMap(List<int> fragments)
        {
            Cuts.Clear();

            int sum = 0;
            Cuts.Add(sum);

            foreach (int fragment in fragments)
            {
                sum += fragment;
                Cuts.Add(sum);
            }

            return Cuts;
        }

        /// <summary>
        /// Converting map P to multiset D
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public List<int> GenerateMultiset(List<int> P, int deletions, int substitutions, int minValue, int maxValue)
        {
            D.Clear();

            for (int i = 0; i < P.Count; i++)
            {
                int sum = 0;
                for (int j = i; j < P.Count; j++)
                {
                    sum += P[j];
                    D.Add(sum);
                }
            }

            ApplyErrors(D, deletions, substitutions, minValue, maxValue);

            D.Sort();

            return D;
        }

        /// <summary>
        /// Applying deletions & substitutions
        /// </summary>
        /// <param name="P"></param>
        /// <param name="deletions"></param>
        /// <param name="substitutions"></param>
        private void ApplyErrors(List<int> D, int deletions, int substitutions, int minValue, int maxValue)
        {
            // deletions
            if (deletions > D.Count)
            {
                MessageBox.Show("Żądana liczba delecji jest większa od liczności multizbioru D", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < deletions; i++)
            {
                if (D.Count > 0)
                {
                    int indexToDelete = random.Next(D.Count);
                    D.RemoveAt(indexToDelete);
                }
            }

            // substitutions
            if (substitutions > D.Count)
            {
                MessageBox.Show("Żądana liczba substytucji jest większa od liczności multizbioru D", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < substitutions; i++)
            {
                if (D.Count > 0)
                {
                    int indexToReplace = random.Next(D.Count);
                    int newValue = random.Next(minValue, maxValue);
                    D[indexToReplace] = newValue;
                }
            }

            errorsSuccessful = true;
        }
    }

}