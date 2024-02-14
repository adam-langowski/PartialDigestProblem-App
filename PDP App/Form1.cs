using System.ComponentModel;
using System.Diagnostics;

namespace PDP_App
{
    public partial class Form1 : Form
    {
        private List<int> P;            // fragmenty P'
        private List<int> Cuts;         // mapa ciêæ - rozwi¹zanie
        private List<int> D;            // multizbiór wejœciowy
        private int m;                  // iloœæ fragmentów w mapie P'
        private int minLength;          // min d³ugoœæ elementów w mapie P'
        private int maxLength;          // max d³ugoœæ elementów w mapie P'
        private int deletionCount;      // liczba delecji
        private int substitutionCount;  // liczba substytucji

        private TabuAlgorithm? tabuAlgorithm = null;
        private readonly PDPInstanceGenerator instanceGenerator = new();

        private BackgroundWorker tabuWorker; // multithread
        private Stopwatch stopwatch;

        public Form1()
        {
            P = [];
            D = [];
            Cuts = [];
            InitializeComponent();
        }

        /// <summary>
        /// Updating UI from selected thread
        /// </summary>
        /// <param name="action"></param>
        public void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Gets instance input
        /// </summary>
        /// <returns></returns>
        public List<int> GetMultiSet()
        {
            return D;
        }

        /// <summary>
        /// Returns triangle number based on instance D size
        /// (no errors assumed)
        /// Tn = (n*(n+1) / 2   - return n
        /// </summary>
        /// <returns></returns>
        public int GetMaxElementsSolutionCount()
        {
            int triangularNumber = D.Count;

            // wspó³czynniki równania kwadratowego
            double a = 1;
            double b = 1;
            double c = -2 * triangularNumber;

            double delta = b * b - 4 * a * c;

            if (delta >= 0)
            {
                double root1 = (-b + Math.Sqrt(delta)) / (2 * a);
                double root2 = (-b - Math.Sqrt(delta)) / (2 * a);

                // dodatni pierwiastek
                double n = (root1 >= 0) ? root1 : root2;

                return (int)Math.Round(n) + 1;
            }

            MessageBox.Show("Error in calculating Solution size");
            return -1;
        }

        /// <summary>
        /// Generating instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(numericUpDown4.Text) || string.IsNullOrEmpty(numericUpDown5.Text) || string.IsNullOrEmpty(numericUpDown6.Text) ||
                string.IsNullOrEmpty(numericUpDown7.Text) || string.IsNullOrEmpty(numericUpDown8.Text))
            {
                MessageBox.Show("¯adne wartoœci parametrów nie mog¹ byæ puste", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            m = Convert.ToInt32(numericUpDown4.Text);

            minLength = Convert.ToInt32(numericUpDown5.Text);
            maxLength = Convert.ToInt32(numericUpDown6.Text);

            if (minLength < 1 || maxLength < 1)
            {
                MessageBox.Show("Podaj dodatnie wartoœci d³ugoœci fragmentów", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (minLength > maxLength)
            {
                MessageBox.Show("Maksymalna d³ugoœæ fragmentu musi byæ d³u¿sza od minimalnej", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            deletionCount = Convert.ToInt32(numericUpDown7.Text);
            substitutionCount = Convert.ToInt32(numericUpDown8.Text);
            if (deletionCount < 0 || substitutionCount < 0)
            {
                MessageBox.Show("Podaj nieujemne iloœci danych b³êdów ", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            P = instanceGenerator.GenerateSolution(m, minLength, maxLength);
            D = instanceGenerator.GenerateMultiset(P, deletionCount, substitutionCount, minLength, maxLength);
            Cuts = instanceGenerator.GenerateCutsMap(P);

            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox4.Clear();

            if (instanceGenerator.errorsSuccessful)
            {
                richTextBox1.AppendText(string.Join(", ", P));
                richTextBox2.AppendText(string.Join(", ", D));
                richTextBox4.AppendText(string.Join(", ", Cuts));
            }

        }

        /// <summary>
        /// Read instance from txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog1.Title = "Wybierz plik .txt z instancj¹";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                try
                {
                    string fileContent = File.ReadAllText(filePath);

                    //sprawdzenie formy pliku
                    if (IsValidFileContent(fileContent))
                    {
                        List<int> D = fileContent.Split(',').Select(int.Parse).ToList();
                        D.Sort();

                        this.D = D;

                        richTextBox1.Clear();
                        richTextBox2.Clear();
                        richTextBox4.Clear();
                        richTextBox2.AppendText(string.Join(", ", D));
                    }
                    else
                    {
                        MessageBox.Show("Zawartoœæ pliku nie jest w odpowiednim formacie.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wyst¹pi³ b³¹d podczas wczytywania pliku: {ex.Message}", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Sprawdzenie, czy zawartoœæ pliku zawiera tylko liczby ca³kowite oddzielone przecinkiem
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static bool IsValidFileContent(string content)
        {
            return content.Split(',').All(part => int.TryParse(part, out _));
        }

        /// <summary>
        /// Save instance to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new()
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Zapisz instancjê do pliku .txt"
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using StreamWriter sw = new(saveFileDialog1.FileName);
                sw.Write(richTextBox2.Text);
            }
        }

        /// <summary>
        /// Start tabu search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(numericUpDown1.Text) || string.IsNullOrEmpty(numericUpDown2.Text) || string.IsNullOrEmpty(numericUpDown3.Text) ||
                string.IsNullOrEmpty(numericUpDown9.Text) || textBox4.Text.ToString() == null)
            {
                MessageBox.Show("¯adne wartoœci parametrów tabu nie mog¹ byæ puste", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tabuSize = (int)numericUpDown1.Value;
            int percentOfIterations = (int)numericUpDown2.Value;
            int deletedPercentOfSolution = (int)numericUpDown9.Value;
            int restartCount = (int)numericUpDown3.Value;

            if (tabuSize < 0 || tabuSize % 1 != 0)
            {
                MessageBox.Show("Wartoœæ rozmiaru listy tabu musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (percentOfIterations < 0 || percentOfIterations % 1 != 0)
            {
                MessageBox.Show("Wartoœæ precentu iteracji bez poprawy musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (deletedPercentOfSolution < 0 || deletedPercentOfSolution % 1 != 0)
            {
                MessageBox.Show("Wartoœæ usuwanego procentu rozwi¹zania przy dywersyfikacji musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (restartCount < 0 || restartCount % 1 != 0)
            {
                MessageBox.Show("Wartoœæ liczby restartów musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(textBox4.Text, out int iterations) || iterations < 0)
            {
                MessageBox.Show("Wartoœæ liczby iteracji musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            numericUpDown3.Enabled = false;
            textBox4.Enabled = false;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown9.Enabled = false;

            stopwatch = new Stopwatch();
            stopwatch.Start();
            tabuWorker = new BackgroundWorker();
            tabuWorker.DoWork += new DoWorkEventHandler(tabuWorker_DoWork);
            tabuWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Run tabu in seperate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabuWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int tabuSize = (int)numericUpDown1.Value;
            int percentOfIterations = (int)numericUpDown2.Value;
            int deletedPercentOfSolution = (int)numericUpDown9.Value;
            int restartCount = (int)numericUpDown3.Value;
            if (!int.TryParse(textBox4.Text, out int iterations) || iterations < 0)
            {
                MessageBox.Show("Wartoœæ liczby iteracji musi byæ nieujemna i ca³kowita.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tabuAlgorithm = new(tabuSize, percentOfIterations, restartCount, iterations, deletedPercentOfSolution, this);

            tabuAlgorithm.GenerateInitialSolution();

            timer1.Start();

            tabuAlgorithm.SearchSolutionSpace();

            stopwatch.Stop();

            UpdateUI(() =>
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown9.Enabled = true;
                textBox4.Enabled = true;
            });
        }

        /// <summary>
        /// Timer updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stopwatch != null)
            {
                label23.Text = $"Czas pracy algorytmu: {stopwatch.ElapsedMilliseconds} ms";
            }
        }

        /// <summary>
        /// Stop tabu search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            tabuAlgorithm?.StopTabu();
            progressBar1.Value = 100;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            numericUpDown3.Enabled = true;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown9.Enabled = true;
            textBox4.Enabled = true;
        }
    }
}
