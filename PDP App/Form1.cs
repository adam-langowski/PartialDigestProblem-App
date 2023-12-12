namespace PDP_App
{
    public partial class Form1 : Form
    {
        private int m; // ilo�� element�w w rozwi�zaniu
        private int minLength; // d�ugo�� element�w - od
        private int maxLength; // d�ugo�� element�w - do
        private int deletionCount; // liczba delecji
        private int substitutionCount; // liczba substytucji

        private PDPInstanceGenerator instanceGenerator = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Pobierz warto�ci z DomainUpDown i przekonwertuj je na int
            m = Convert.ToInt32(domainUpDown1.Text);
            minLength = Convert.ToInt32(domainUpDown2.Text);
            maxLength = Convert.ToInt32(domainUpDown3.Text);
            deletionCount = Convert.ToInt32(domainUpDown4.Text);
            substitutionCount = Convert.ToInt32(domainUpDown5.Text);

            // Wygeneruj instancj� przy u�yciu PDPInstanceGenerator
            List<int> P = instanceGenerator.GenerateSolution(m, minLength, maxLength);
            List<int> D = instanceGenerator.GenerateMultiset(P, deletionCount, substitutionCount);

            // Przypisz wyniki do odpowiednich DataGridView
            dataGridView2.DataSource = CreateBindingSource(P);
            dataGridView3.DataSource = CreateBindingSource(D);
        }

        private static BindingSource CreateBindingSource(List<int> data)
        {
            // Utw�rz BindingSource i przypisz dane
            BindingSource bindingSource = new()
            {
                DataSource = data
            };
            return bindingSource;
        }
    }
}
