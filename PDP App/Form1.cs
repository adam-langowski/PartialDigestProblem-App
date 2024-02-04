namespace PDP_App
{
    public partial class Form1 : Form
    {
        private List<int> P; // mapa P'
        private List<int> D; // multizbi�r wej�ciowy
        private int m; // ilo�� element�w w rozwi�zaniu
        private int minLength; // min d�ugo�� element�w w mapie P'
        private int maxLength; // max d�ugo�� element�w w mapie P'
        private int deletionCount; // liczba delecji
        private int substitutionCount; // liczba substytucji

        private readonly PDPInstanceGenerator instanceGenerator = new();

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
            m = Convert.ToInt32(domainUpDown1.Text);

            minLength = Convert.ToInt32(domainUpDown2.Text);
            maxLength = Convert.ToInt32(domainUpDown3.Text);
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

            deletionCount = Convert.ToInt32(domainUpDown4.Text);
            substitutionCount = Convert.ToInt32(domainUpDown5.Text);
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
        /// Read instance from csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
