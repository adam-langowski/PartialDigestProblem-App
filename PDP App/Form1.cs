namespace PDP_App
{
    public partial class Form1 : Form
    {
        private List<int> P; // mapa P'
        private List<int> D; // multizbi�r wej�ciowy
        private int m; // ilo�� fragment�w w mapie P'
        private int minLength; // min d�ugo�� element�w w mapie P'
        private int maxLength; // max d�ugo�� element�w w mapie P'
        private int deletionCount; // liczba delecji
        private int substitutionCount; // liczba substytucji

        private readonly PDPInstanceGenerator instanceGenerator = new();

        public List<int> GetMultiSet() {
            return D;        
        }   

        public Form1()
        {
            P = [];
            D = [];
            InitializeComponent();
        }

        /// <summary>
        /// Generating instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            m = Convert.ToInt32(numericUpDown4.Text);

            minLength = Convert.ToInt32(numericUpDown5.Text);
            maxLength = Convert.ToInt32(numericUpDown6.Text);
            if (minLength < 1 || maxLength < 1)
            {
                MessageBox.Show("Podaj dodatnie warto�ci d�ugo�ci fragment�w", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (minLength > maxLength)
            {
                MessageBox.Show("Maksymalna d�ugo�� fragmentu musi by� d�u�sza od minimalnej", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            deletionCount = Convert.ToInt32(numericUpDown7.Text);
            substitutionCount = Convert.ToInt32(numericUpDown8.Text);
            if (deletionCount < 0 || substitutionCount < 0)
            {
                MessageBox.Show("Podaj nieujemne ilo�ci danych b��d�w ", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            P = instanceGenerator.GenerateSolution(m, minLength, maxLength);
            D = instanceGenerator.GenerateMultiset(P, deletionCount, substitutionCount, minLength, maxLength);

            richTextBox1.Clear();
            richTextBox2.Clear();

            if (instanceGenerator.errorsSuccessful)
            {
                richTextBox1.AppendText(string.Join(", ", P));
                richTextBox2.AppendText(string.Join(", ", D));
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
            openFileDialog1.Title = "Wybierz plik .txt z instancj�";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string fileContent = File.ReadAllText(filePath);

                List<int> D = fileContent.Split(',').Select(int.Parse).ToList();

                D.Sort();

                this.D = D;

                richTextBox1.Clear();
                richTextBox2.Clear();
                richTextBox2.AppendText(string.Join(", ", D));
            }
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
                Title = "Zapisz instancj� do pliku .txt"
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new(saveFileDialog1.FileName))
                {
                    sw.Write(richTextBox2.Text);
                }
            }
        }
    }
}
