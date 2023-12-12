class PDPInstanceGenerator
{
    private Random random;
    private List<int> P;
    private List<int> D;
    private List<int> fragments;

    public PDPInstanceGenerator()
    {
        random = new Random();
        P = [];
        D = [];
        fragments = [];
    }

    /// <summary>
    /// Generating solution P and applying given number of deletions & substitutions
    /// </summary>
    /// <param name="m"></param>
    /// <param name="dMin"></param>
    /// <param name="dMax"></param>
    /// <param name="deletions"></param>
    /// <param name="substitutions"></param>
    /// <returns></returns>
    public List<int> GenerateSolution(int m, int dMin, int dMax)
    {
        for (int i = 0; i < m; i++)
        {
            int value = random.Next(dMin, dMax + 1);
            P.Add(value);
        }
        return P;
    }

    /// <summary>
    /// Converting P to multiset D
    /// </summary>
    /// <param name="P"></param>
    /// <returns></returns>
    public List<int> GenerateMultiset(List<int> P, int deletions, int substitutions )
    {
        List<int> D = new();
        foreach (int value in P)
        {
            D.Add(value);
            foreach (int cut in P)
            {
                if (cut != value)
                {
                    int distance = Math.Abs(value - cut);
                    D.Add(distance);
                }
            }
        }

        ApplyErrors(D, deletions, substitutions);

        D.Sort();

        return D;
    }

    /// <summary>
    /// Applying deletions & substitutions
    /// </summary>
    /// <param name="P"></param>
    /// <param name="deletions"></param>
    /// <param name="substitutions"></param>
    private void ApplyErrors(List<int> D, int deletions, int substitutions)
    {
        // deletions
        for (int i = 0; i < deletions; i++)
        {
            if (D.Count > 0)
            {
                int indexToDelete = random.Next(D.Count);
                D.RemoveAt(indexToDelete);
            }
        }

        // substitutions
        for (int i = 0; i < substitutions; i++)
        {
            if (D.Count > 0)
            {
                int indexToReplace = random.Next(D.Count);
                int newValue = random.Next(1, int.MaxValue); // assuming fragment lengths are positive
                D[indexToReplace] = newValue;
            }
        }

    }
}
