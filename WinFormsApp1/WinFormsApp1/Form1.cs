namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        string FileWay;
        string SaveWay;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();

        }
        private void button3_Click(object sender, EventArgs e)
        {



            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Text = "Операция прошла успешно.";
        }


    }
}