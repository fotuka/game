using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace game
{
    public partial class Form1 : Form
    {
        Cell[] cells = new Cell[100];
        string lastCellValue = "";
        int ticksSpeed = 3000;
        int score = 0;
        int found = 0;

        public Form1()
        {
            InitializeComponent();
            FillCells();           
            dataGridView1.Enabled = false;
            UpdateScore();
        }

        private void TimerEventProcessor(Object myObject,
                                            EventArgs myEventArgs)
        {
            myTimer.Stop();

            MessageBox.Show("");
        }

        public void UpdateFound()
        {
            label14.Text = "Found: " + this.found + " / 100";
        }

        public void UpdateScore()
        {
            labelScore.Text = this.score.ToString();
        }

        public void FillCells()
        {
            char ch;
            var rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                ch = System.Convert.ToChar(rand.Next(1072, 1103));
                cells[i] = new Cell(false, ch);
            }
        }

        public void FillDataGrid()
        {
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add();
            }            
        }

        public int TransformToHundred(int x, int y)
        {
            return x * 10 + y;            
        }

        public (int x, int y) TransformToCoords(int value)
        {
            int x = value / 10;
            int y = value % 10;
            return (x, y);
        }

        public void ShowAllCellsWithValue(string value)
        {
            int count = 0;
            foreach (Cell cell in this.cells)
            {
                if (cell.value.ToString() == value)
                {
                    dataGridView1.Rows[TransformToCoords(count).x].Cells[TransformToCoords(count).y].Value = cell.value;
                }
                count++;
            }
        }

        public void LoadFromDataGrid()
        {
            int count = 0;
            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    dataGridView1.Rows[row].Cells[column].Value = cells[count].value;
                    //dataGridView1.Rows[row].Cells[column]
                    count++;
                }
            }
        }

        private void HideAll(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            int count = 0;
            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    if (!this.cells[count].isChecked)
                        dataGridView1.Rows[row].Cells[column].Value = null;                    
                    count++;
                }
            }
        }

        public bool IsEveryCellChecked(string value)
        {
            int check = 0;
            int amount = 0;
            foreach (Cell cell in this.cells)
            {
                if (cell.value.ToString() == value)
                {
                    if (cell.isChecked)
                        check++;
                    amount++;
                }
            }
            return check == amount;
        }

        private void TurnOffAll(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    dataGridView1[column, row].Style.BackColor = Color.White;
                }
            }
        }

        public void FireAllCells(string cellValue)
        {            
            int amount = 0;
            foreach (Cell cell in this.cells)
            {
                if (cell.value.ToString() == cellValue)
                {
                    (int x, int y) index = TransformToCoords(amount);
                    dataGridView1[index.y, index.x].Style.BackColor = Color.Coral;
                    myTimer.Tick += new EventHandler(TurnOffAll);
                    myTimer.Interval = 350;
                    myTimer.Start();
                }
                amount++;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1[e.ColumnIndex, e.RowIndex].Selected = false;
            int index = TransformToHundred(e.RowIndex, e.ColumnIndex);
            if (this.cells[index].isChecked)
            {

            }
            else
            {
                if (this.lastCellValue == "")
                {                                        
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cells[index].value;
                    this.lastCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    ShowAllCellsWithValue(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    myTimer.Tick += new EventHandler(HideAll);
                    myTimer.Interval = ticksSpeed;
                    myTimer.Start();
                }
                else if (this.lastCellValue == this.cells[index].value.ToString())
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.cells[index].value;
                    this.cells[index].isChecked = true;
                    if (IsEveryCellChecked(this.cells[index].value.ToString()))
                    {
                        FireAllCells(this.cells[index].value.ToString());
                        this.lastCellValue = "";
                    }
                    this.score = this.score + 100000 / ticksSpeed;
                    this.found++;
                    UpdateFound();
                    UpdateScore();
                    if (this.found == 100)
                        MessageBox.Show("You won!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    this.score = this.score - 100000 / 2125;
                    UpdateScore();
                }                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowAllCellsWithValue(this.lastCellValue);
            myTimer.Tick += new EventHandler(HideAll);
            myTimer.Interval = ticksSpeed;
            myTimer.Start();
            this.score = this.score - 100000 / 2125;
            UpdateScore();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Enabled)
            {
                FillCells();
                this.lastCellValue = "";
                dataGridView1.Rows.Clear();
                FillDataGrid();
                this.score = 0;
                UpdateScore();
                this.found = 0;
                UpdateFound();
                dataGridView1[0, 0].Selected = false;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            FillDataGrid();
            dataGridView1.Enabled = true;
            dataGridView1.ClearSelection();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.ticksSpeed = trackBar1.Value * 250;
            label12.Text = "Speed: " + trackBar1.Value;
        }
    }

    public class Cell
    {
        public Cell(bool check, char value)
        {
            this.isChecked = check;
            this.value = value;
        }

        public bool isChecked { get; set; }
        public char value { get; set; }
    }
}
