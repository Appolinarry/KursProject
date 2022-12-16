using System.Xml;
using WinFormsApp1.Model;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        string FileWay;
        string SaveWay;

        bool status = false;
        List<EKB> ekb;
        List<Association> associations;

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Файл ekb (*.ekb)|*.ekb|All files(*.*)|*.*"; //Фильтр файлов
            button3.Enabled = false;

            ekb = new List<EKB>();
            associations = new List<Association>();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                richTextBox1.Text = "Ошибка загрузки файла!";
                richTextBox1.ForeColor = Color.Maroon;
                FileWay = null;
            }
            else
            {
                // получаем выбранный файл
                FileWay = openFileDialog1.FileName;
                richTextBox1.Text = FileWay;
                richTextBox1.ForeColor = Color.Green;

            }
            checkToReady();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowNewFolderButton = false;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                richTextBox2.ForeColor = Color.Green;
                SaveWay = FBD.SelectedPath;
                richTextBox2.Text = SaveWay;
            }
            else
            {
                richTextBox2.Text = "Ошибка выбора директории для сохранения";
                SaveWay = null;
                richTextBox2.ForeColor = Color.Maroon;
            }
            checkToReady();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();

            parseTheFile();
            //    convertToEKB();
            if (status)
            {
                //        saveToEkb();
                richTextBox3.Text = "Операция прошла успешно.";
            }
            else
            {
                richTextBox3.Text = "Проверьте файл для конвертации.";
            }
            
        }

        private void checkToReady() //Метод проверки выбора директории и файла
        {
            if (FileWay != null && SaveWay != null)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
        }

        private void parseTheFile() //Метод парсинга файла
        {
            try
            {
                string attr = "";
                string className = "";

                string start = "";
                string end = "";
                string typeText = "";
                string assocName = "";

                List<(string, string)> Attribute;

                FileWay = openFileDialog1.FileName;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(FileWay);
                XmlElement xRoot = xDoc.DocumentElement;
                if (xRoot != null) //Проверка корня файла на пустоту
                {
                    var child = xRoot.ChildNodes; //Начало парсинга
                    //if (child.Item(1) == null)
                    //{
                    //    status = false;
                    //    return;
                    //}
                    var know = child.Item(0).ChildNodes;
                    var templates = know.Item(6).ChildNodes;
                    for (int i = 0; i < templates.Count; i++)
                    {
                        Attribute = new List<(string, string)>();
                        var template = templates[i];
                        className = template.ChildNodes[1].InnerText;
                        //    MessageBox.Show(className);
                        var templateInside = template.ChildNodes;
                        var slots = templateInside.Item(7).ChildNodes;
                        for (int j = 0; j < slots.Count; j++)
                        {
                            var slot = slots[j];
                            attr = slot.ChildNodes[0].InnerText;
                            typeText = slot.ChildNodes[4].InnerText;
                            Attribute.Add((attr, typeText));
                        }
                        ekb.Add(new EKB(className, Attribute));
                    }
                    var grules = know.Item(8).ChildNodes;
                    for (int i = 0; i < grules.Count; i++)
                    {
                        var grule = grules[i];
                        assocName = grule.ChildNodes[1].InnerText;
                        start = grule.ChildNodes[7].InnerText;
                        end = grule.ChildNodes[8].InnerText;
                        associations.Add(new Association(assocName, start, end));
                    }
                    Console.WriteLine();
                }
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                return;
            }
        }

    }
}