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
        string umpText = "";

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "���� ekb (*.ekb)|*.ekb|All files(*.*)|*.*"; //������ ������
            button3.Enabled = false;

            ekb = new List<EKB>();
            associations = new List<Association>();
        }
        private void button1_Click(object sender, EventArgs e) //������ � �������� �������
        {
            richTextBox1.Clear(); //������ ����

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) //���� �� ��������� ����
            {
                richTextBox1.Text = "������ �������� �����!"; //����� ������ � ����
                richTextBox1.ForeColor = Color.Maroon;
                FileWay = null;
            }
            else
            {
                // �������� ��������� ����
                FileWay = openFileDialog1.FileName;
                richTextBox1.Text = FileWay; //������� ���� � �����
                richTextBox1.ForeColor = Color.Green;

            }
            checkToReady(); //��������� ��������� ������ ��������
        }

        private void button2_Click(object sender, EventArgs e) //������ � �������� �������
        {
            richTextBox2.Clear(); //������ ����

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowNewFolderButton = false;
            if (FBD.ShowDialog() == DialogResult.OK) //���� ����� �������
            {
                richTextBox2.ForeColor = Color.Green;
                SaveWay = FBD.SelectedPath;
                richTextBox2.Text = SaveWay; //���� � ����� �����
            }
            else //���� ���, �� ����� ������
            {
                richTextBox2.Text = "������ ������ ���������� ��� ����������";
                SaveWay = null;
                richTextBox2.ForeColor = Color.Maroon;
            }
            checkToReady();
        }
        private void button3_Click(object sender, EventArgs e) //������-���������
        {
            //������ ����
            richTextBox1.Clear();
            richTextBox2.Clear();

            //������ ����� ��� ������
            ekb.Clear();
            associations.Clear();
            umpText = "";

            parseTheFile(); //��������� ����
            convertToUmp();//���������� �����
            if (status)//�������� ������� �����������
            {//���� �����, �� ��������� ����
                saveToUmp();
                richTextBox3.ForeColor= Color.Green;
                richTextBox3.Text = "�������� ������ �������.";
            }
            else //����� ������� ������
            {
                richTextBox3.Text = "��������� ���� ��� �����������.";
                richTextBox3.ForeColor = Color.Maroon;
            }
            
        }

        private void convertToUmp() //���������� �����
        {
            var clearText = "";
            var dirtText = "";
            var assocStatus = false;
            for(int i = 0; i < ekb.Count; i++)
            { //��������� � ���������� ������
                assocStatus = false;
                var temp = "";
                var classText = $"\r\nclass {ekb[i].ClassName.Replace(" ", "").Replace("-", "")}";
                temp += classText;
                var startClass = "\r\n{";
                temp += startClass;
                for(int j = 0; j < ekb[i].Attribute.Count; j++) //��������� � ���������� ����������
                {
                    var param = "";
                    if (ekb[i].Attribute[j].Item2 == "String")
                    {
                        param = $"\r\n{ekb[i].Attribute[j].Item1.Replace(" ", "").Replace("-", "")};";
                        temp += param;
                    }
                    else
                    {
                        param = $"\r\n{ekb[i].Attribute[j].Item2.Replace(" ", "").Replace("-", "")} {ekb[i].Attribute[j].Item1.Replace(" ", "").Replace("-", "")};";
                        temp += param;
                    }
                //    umpText += param;
                }
                for(int j = 0; j < associations.Count; j++) //����� �����
                {
                    if (associations[j].SourceName == ekb[i].ClassName)
                    {
                        temp += $"\r\n* -- * {associations[j].TargetName.Replace(" ", "").Replace("-", "")};";
                        assocStatus = true;
                    }
                }
                if(assocStatus) //�������� � ���������� ������������������
                {
                    dirtText += temp;
                    dirtText += "\r\n}";
                }
                else
                {
                    clearText += temp;
                    clearText += "\r\n}";
                }
                
            }
            umpText += clearText; //��������� ���� ����� � ���������
            umpText += dirtText;
        }

        private void saveToUmp() //����� ���������� �����
        {
            string path = $"{SaveWay}/file.ump";
            System.IO.File.WriteAllText(path, umpText);
        }



        private void checkToReady() //����� �������� ������ ���������� � �����
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

        private void parseTheFile() //����� �������� �����
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
                if (xRoot != null) //�������� ����� ����� �� �������
                {
                    var child = xRoot.ChildNodes; //������ ��������
                    var know = child.Item(0).ChildNodes;
                    if (know.Item(6) == null)
                    {
                        return;
                    }
                    var templates = know.Item(6).ChildNodes;
                    for (int i = 0; i < templates.Count; i++) //���������� �� �������
                    {
                        Attribute = new List<(string, string)>();
                        var template = templates[i];
                        className = template.ChildNodes[1].InnerText; //��������� ����� �������
                        var templateInside = template.ChildNodes;
                        var check = templateInside.Item(7);
                        if (check == null)
                        {
                            return;
                        }
                        var slots = templateInside.Item(7).ChildNodes;
                        for (int j = 0; j < slots.Count; j++) //��������� ���������� ������
                        {
                            var slot = slots[j];
                            attr = slot.ChildNodes[0].InnerText;
                            typeText = slot.ChildNodes[4].InnerText;
                            Attribute.Add((attr, typeText));
                        }
                        ekb.Add(new EKB(className, Attribute));
                    }
                    var grules = know.Item(8).ChildNodes;
                    for (int i = 0; i < grules.Count; i++) //��������� �����
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