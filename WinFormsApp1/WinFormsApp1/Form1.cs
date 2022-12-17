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
            openFileDialog1.Filter = "Файл ekb (*.ekb)|*.ekb|All files(*.*)|*.*"; //Фильтр файлов
            button3.Enabled = false;

            ekb = new List<EKB>();
            associations = new List<Association>();
        }
        private void button1_Click(object sender, EventArgs e) //Работа с файловой кнопкой
        {
            richTextBox1.Clear(); //Чистим поле

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) //Если не загрузили файл
            {
                richTextBox1.Text = "Ошибка загрузки файла!"; //Пишем ошибку в поле
                richTextBox1.ForeColor = Color.Maroon;
                FileWay = null;
            }
            else
            {
                // получаем выбранный файл
                FileWay = openFileDialog1.FileName;
                richTextBox1.Text = FileWay; //Выводим путь к файлу
                richTextBox1.ForeColor = Color.Green;

            }
            checkToReady(); //Проверяем активатор кнопки конверта
        }

        private void button2_Click(object sender, EventArgs e) //Работа с папковой кнопкой
        {
            richTextBox2.Clear(); //Чистим поле

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowNewFolderButton = false;
            if (FBD.ShowDialog() == DialogResult.OK) //Если папка указана
            {
                richTextBox2.ForeColor = Color.Green;
                SaveWay = FBD.SelectedPath;
                richTextBox2.Text = SaveWay; //Путь к папке вывод
            }
            else //Если нет, то пишем ошибку
            {
                richTextBox2.Text = "Ошибка выбора директории для сохранения";
                SaveWay = null;
                richTextBox2.ForeColor = Color.Maroon;
            }
            checkToReady();
        }
        private void button3_Click(object sender, EventArgs e) //Кнопка-конвертер
        {
            //Чистим поля
            richTextBox1.Clear();
            richTextBox2.Clear();

            //Чистим листы для записи
            ekb.Clear();
            associations.Clear();
            umpText = "";

            parseTheFile(); //Разбираем файл
            convertToUmp();//Записываем новый
            if (status)//Проверка статуса конвертации
            {//Если успех, то сохраняем файл
                saveToUmp();
                richTextBox3.ForeColor= Color.Green;
                richTextBox3.Text = "Операция прошла успешно.";
            }
            else //Иначе выводим ошибку
            {
                richTextBox3.Text = "Проверьте файл для конвертации.";
                richTextBox3.ForeColor = Color.Maroon;
            }
            
        }

        private void convertToUmp() //Транслятор файла
        {
            var clearText = "";
            var dirtText = "";
            var assocStatus = false;
            for(int i = 0; i < ekb.Count; i++)
            { //Считываем и запишиваем классы
                assocStatus = false;
                var temp = "";
                var classText = $"\r\nclass {ekb[i].ClassName.Replace(" ", "").Replace("-", "")}";
                temp += classText;
                var startClass = "\r\n{";
                temp += startClass;
                for(int j = 0; j < ekb[i].Attribute.Count; j++) //Считываем и записываем переменные
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
                for(int j = 0; j < associations.Count; j++) //Пишем связи
                {
                    if (associations[j].SourceName == ekb[i].ClassName)
                    {
                        temp += $"\r\n* -- * {associations[j].TargetName.Replace(" ", "").Replace("-", "")};";
                        assocStatus = true;
                    }
                }
                if(assocStatus) //Собираем в правильной последовательности
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
            umpText += clearText; //Сохраняем весь текст в перменную
            umpText += dirtText;
        }

        private void saveToUmp() //Метод сохранения файла
        {
            string path = $"{SaveWay}/file.ump";
            System.IO.File.WriteAllText(path, umpText);
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
                    var know = child.Item(0).ChildNodes;
                    if (know.Item(6) == null)
                    {
                        return;
                    }
                    var templates = know.Item(6).ChildNodes;
                    for (int i = 0; i < templates.Count; i++) //Проходимся по классам
                    {
                        Attribute = new List<(string, string)>();
                        var template = templates[i];
                        className = template.ChildNodes[1].InnerText; //Сохраняем имена классов
                        var templateInside = template.ChildNodes;
                        var check = templateInside.Item(7);
                        if (check == null)
                        {
                            return;
                        }
                        var slots = templateInside.Item(7).ChildNodes;
                        for (int j = 0; j < slots.Count; j++) //Сохраняем переменные класса
                        {
                            var slot = slots[j];
                            attr = slot.ChildNodes[0].InnerText;
                            typeText = slot.ChildNodes[4].InnerText;
                            Attribute.Add((attr, typeText));
                        }
                        ekb.Add(new EKB(className, Attribute));
                    }
                    var grules = know.Item(8).ChildNodes;
                    for (int i = 0; i < grules.Count; i++) //Сохраняем связи
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