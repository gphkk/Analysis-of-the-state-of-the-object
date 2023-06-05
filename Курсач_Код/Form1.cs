using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms.DataVisualization.Charting;


namespace Курсач_Код
{
    public partial class Form1 : Form
    {
        private SQLiteConnection SQLiteConn;
        private DataTable dTable;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Enabled = false;
            textBox1.Enabled = false;
            Show_DB.Enabled = false;
            textBox2.Enabled = false;
            AddLine.Enabled = false;
            DeleteLine.Enabled = false;
            tabPage2.Enabled = false;
            tabPage3.Enabled = false;
            tabPage6.Enabled = false;
            button1.Enabled = false;
            chart1.Visible = false;
            chart2.Visible = false;
            chart3.Visible = false;
            chart4.Visible = false;
            chart5.Visible = false;
            button10.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
            this.WindowState = FormWindowState.Maximized;
            
        }


        private bool OpenDBFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Текстовые файлы (*.sqlite)|*.sqlite|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version=3;");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;

                return true;
            }
            else return false;
        }

        private void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
        }

        public void ShowTable(string SQLQuery)
        {
            tabPage2.Enabled = true;
            dTable.Clear();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dTable);

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);
            }
            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }
        }


        private string SQL_AllTable()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] order by 1";
        }

        private void OpenDB_Click(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                GetTableNames();
                comboBox1.Enabled = true;
                Show_DB.Enabled = true;
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = this.ClientSize.Width - 21 - groupBox1.Width;

            dataGridView2.Width = (this.ClientSize.Width - 30 - groupBox2.Width) / 3 ;
            dGV_prognoz.Width = dataGridView2.Width;
            dGV_Stabil.Width = dataGridView2.Width;

            dataGridView2.Left = groupBox2.Right + 5;
            dGV_prognoz.Left = dataGridView2.Right + 5;
            dGV_Stabil.Left = dGV_prognoz.Right + 5;

            dGV_Faz2.Width = (this.ClientSize.Width - 30 - groupBox4.Width) / 3;
            dGV_Prg2.Width = dGV_Faz2.Width;
            dGV_Stabil2.Width = dGV_Faz2.Width;

            dGV_Faz2.Left = groupBox4.Right + 5;
            dGV_Prg2.Left = dGV_Faz2.Right + 5;
            dGV_Stabil2.Left = dGV_Prg2.Right + 5;

        }

        public Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);       // Создает поток, резервным хранилищем которого является память.
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);                    // Инкапсулирует точечный рисунок
            return image;
        }




        private void Show_DB_Click(object sender, EventArgs e)
        {
            if (Convert.ToString( comboBox1.SelectedItem) == "Images")
            {
                MessageBox.Show("Недопустимая таблица! Выберите другую))", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button1.Enabled = true;
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ShowTable(SQL_AllTable());
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            AddLine.Enabled = true;
            DeleteLine.Enabled = true;

            string selectImage = "SELECT data FROM `Images` WHERE `id`= 1";
            SQLiteCommand cmd = new SQLiteCommand(selectImage, SQLiteConn);

            SQLiteDataReader SQLdr = cmd.ExecuteReader();

            while (SQLdr.Read())
            {
                byte[] a = (System.Byte[])SQLdr[0];
                pictureBox1.Image = ByteToImage(a);
                pictureBox2.Image = ByteToImage(a);
                pictureBox3.Image = ByteToImage(a);
                pictureBox4.Image = ByteToImage(a);
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            dataGridView1.AllowUserToAddRows = false;

            for (int i = 1; i < dataGridView1.ColumnCount; i++)
            {
                listBox1.Items.Add(i);
                listBox3.Items.Add(i);
            }

        }

        private void AddLine_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
            
            double delta = 0;
            for (int i = 0; i < ((dTable.Rows.Count) - 1); i++)
            {
                if (Math.Abs(Convert.ToDouble(dTable.Rows[i][1]) - Convert.ToDouble(dTable.Rows[i + 1][1])) > delta)
                {
                    delta = Math.Abs(Convert.ToDouble(dTable.Rows[i][1]) - Convert.ToDouble(dTable.Rows[i + 1][1]));
                }
            }

            Random rnd = new Random();
            double value = -delta + rnd.NextDouble() * (delta - (-delta));
            value = value * 0.2;
            int epoha = dataGridView1.Rows.Count - 1;
            int point = dataGridView1.Columns.Count;
            dataGridView1.Rows[epoha].Cells[0].Value = epoha;


            for (int i = dTable.Rows.Count - 1; i < dTable.Rows.Count; i++)
            {
                for (int j = 1; j < dataGridView1.ColumnCount; j++)
                {
                    double cell = Math.Round(Convert.ToDouble(dTable.Rows[i][j]) + value, 4);
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[j].Value = cell;

                }
            }
        }

        public double [,] MassivBlock(int col)
        {
            double[,] arrBlock;
            arrBlock = new double[dataGridView1.RowCount, col];

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    arrBlock[i, j] = Convert.ToDouble(dGV_Block.Rows[i].Cells[j].Value.ToString());
                }
            }
            return arrBlock;
        }

        public double[,] MassivFromDB()
        {
            double[,] arrDB;
            arrDB = new double[dataGridView1.RowCount, dataGridView1.ColumnCount];

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 1; j < dataGridView1.Columns.Count; j++)
                {
                    arrDB[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString());
                }
            }
            return arrDB;
        }

        public double[,] MassivFromDBplus(double[,] DB_peredacha, double E,  int col)
        {
            double[,] arrPlus;
            arrPlus = new double[dataGridView1.RowCount, col];

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    arrPlus[i,j] = DB_peredacha[i,j] + E;
                }
            }
            return arrPlus;
        }

        public double[,] MassivFromDBminus(double[,] DB_peredacha, double E, int col)
        {
            double[,] arrMinus;
            arrMinus = new double[dataGridView1.RowCount, col];
            
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    arrMinus[i, j] = DB_peredacha[i, j] - E;
                }
            }
            return arrMinus;
        }

        public double[] MassivM(double[,] m_peredacha,  int col)
        {
            double[] arrMu;
            arrMu = new double[dataGridView1.RowCount];

            double M0 = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double Mi = 0;
                for (int j = 1; j < col; j++)
                {
                    Mi = Mi + Math.Pow(Convert.ToDouble(m_peredacha[i, j]), 2); 
                }
                Mi = Math.Sqrt(Mi);
                if (i == 0)
                    M0 = Mi;
                
                arrMu[i] = Mi;
                
            }
            return arrMu;

        }
        public double [] MassivAlpha(double[,] a_peredacha, int col)
        {
            double[] arrAlpha;
            arrAlpha = new double[dataGridView1.RowCount];
            
            double M0 = 0;
            
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                double sumProiz = 0;
                double Mi = 0;
                for (int j = 1; j < col; j++)
                {
                    Mi = Mi + Math.Pow(Convert.ToDouble(a_peredacha[i, j]), 2);
                }
                Mi = Math.Sqrt(Mi);
                if (i == 0)
                    M0 = Mi;
                for (int j = 1; j < col; j++)
                {
                    sumProiz = sumProiz + Convert.ToDouble(a_peredacha[0,j]) * Convert.ToDouble(a_peredacha[i,j]);
                }
                double Ai = sumProiz / (Mi * M0);
                if (Ai > 1)     
                    Ai = 1;
                double result = Math.Acos(Ai) * 3600 * 180 / Math.PI;
                result = ((int)(result * Math.Pow(10, 1))) / Math.Pow(10, 2);
                arrAlpha[i] = result;
                

            }
            return arrAlpha;
        }

        public double[] CountPrognoz(double[] p_peredacha, double A)
        {
            double[] arrPrg;
            arrPrg = new double[p_peredacha.Length + 1];
           
            double sum1 = 0;                                     // СУММА ПЕРЕДАВАЕМОГО МАССИВА
            for (int i = 0; i < p_peredacha.Length; i++)
            {
                sum1 = sum1 + p_peredacha[i];
                
            }
            double sred1 = sum1 / p_peredacha.Length;               // СРЕДНЕЕ ЗНАЧЕНИЕ ПЕРЕДАВАЕМОГО МАССИВА

            arrPrg[0] = A * p_peredacha[0] + (1 - A) * sred1;
            double sum2 = arrPrg[0];
            for (int i = 1; i < p_peredacha.Length; i++)
            {
                arrPrg[i] = p_peredacha[i] * A + (1 - A) * arrPrg[i - 1];
                sum2 = sum2 + arrPrg[i];                                // СУММА МАССИВА ПРОГНОЗА
            }
           
            double sred2 = sum2 / p_peredacha.Length;                       // СРЕДНЕЕ ЗНАЧЕНИЕ МАССИВА ПРОГНОЗА
            arrPrg[p_peredacha.Length] = A * sred2 + (1 - A) * arrPrg[p_peredacha.Length-1];     // ПРОГНОЗ ОКОНЧАТЕЛЬНЫЙ
            
            return arrPrg;
        }

        public double[] Stability_R(double[] stb_plus, double[] stb_min, double[] prgPlus, double[] prgMinus)
        {
            double[] R;
            R = new double[stb_plus.Length + 1];
            
            for (int i = 0; i < stb_plus.Length; i++)
            {
                R[i] = Math.Abs(stb_plus[i]-stb_min[i])/2;
            }
            
            double cell = Math.Abs(prgPlus[stb_plus.Length] - prgMinus[prgPlus.Length - 1])/2;
            
            R[stb_plus.Length] = cell;           
            return R;
        }

        public double[] Stability_L(double[] stb, double[] prg)
        {
            double[] L;
            L = new double[stb.Length + 1];
            for (int i = 0; i < stb.Length; i++)
            {
                L[i] = Math.Abs(stb[i] - stb[0]);
            }
            L[stb.Length] = Math.Abs(prg[stb.Length] - stb[0]);
            return L;
        }

        public string[] Stability_Normal(double[] R, double [] L)
        {
            string[] stb;
            stb = new string[R.Length];

            for(int i = 0; i < R.Length; i++)
            {
                if (R[i] > L[i])
                {
                    stb[i] = "Нормальное";
                }
                else if (R[i] < L[i])
                {
                    stb[i] = "Аварийное";
                }
                else if (R[i] == L[i])
                {
                    stb[i] = "Предаварийное";
                }
            }
            return stb;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
            {
                MessageBox.Show("Выберите значения параметров сглаживания и точности!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Convert.ToDouble(textBox1.Text) < 0 || Convert.ToDouble(textBox1.Text) > 1) 
            {
                MessageBox.Show("Коэффициент сглаживания должен находиться в промежутке от 0 до 1!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Convert.ToDouble(textBox2.Text) < 0 )
            {
                MessageBox.Show("Точность Е должна быть больше 0!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double Asg = Convert.ToDouble(textBox1.Text);
            double E = Convert.ToDouble(textBox2.Text);
            toolStripStatusLabel1.Text = "A = " + textBox1.Text;
            toolStripStatusLabel2.Text = "E = " + textBox2.Text;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
            {
                MessageBox.Show("Выберите значения параметров сглаживания и точности!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox4.Checked && !checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Выберите какие графики необходимо построить!", "Ошибка",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox4.Checked)
            {
                if ((checkBox1.Checked) && (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton7.Checked))
                    chart1.Visible = true;
                else chart1.Visible = false;
                if (checkBox2.Checked)
                    chart2.Visible = true;
                else chart2.Visible = false;    
                    
            }

            if ((checkBox1.Checked) && (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked && !radioButton7.Checked))
            {
                MessageBox.Show("Выберите для каких значений построить график фазовых координат!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            dataGridView2.RowCount = dataGridView1.RowCount;
            dGV_prognoz.RowCount = dataGridView1.RowCount + 1;
            dGV_Stabil.RowCount = dataGridView1.RowCount + 1;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dGV_prognoz.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
                dGV_Stabil.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
            }
            dGV_prognoz.Rows[dGV_prognoz.RowCount - 1].Cells[0].Value = "Прогноз";
            dGV_Stabil.Rows[dGV_Stabil.RowCount - 1].Cells[0].Value = "Прогноз";
            
            peredacha();
            if (checkBox4.Checked)
            {
                FactCreateSerie();
            }
            else
            {
                chart1.Visible = false;
                chart2.Visible = false;
            }
            tabPage3.Enabled = true;
        }

        public void FactCreateSerie()
        {
            double Asg = Convert.ToDouble(textBox1.Text);
            double E = Convert.ToDouble(textBox2.Text);
            //int rowBD = dataGridView1.RowCount;
            int colDB = dataGridView1.ColumnCount;
            //int rowB = dGV_Block.RowCount;
            int colB = dGV_Block.ColumnCount;

            double[,] nachalo = MassivFromDB();
            double[,] nach_block = MassivBlock(colB);
            double[,] DBplus = MassivFromDBplus(nachalo, E, colDB);
            double[,] DBminus = MassivFromDBminus(nachalo, E, colDB);

            double[] Mstart = MassivM(nachalo, colDB);
            double[] Mplus = MassivM(DBplus,  colDB);
            double[] Mminus = MassivM(DBminus, colDB);

            double[] Astart = MassivAlpha(nachalo,  colDB);
            double[] Aplus = MassivAlpha(DBplus,  colDB);
            double[] Aminus = MassivAlpha(DBminus,  colDB);

            double[] PrgStartM = CountPrognoz(Mstart, Asg);
            double[] PrgStartA = CountPrognoz(Astart, Asg);
            double[] PrgPlusM = CountPrognoz(Mplus, Asg);
            double[] PrgPlusA = CountPrognoz(Aplus, Asg);
            double[] PrgMinusM = CountPrognoz(Mminus, Asg);
            double[] PrgMinusA = CountPrognoz(Aminus, Asg);

            //chart1.Series[realName].Label = "#INDEX";
            //chart1.Series[realName].ToolTip = "M =#VALX, A=#VALY";

            //chart1.ChartAreas[0].AxisY.Maximum = 16.5;

            double min = double.MaxValue;
            double min1 = double.MaxValue;
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();            // с 1-м чартом всё ок!!
            chart1.Series[3].Points.Clear();
            chart1.Series[4].Points.Clear();
            chart1.Series[5].Points.Clear();

            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart2.Series[2].Points.Clear();
            chart2.Series[3].Points.Clear();

            for (int i = 0; i < Mstart.Length; i++)
            {
                double x1 = Mstart[i];
                double y1 = Astart[i]; 

                double x2 = Mplus[i];
                double y2 = Aplus[i];

                double x3 = Mminus[i];
                double y3 = Aminus[i];
                if (radioButton1.Checked)
                chart1.Series[0].Points.AddXY(x1, y1);
                if (radioButton2.Checked)
                chart1.Series[1].Points.AddXY(x2, y2);
                if (radioButton3.Checked)
                chart1.Series[2].Points.AddXY(x3, y3);
                if (radioButton7.Checked)
                {
                    chart1.Series[0].Points.AddXY(x1, y1);
                    chart1.Series[1].Points.AddXY(x2, y2);
                    chart1.Series[2].Points.AddXY(x3, y3);
                }
                if (Aminus[i] < min1)
                    min1 = Aminus[i];
            }
            for (int i = 0; i < PrgStartM.Length; i++)
            {
                double x1 = PrgStartM[i];
                double y1 = PrgStartA[i];

                double x2 = PrgPlusM[i];
                double y2 = PrgPlusA[i];

                double x3 = PrgMinusM[i];
                double y3 = PrgMinusA[i];
                if (radioButton1.Checked)
                    chart1.Series[3].Points.AddXY(x1, y1);
                if (radioButton2.Checked)
                    chart1.Series[4].Points.AddXY(x2, y2);
                if (radioButton3.Checked)
                    chart1.Series[5].Points.AddXY(x3, y3);
                if (radioButton7.Checked)
                {
                    chart1.Series[3].Points.AddXY(x1, y1);
                    chart1.Series[4].Points.AddXY(x2, y2);
                    chart1.Series[5].Points.AddXY(x3, y3);
                }
            }

            

            for (int i = 0; i < Mstart.Length; i++)
            {
                double x = i;

                double y1 = Mstart[i];
                double y2 = Mplus[i];
                double y3 = Mminus[i];
                chart2.Series[0].Points.AddXY(x, y1);
                chart2.Series[1].Points.AddXY(x, y2);
                chart2.Series[2].Points.AddXY(x, y3);
                if (Mminus[i] < min)
                    min = Mminus[i];
            }
            for (int i = 0; i < PrgStartM.Length; i++)
            {
                double x = i;
                double y0 = PrgStartM[i];
                
                chart2.Series[3].Points.AddXY(x, y0);
            }
            chart2.ChartAreas[0].AxisY.Minimum = min - 0.01;
            chart1.ChartAreas[0].AxisY.Minimum = min1 - 0.01;

        }

        List<List<object>> list_block = new List<List<object>>();       // Вложенный список

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите для какого блока будут выбираться марки!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
             
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                var item = listBox1.SelectedItems[i];
                listBox2.Items.Add(item);
                
            }
            int count = listBox1.SelectedItems.Count;
            for (int i = 0; i < count; i++)
            {
                var rmv = listBox1.SelectedItems[0];
                listBox1.Items.Remove(rmv);
                //listBox1.Items.Remove(item);
            }
            listBox1.ClearSelected();

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {

                if (comboBox2.SelectedIndex == i)
                {
                    list_block[i].Clear();
                    for (int j = 0; j < listBox2.Items.Count; j++)
                    {
                        list_block[i].Add(listBox2.Items[j]);
                    }
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.SelectedItems.Count; i++)  // работает
            {
                var item = listBox2.SelectedItems[i];
                listBox1.Items.Add(item);                   // перенос в общий лист
                
            }
            int count = listBox2.SelectedItems.Count;
            for (int i = 0; i < list_block.Count; i++)
            {
                
                if (comboBox2.SelectedIndex == i)
                {
                    int count1 = listBox2.SelectedItems.Count;
                    for (int j = 0; j < count1; j++)
                    {
                        var rmv = listBox2.SelectedItems[0];
                        listBox2.Items.Remove(rmv);
                        list_block[i].Remove(rmv);
                    }
                        
                }
            }

            listBox2.ClearSelected();
        }

        private void ClearAll_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            for (int i = 1; i < dataGridView1.ColumnCount; i++)
            {
                listBox1.Items.Add(i);
            }
            
            for (int i = 0; i < list_block.Count; i++)
            {
                list_block[i].Clear();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int limit = (dataGridView1.ColumnCount - 1)/ 2;

            if (textBox3.Text == "")
            {
                MessageBox.Show("Укажите количество блоков", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((textBox3.Text == "1") || (Convert.ToInt32(textBox3.Text) > limit))
            {
                MessageBox.Show($"Минимальное количество блоков - 2\nМаксимальное количество блоков - {limit}", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            comboBox2.Items.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            for (int i = 1; i < dataGridView1.ColumnCount; i++)
            {
                //listBox1.Items.Add(i);
                listBox1.Items.Add(i);
            }

            for (int i = 1040; i < (1040 + Convert.ToInt32( textBox3.Text)); i++)
            {
                comboBox2.Items.Add((char)i);
            }
            list_block.Clear();
            for (int i = 0; i < comboBox2.Items.Count; i++)
                list_block.Add(new List<object>());

            
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            for (int i=0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.SelectedIndex == i)
                {
                    for (int j = 0; j < list_block[i].Count; j++)
                    {
                        //listBox2.Items.Add(list_block[i]);
                        listBox2.Items.Add(list_block[i][j]);
                    }
                }
            }
            Show_Block_Click(sender, e);
            toolStripStatusLabel3.Text = "Выбран блок: " + comboBox2.SelectedItem.ToString();
        }

        private void Show_Block_Click(object sender, EventArgs e)
        {
            int num_blocks = Convert.ToInt32(textBox3.Text);
            int num_points = listBox2.Items.Count;

            object[,] arr2_lvl = new object[num_blocks, num_points];
            object[,] arr_block = new object[dataGridView1.RowCount, num_points];
            for (int i = 0; i < num_blocks; i++)
            {
                dGV_Block.ColumnCount = num_points + 1;
                dGV_Block.Columns[0].HeaderText = "Эпоха";
                dGV_Block.RowCount = dataGridView1.RowCount;
                for (int l = 0; l < dGV_Block.RowCount; l++)
                {
                    dGV_Block.Rows[l].Cells[0].Value = l;
                }

                for (int j = 0; j < num_points; j++)
                {
                    //arr2_lvl[i, j] = listBox2.Items[j];
                    dGV_Block.Columns[j + 1].HeaderText = listBox2.Items[j].ToString();
                    
                }
            }

            for (int m = 0; m < num_points; m++)
            {
                for(int k = 0; k < dGV_Block.RowCount; k++)
                {
                    for(int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (listBox2.Items[m].ToString() == dataGridView1.Columns[j].HeaderText)
                        {
                            dGV_Block.Rows[k].Cells[m+1].Value = dataGridView1.Rows[k].Cells[j].Value;
                        }
                    }
                }
            }
           
            

        }

        private void button6_Click(object sender, EventArgs e)  // Кнопка "Вычислить" 2 уровня
        {
            for (int i = 0; i < list_block.Count - 1; i++)
            {
                if (list_block[i].Count != list_block[i+1].Count)
                {
                    MessageBox.Show("В блоках должно быть одинаковое количество марок!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
            {
                MessageBox.Show("Выберите значения параметров сглаживания и точности!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox8.Checked && !checkBox5.Checked && !checkBox6.Checked)
            {
                MessageBox.Show("Выберите какие графики необходимо построить!", "Ошибка",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((checkBox5.Checked) && (!radioButton4.Checked && !radioButton5.Checked && !radioButton6.Checked && !radioButton8.Checked))
            {
                MessageBox.Show("Выберите для каких значений построить график фазовых координат!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox8.Checked)
            {
                if ((checkBox5.Checked) && (radioButton4.Checked || radioButton5.Checked || radioButton6.Checked || radioButton8.Checked))
                    chart4.Visible = true;
                else chart4.Visible = false;
                if (checkBox6.Checked)
                    chart5.Visible = true;
                else chart5.Visible = false;

            }
            

            dGV_Faz2.RowCount = dataGridView1.RowCount;
            dGV_Prg2.RowCount = dataGridView1.RowCount + 1;
            dGV_Stabil2.RowCount = dataGridView1.RowCount + 1;
            for (int i = 0; i < dGV_Faz2.RowCount; i++)
            {
                dGV_Faz2.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dGV_Prg2.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
                dGV_Stabil2.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value;
            }
            dGV_Prg2.Rows[dGV_Prg2.RowCount - 1].Cells[0].Value = "Прогноз";
            dGV_Stabil2.Rows[dGV_Stabil2.RowCount - 1].Cells[0].Value = "Прогноз";

            peredacha();
            if (checkBox8.Checked)
            {
                CreateSerie_2lvl();
            }
            else
            {
                chart4.Visible = false;
                chart5.Visible = false;
            }
            
            tabPage6.Enabled = true;
        }

        

        public void peredacha()
        {
            double E = Convert.ToDouble(textBox2.Text);
            double Asg = Convert.ToDouble(textBox1.Text);

            int colDB = dataGridView1.ColumnCount;
            int colB = dGV_Block.ColumnCount;
            
            //double[,] array;
            //array = new double[dataGridView1.RowCount, dataGridView1.ColumnCount];
            
            double[,] nachalo = MassivFromDB();
            double[,] DBplus = MassivFromDBplus(nachalo, E, colDB);
            double[,] DBminus = MassivFromDBminus(nachalo, E,  colDB);

            double[] Mstart = MassivM(nachalo, colDB);
            double[] Mplus = MassivM(DBplus, colDB);
            double[] Mminus = MassivM(DBminus, colDB);

            double[] Astart = MassivAlpha(nachalo, colDB);
            double[] Aplus = MassivAlpha(DBplus, colDB);
            double[] Aminus = MassivAlpha(DBminus, colDB);

            double[] PrgStartM = CountPrognoz(Mstart, Asg);
            double[] PrgStartA = CountPrognoz(Astart, Asg);
            double[] PrgPlusM = CountPrognoz(Mplus, Asg);
            double[] PrgPlusA = CountPrognoz(Aplus, Asg);
            double[] PrgMinusM = CountPrognoz(Mminus, Asg);
            double[] PrgMinusA = CountPrognoz(Aminus, Asg);

            double[] Rstab = Stability_R(Mplus, Mminus, PrgPlusM, PrgMinusM);
            double[] Lstab = Stability_L(Mstart, PrgStartM);
            string[] NormStab = Stability_Normal(Rstab, Lstab);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            double[,] nach_block = MassivBlock(colB);
            double[,] Bplus = MassivFromDBplus(nach_block, E, colB);
            double[,] Bminus = MassivFromDBminus(nach_block, E, colB);

            double[] Mstart2 = MassivM(nach_block, colB);
            double[] Mplus2 = MassivM(Bplus, colB);
            double[] Mminus2 = MassivM(Bminus, colB);

            double[] Astart2 = MassivAlpha(nach_block, colB);
            double[] Aplus2 = MassivAlpha(Bplus, colB);
            double[] Aminus2 = MassivAlpha(Bminus, colB);

            double[] PrgStartM2 = CountPrognoz(Mstart2, Asg);
            double[] PrgStartA2 = CountPrognoz(Astart2, Asg);
            double[] PrgPlusM2 = CountPrognoz(Mplus2, Asg);
            double[] PrgPlusA2 = CountPrognoz(Aplus2, Asg);
            double[] PrgMinusM2 = CountPrognoz(Mminus2, Asg);
            double[] PrgMinusA2 = CountPrognoz(Aminus2, Asg);

            double[] Lstab_block = Stability_L(Mstart2, PrgStartM2);
            double[] Rstab_block = Stability_R(Mplus2, Mminus2, PrgPlusM2, PrgMinusM2);
            string[] Normstab_block = Stability_Normal(Rstab_block, Lstab_block);

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                Mstart[i] = Math.Round(Mstart[i], 4);
                dataGridView2.Rows[i].Cells[1].Value = Mstart[i];
                Astart[i] = Math.Round(Astart[i], 4);
                dataGridView2.Rows[i].Cells[2].Value = Astart[i];
                Mplus[i] = Math.Round(Mplus[i], 4);
                dataGridView2.Rows[i].Cells[3].Value = Mplus[i];
                Aplus[i] = Math.Round(Aplus[i], 4);
                dataGridView2.Rows[i].Cells[4].Value = Aplus[i];
                Mminus[i] = Math.Round(Mminus[i], 4);
                dataGridView2.Rows[i].Cells[5].Value = Mminus[i];
                Aminus[i] = Math.Round(Aminus[i], 4);
                dataGridView2.Rows[i].Cells[6].Value = Aminus[i];
            }
            for (int i = 0; i < dGV_prognoz.RowCount; i++)
            {
                PrgStartM[i] = Math.Round(PrgStartM[i], 4);
                dGV_prognoz.Rows[i].Cells[1].Value = PrgStartM[i];
                PrgStartA[i] = Math.Round(PrgStartA[i], 4);
                dGV_prognoz.Rows[i].Cells[2].Value = PrgStartA[i];
                PrgPlusM[i] = Math.Round(PrgPlusM[i], 4);
                dGV_prognoz.Rows[i].Cells[3].Value = PrgPlusM[i];
                PrgPlusA[i] = Math.Round(PrgPlusA[i], 4);
                dGV_prognoz.Rows[i].Cells[4].Value = PrgPlusA[i];
                PrgMinusM[i] = Math.Round(PrgMinusM[i], 4);
                dGV_prognoz.Rows[i].Cells[5].Value = PrgMinusM[i];
                PrgMinusA[i] = Math.Round(PrgMinusA[i], 4);
                dGV_prognoz.Rows[i].Cells[6].Value = PrgMinusA[i];

            }
            for (int i = 0; i < dGV_Stabil.RowCount; i++)
            {
                Lstab[i] = Math.Round(Lstab[i], 4);
                dGV_Stabil.Rows[i].Cells[1].Value = Lstab[i];

                Rstab[i] = Math.Round(Rstab[i], 4);
                dGV_Stabil.Rows[i].Cells[2].Value = Rstab[i];

                dGV_Stabil.Rows[i].Cells[3].Value = NormStab[i];
            }

            for (int i = 0; i < dGV_Faz2.RowCount; i++)
            {
                Mstart2[i] = Math.Round(Mstart2[i], 4);
                dGV_Faz2.Rows[i].Cells[1].Value = Mstart2[i];
                Astart2[i] = Math.Round(Astart2[i], 4);
                dGV_Faz2.Rows[i].Cells[2].Value = Astart2[i];
                Mplus2[i] = Math.Round(Mplus2[i], 4);
                dGV_Faz2.Rows[i].Cells[3].Value = Mplus2[i];
                Aplus2[i] = Math.Round(Aplus2[i], 4);
                dGV_Faz2.Rows[i].Cells[4].Value = Aplus2[i];
                Mminus2[i] = Math.Round(Mminus2[i], 4);
                dGV_Faz2.Rows[i].Cells[5].Value = Mminus2[i];
                Aminus2[i] = Math.Round(Aminus2[i], 4);
                dGV_Faz2.Rows[i].Cells[6].Value = Aminus2[i];
            }
            for (int i = 0; i < dGV_Prg2.RowCount; i++)
            {
                PrgStartM2[i] = Math.Round(PrgStartM2[i], 4);
                dGV_Prg2.Rows[i].Cells[1].Value = PrgStartM2[i];
                PrgStartA2[i] = Math.Round(PrgStartA2[i], 4);
                dGV_Prg2.Rows[i].Cells[2].Value = PrgStartA2[i];
                PrgPlusM2[i] = Math.Round(PrgPlusM2[i], 4);
                dGV_Prg2.Rows[i].Cells[3].Value = PrgPlusM2[i];
                PrgPlusA2[i] = Math.Round(PrgPlusA2[i], 4);
                dGV_Prg2.Rows[i].Cells[4].Value = PrgPlusA2[i];
                PrgMinusM2[i] = Math.Round(PrgMinusM2[i], 4);
                dGV_Prg2.Rows[i].Cells[5].Value = PrgMinusM2[i];
                PrgMinusA2[i] = Math.Round(PrgMinusA2[i], 4);
                dGV_Prg2.Rows[i].Cells[6].Value = PrgMinusA2[i];

            }
            for (int i = 0; i < dGV_Stabil2.RowCount; i++)
            {
                Lstab_block[i] = Math.Round(Lstab_block[i], 4); ;
                dGV_Stabil2.Rows[i].Cells[1].Value = Lstab_block[i];

                Rstab_block[i] = Math.Round(Rstab_block[i], 4);
                dGV_Stabil2.Rows[i].Cells[2].Value = Rstab_block[i];

                dGV_Stabil2.Rows[i].Cells[3].Value = Normstab_block[i];
            }
        }

        List<List<object>> list_four = new List<List<object>>();

        private void button7_Click(object sender, EventArgs e)
        {

            int count = listBox3.SelectedItems.Count;

            for (int i = 0; i < listBox3.SelectedItems.Count; i++)
            {
                var item = listBox3.SelectedItems[i];
                listBox4.Items.Add(item);

            }
            
            for (int i = 0; i < count; i++)
            {
                var rmv = listBox3.SelectedItems[0];
                listBox3.Items.Remove(rmv);
            }
            listBox3.ClearSelected();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.SelectedItems.Count; i++)  // работает
            {
                var item = listBox4.SelectedItems[i];
                listBox3.Items.Add(item);                   // перенос в общий лист

            }
            int count = listBox4.SelectedItems.Count;
            for (int i = 0; i < count; i++)
            {
                var rmv1 = listBox4.SelectedItems[0];
                listBox4.Items.Remove(rmv1);
            }
            listBox4.ClearSelected();
        }

        public void CreateSerie_2lvl()
        {
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            chart4.Series[2].Points.Clear();            
            chart4.Series[3].Points.Clear();
            chart4.Series[4].Points.Clear();
            chart4.Series[5].Points.Clear();

            chart5.Series[0].Points.Clear();
            chart5.Series[1].Points.Clear();
            chart5.Series[2].Points.Clear();
            chart5.Series[3].Points.Clear();

            

            double Asg = Convert.ToDouble(textBox1.Text);
            double E = Convert.ToDouble(textBox2.Text);
            int colB = dGV_Block.ColumnCount;

            double[,] nach_block = MassivBlock(colB);
            double[,] Bplus = MassivFromDBplus(nach_block, E, colB);
            double[,] Bminus = MassivFromDBminus(nach_block, E, colB);

            double[] Mstart2 = MassivM(nach_block, colB);
            double[] Mplus2 = MassivM(Bplus, colB);
            double[] Mminus2 = MassivM(Bminus, colB);

            double[] Astart2 = MassivAlpha(nach_block, colB);
            double[] Aplus2 = MassivAlpha(Bplus, colB);
            double[] Aminus2 = MassivAlpha(Bminus, colB);

            double[] PrgStartM2 = CountPrognoz(Mstart2, Asg);
            double[] PrgStartA2 = CountPrognoz(Astart2, Asg);
            double[] PrgPlusM2 = CountPrognoz(Mplus2, Asg);
            double[] PrgPlusA2 = CountPrognoz(Aplus2, Asg);
            double[] PrgMinusM2 = CountPrognoz(Mminus2, Asg);
            double[] PrgMinusA2 = CountPrognoz(Aminus2, Asg);

            double min1 = double.MaxValue;
            for (int i = 0; i < Mstart2.Length; i++)
            {
                double x1 = Mstart2[i];
                double y1 = Astart2[i];

                double x2 = Mplus2[i];
                double y2 = Aplus2[i];

                double x3 = Mminus2[i];
                double y3 = Aminus2[i];
                if (radioButton4.Checked)
                    chart4.Series[0].Points.AddXY(x1, y1);
                if (radioButton5.Checked)
                    chart4.Series[1].Points.AddXY(x2, y2);
                if (radioButton6.Checked)
                    chart4.Series[2].Points.AddXY(x3, y3);
                if (radioButton8.Checked)
                {
                    chart4.Series[0].Points.AddXY(x1, y1);
                    chart4.Series[1].Points.AddXY(x2, y2);
                    chart4.Series[2].Points.AddXY(x3, y3);
                }
                if (Aminus2[i] < min1)
                    min1 = Aminus2[i];
            }
            
            for (int i = 0; i < PrgStartM2.Length; i++)
            {
                double x1 = PrgStartM2[i];
                double y1 = PrgStartA2[i];

                double x2 = PrgPlusM2[i];
                double y2 = PrgPlusA2[i];

                double x3 = PrgMinusM2[i];
                double y3 = PrgMinusA2[i];
                if (radioButton4.Checked)
                    chart4.Series[3].Points.AddXY(x1, y1);
                if (radioButton5.Checked)
                    chart4.Series[4].Points.AddXY(x2, y2);
                if (radioButton6.Checked)
                    chart4.Series[5].Points.AddXY(x3, y3);
                if (radioButton8.Checked)
                {
                    chart4.Series[3].Points.AddXY(x1, y1);
                    chart4.Series[4].Points.AddXY(x2, y2);
                    chart4.Series[5].Points.AddXY(x3, y3);
                }
                
            }
            chart4.ChartAreas[0].AxisY.Minimum = min1 - 0.01;
            chart4.Series[0].Color = Color.Coral;
            chart4.Series[3].Color = Color.Chartreuse;

            chart4.Series[1].Color = Color.DarkSlateBlue;
            chart4.Series[4].Color = Color.MediumTurquoise;

            chart4.Series[2].Color = Color.MediumVioletRed;
            chart4.Series[5].Color = Color.Gold;



            double min = double.MaxValue;
            for (int i = 0; i < Mstart2.Length; i++)
            {
                double x = i;

                double y1 = Mstart2[i];
                double y2 = Mplus2[i];
                double y3 = Mminus2[i];
                chart5.Series[0].Points.AddXY(x, y1);
                chart5.Series[1].Points.AddXY(x, y2);
                chart5.Series[2].Points.AddXY(x, y3);
                if (Mminus2[i] < min)
                    min = Mminus2[i];

            }
            for (int i = 0; i < PrgStartM2.Length; i++)
            {
                double x = i;
                double y0 = PrgStartM2[i];

                chart5.Series[3].Points.AddXY(x, y0);
            }
            chart5.ChartAreas[0].AxisY.Minimum = min - 0.001;
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox4.Items.Count == 0)
            {
                MessageBox.Show("Выберите для каких марок построить график!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button10.Enabled = true;
            int num_points = listBox4.Items.Count;
                dGV_4lvl.ColumnCount = num_points + 1;
                dGV_4lvl.Columns[0].HeaderText = "Эпоха";
                dGV_4lvl.RowCount = dataGridView1.RowCount;
                for (int l = 0; l < dGV_4lvl.RowCount; l++)
                {
                    dGV_4lvl.Rows[l].Cells[0].Value = l;
                }

                for (int j = 0; j < num_points; j++)
                {
                    dGV_4lvl.Columns[j + 1].HeaderText = listBox4.Items[j].ToString();

                }
            

            for (int m = 0; m < num_points; m++)
            {
                for (int k = 0; k < dGV_4lvl.RowCount; k++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (listBox4.Items[m].ToString() == dataGridView1.Columns[j].HeaderText)
                        {
                            dGV_4lvl.Rows[k].Cells[m + 1].Value = dataGridView1.Rows[k].Cells[j].Value;
                        }
                    }
                }
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            chart3.Visible = true;
            double[,] arr4 = new double[dGV_4lvl.ColumnCount - 1, dGV_4lvl.RowCount];
            double max = double.MinValue, min = double.MaxValue;
            for (int j = 0; j < dGV_4lvl.ColumnCount - 1; j++)
            {
                for (int i = 0; i < dGV_4lvl.RowCount; i++)
                {
                    arr4[j, i] = Convert.ToDouble( dGV_4lvl.Rows[i].Cells[j+1].Value);
                }
            }

            for (int j = 0; j < dGV_4lvl.ColumnCount - 1; j++)
            {
                for (int i = 0; i < dGV_4lvl.RowCount; i++)
                {
                    if (arr4[j,i] < min)
                        min = arr4[j, i];
                    if (arr4[j, i] > max)
                        max = arr4[j, i];
                }
            }
            chart3.Series.Clear();
            int x;
            chart3.Palette = ChartColorPalette.BrightPastel;
            chart3.ChartAreas[0].AxisY.Minimum = min - 0.01;
            chart3.ChartAreas[0].AxisY.Maximum = max + 0.01;

            for (int i = 0; i < dGV_4lvl.ColumnCount - 1; i++)
            {
                chart3.Series.Add(Convert.ToString(i));
                chart3.Series[i].ChartType = (System.Windows.Forms.DataVisualization.Charting.SeriesChartType)4;
                chart3.Series[i].BorderWidth = 2;
                chart3.Series[i].MarkerStyle = MarkerStyle.Circle;
                chart3.Series[i].Label = "#INDEX";
                chart3.Series[i].ToolTip = "M =#VALX, A=#VALY";
                chart3.Series[i].LegendText = dGV_4lvl.Columns[i+1].HeaderText;

                for (int j = 0; j < dGV_4lvl.RowCount; j++)
                {
                    x = j;
                    
                    chart3.Series[i].Points.AddXY(x, arr4[i,j]);
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back )
            {
                e.Handled = true;
                
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != ','  && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void DeleteLine_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    
}
