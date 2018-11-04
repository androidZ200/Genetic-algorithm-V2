using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace генетический_алгоритм__версия_2_
{
    public partial class FormMain : Form
    {
        int[,] Field = new int[100, 60];
        List<Bot> Bots = new List<Bot>();
        Random rand = new Random();
        Thread t = null;
        bool Stop = true;
        int StopPoint = 0;
        int generation = 0;
        public bool AutoSave = false;
        public string fileName = null;

        void NewField()
        {
            for (int i = 0; i < Field.GetLength(0); i++)
                for (int j = 0; j < Field.GetLength(1); j++)
                    if (i == 0 || j == 0 || Field.GetLength(0) - 1 == i || Field.GetLength(1) - 1 == j) Field[i, j] = -2;
                    else Field[i, j] = -1;
            for (int i = 0; i < Bots.Count; i++)
                while (true)
                {
                    int x = rand.Next(1, Field.GetLength(0) - 1);
                    int y = rand.Next(1, Field.GetLength(1) - 1);
                    if (Field[x, y] == -1)
                    {
                        Bots[i].coordinates = new Point(x, y);
                        Field[x, y] = i;
                        break;
                    }
                }
            for (int i = 0; i < 100; i++)
                while (true)
                {
                    int x = rand.Next(1, Field.GetLength(0) - 1);
                    int y = rand.Next(1, Field.GetLength(1) - 1);
                    if (Field[x, y] == -1)
                    {
                        Field[x, y] = -3;
                        break;
                    }
                }
            for (int i = 0; i < 100; i++)
                while (true)
                {
                    int x = rand.Next(1, Field.GetLength(0) - 1);
                    int y = rand.Next(1, Field.GetLength(1) - 1);
                    if (Field[x, y] == -1)
                    {
                        Field[x, y] = -4;
                        break;
                    }
                }
            for (int i = 0; i < 50; i++)
                while (true)
                {
                    int x = rand.Next(1, Field.GetLength(0) - 1);
                    int y = rand.Next(1, Field.GetLength(1) - 1);
                    if (Field[x, y] == -1)
                    {
                        Field[x, y] = -2;
                        break;
                    }
                }
        }
        void DrawImageField()
        {
            if (ShowCheckBox.Checked)
            {
                Bitmap bmp = new Bitmap(Field.GetLength(0) * 10, Field.GetLength(1) * 10);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Wheat);
                for (int j = 0; j < Field.GetLength(1); j++)
                    for (int i = 0; i < Field.GetLength(0); i++)
                    {
                        Color c = Color.Black;
                        if (Field[i, j] == -1) c = Color.White;
                        else if (Field[i, j] == -2) c = Color.Gray;
                        else if (Field[i, j] == -3) c = Color.DarkGreen;
                        else if (Field[i, j] == -4) c = Color.DarkRed;
                        else if (Field[i, j] >= 0) c = Color.DarkBlue;
                        g.FillRectangle(new SolidBrush(c), i * 10, j * 10, 9, 9);
                    }
                pictureBox1.Image = bmp;
                //bmp.Dispose();
                //g.Dispose();
            }
        }
        void Simulation()
        {
            while (!Stop)
            {
                this.Invoke(new EventHandler(delegate { label1.Text = Convert.ToString(generation); }));
                this.Invoke(new EventHandler(delegate { label2.Text = Convert.ToString(Bots.Count); }));
                this.Invoke(new EventHandler(delegate { label3.Text = Convert.ToString(StopPoint); }));
                StopPoint %= Bots.Count;
                int[] timeField = new int[20];
                Point prevCoor = Bots[StopPoint].coordinates;
                for (int j = Bots[StopPoint].coordinates.Y - 2, j1 = 0, k = 0; j1 < 5; j++, j1++)
                    for (int i = Bots[StopPoint].coordinates.X - 2, i1 = 0; i1 < 5; i++, i1++)
                        if (!((i1 == 0 || i1 == 4) && (j1 == 0 || j1 == 4)) && !(i1 == 2 && j1 == 2))
                        {
                            try { timeField[k] = Field[i, j]; }
                            catch { timeField[k] = -1; }
                            k++;
                        }
                Point p = Bots[StopPoint].MakeAMove(timeField);
                if (p != new Point(0, 0))
                {
                    if (Field[prevCoor.X + p.X, prevCoor.Y + p.Y] >= 0)
                    {
                        int t = Field[prevCoor.X + p.X, prevCoor.Y + p.Y];
                        Bots.RemoveAt(t);
                        if (t < StopPoint) StopPoint--;
                        for (int i = 0; i < Field.GetLength(0); i++)
                            for (int j = 0; j < Field.GetLength(1); j++)
                                if (Field[i, j] > t) Field[i, j]--;
                    }
                    Field[prevCoor.X, prevCoor.Y] = -1;
                    Field[prevCoor.X + p.X, prevCoor.Y + p.Y] = StopPoint;
                }
                if (Bots[StopPoint].lives <= 0)
                {
                    for (int i = 0; i < Field.GetLength(0); i++)
                        for (int j = 0; j < Field.GetLength(1); j++)
                            if (Field[i, j] > StopPoint) Field[i, j]--;
                    Field[Bots[StopPoint].coordinates.X, Bots[StopPoint].coordinates.Y] = -3;
                    Bots.RemoveAt(StopPoint);
                    StopPoint--;
                }
                StopPoint++;
                if (rand.Next(50) == 0)
                    for (int i = 0; i < 20; i++)
                    {
                        int x = rand.Next(1, Field.GetLength(0) - 1);
                        int y = rand.Next(1, Field.GetLength(1) - 1);
                        if (Field[x, y] == -1)
                        {
                            Field[x, y] = -3;
                            break;
                        }
                    }
                if (Bots.Count <= 10)
                {
                    int t = Bots.Count;
                    if (AutoSave && generation % 50 == 0) AutoSaveF();
                    for (int i = 0; i < Bots.Count; i++)
                        Bots[i].Reset();
                    while (Bots.Count < 100)
                    {
                        Bots.Add(new Bot(Bots[(Bots.Count - 1) % t]));
                        if (Bots.Count > 90) Bots[Bots.Count - 1].Mutatin(5);
                    }
                    StopPoint = 0;
                    NewField();
                    generation++;
                }
                if (StopPoint == 1) DrawImageField();
            }
        }
        public string Getsave()
        {
            string save = "";
            save += Field.GetLength(0) + " ";
            save += Field.GetLength(1) + " ";
            save += StopPoint + " ";
            save += generation + "*";
            for (int j = 0, k = 0, p = 0; j < Field.GetLength(1); j++)
                for (int i = 0; i < Field.GetLength(0); i++)
                {
                    if (k == 0) { save += Field[i, j] + " "; p = Field[i, j]; }
                    else if (p != Field[i, j])
                    {
                        save += k + " ";
                        k = 0;
                        save += Field[i, j] + " ";
                        p = Field[i, j];
                    }
                    k++;
                    if (j * Field.GetLength(0) + i == Field.Length - 1) save += k + "*";
                }
            for (int i = 0; i < Bots.Count; i++)
            {
                if (i != 0) save += "&";
                save += Bots[i].Save();
            }
            return save;
        }
        public void SetSave(string save)
        {
            if (t != null) t.Abort();
            string[] saves1 = save.Split('*');
            Field = new int[Convert.ToInt32(saves1[0].Split(' ')[0]), Convert.ToInt32(saves1[0].Split(' ')[1])];
            StopPoint = Convert.ToInt32(saves1[0].Split(' ')[2]);
            generation = Convert.ToInt32(saves1[0].Split(' ')[3]);
            for (int i = 0, x = 0, y = 0; i < saves1[1].Split(' ').Length; i += 2)
            {
                int p = Convert.ToInt32(saves1[1].Split(' ')[i]), k = Convert.ToInt32(saves1[1].Split(' ')[i + 1]);
                while (k > 0)
                {
                    try
                    {
                        Field[x, y] = p;
                        x++;
                    }
                    catch
                    {
                        x = 0;
                        y++;
                        Field[x, y] = p;
                        x++;
                    }
                    k--;
                }
            }
            Bots.Clear();
            int i1 = 0;
            while (true)
            {
                try
                {
                    Bots.Add(new Bot(saves1[2].Split('&')[i1]));
                }
                catch
                {
                    break;
                }
                i1++;
            }
            DrawImageField();
        }
        void AutoSaveF()
        {
            string save = null;
            save = Getsave();
            System.IO.File.WriteAllText(fileName, save);
        }

        public FormMain()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            for (int i = 0; i < 100; i++) Bots.Add(new Bot());
            NewField();
            DrawImageField();
        }
        private void StartStopButton_Click(object sender, EventArgs e)
        {
            if (Stop)
            {
                if (t != null) t.Abort();
                t = new Thread(Simulation);
                t.Start();
                StartStopButton.Text = "Stop";
                Stop = false;
            }
            else
            {
                Stop = true;
                StartStopButton.Text = "Start";
            }
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (t != null) t.Abort();
            Stop = true;
            StartStopButton.Text = "Start";
            Bots.Clear();
            for (int i = 0; i < 100; i++) Bots.Add(new Bot());
            NewField();
            DrawImageField();
        }
        private void OptionsButton_Click(object sender, EventArgs e)
        {
            Stop = true;
            StartStopButton.Text = "Start";
            FormOptions form = new FormOptions(this);
            form.ShowDialog();
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != null) t.Abort();
        }
    }
    public class Bot
    {
        const int MaxLives = 60;
        static Random rand = new Random();
        int[] mind = new int[100];
        int[] memory = new int[10];
        int counter = 0;
        int angle;
        int attempts = 20;
        int[,,] neuron = new int[4, 4, 6];
        public Point coordinates = new Point();
        public bool isMurant { get; private set; } = false;
        public int lives { get; private set; } = MaxLives;

        public Bot()
        {
            for (int i = 0; i < mind.Length; i++)
                mind[i] = rand.Next(144);
            for (int i = 0; i < memory.Length; i++)
                memory[i] = 0;
            angle = rand.Next(4);
            for (int i1 = 0; i1 < neuron.GetLength(0); i1++)
                for (int i2 = 0; i2 < neuron.GetLength(1); i2++)
                    for (int i3 = 0; i3 < neuron.GetLength(2); i3++)
                        neuron[i1, i2, i3] = rand.Next(-3, 4);
        }
        public Bot(Bot b)
        {
            b.mind.CopyTo(mind, 0);
            b.memory.CopyTo(memory, 0);
            angle = b.angle;
            lives = b.lives;
            counter = b.counter;
            coordinates = b.coordinates;
            neuron = Copy<int>(b.neuron);
            isMurant = false;
        }
        public Bot(int[] mind, int[,,] neuron)
        {
            this.mind = new int[100];
            mind.CopyTo(this.mind, 0);
            for (int i = 0; i < memory.Length; i++)
                memory[i] = 0;
            angle = rand.Next(4);
            this.neuron = new int[3, 4, 6];
            this.neuron = Copy<int>(neuron);
        }
        public Bot(string save)
        {
            string[] saves1 = save.Split('=');
            for (int i = 0; i < mind.Length; i++)
                mind[i] = Convert.ToInt32(saves1[0].Split(' ')[i]);
            for (int i1 = 0; i1 < neuron.GetLength(0); i1++)
                for (int i2 = 0; i2 < neuron.GetLength(1); i2++)
                    for (int i3 = 0; i3 < neuron.GetLength(2); i3++)
                        neuron[i1, i2, i3] = Convert.ToInt32(saves1[1].Split(' ')
                            [i3 + neuron.GetLength(2) * i2 + neuron.GetLength(2) * neuron.GetLength(1) * i1]);
            for (int i = 0; i < 10; i++)
                memory[i] = Convert.ToInt32(saves1[2].Split(' ')[i]);
            coordinates = new Point(Convert.ToInt32(saves1[3].Split(' ')[0]), Convert.ToInt32(saves1[3].Split(' ')[1]));
            angle = Convert.ToInt32(saves1[3].Split(' ')[2]);
            lives = Convert.ToInt32(saves1[3].Split(' ')[3]);
            counter = Convert.ToInt32(saves1[3].Split(' ')[4]);
        }

        public void Mutatin(int HowMany)
        {
            for (int i = 0; i < HowMany; i++)
            {
                mind[rand.Next(mind.Length)] = rand.Next(144);
                neuron[rand.Next(neuron.GetLength(0)), rand.Next(neuron.GetLength(1)), rand.Next(neuron.GetLength(2))] = rand.Next(-3, 4);
            }
            isMurant = true;
        }
        public Point MakeAMove(int[] Review)
        {
            if (angle == 2) Review = CoupArray(Review);
            else if (angle == 1 || angle == 3)
            {
                int[] index = { 7, 11, 16, 2, 6, 10, 15, 19, 1, 5, 14, 18, 0, 4, 9, 13, 17, 3, 8, 12 };
                int[] arr = new int[Review.Length];
                Review.CopyTo(arr, 0);
                for (int i = 0; i < Review.Length; i++)
                    Review[i] = arr[index[i]];
                if (angle == 3) Review = CoupArray(Review);
            }
            attempts = 20;
            while (attempts > 0)
            {
                counter %= 100;
                if (mind[counter] < 100) counter = mind[counter];
                else if (mind[counter] < 110) counter = memory[mind[counter] % 10];
                else if (mind[counter] < 120) { memory[mind[counter] % 10] = mind[(counter + 1) % 100]; counter += 2; }
                else if (mind[counter] < 136)
                {
                    int[,] check = {
                    {0, 1, 2, 4, 5, 6},
                    {7, 11, 16, 6, 10, 15},
                    {19, 18, 17, 15, 14, 13},
                    {12, 8, 3, 13, 9, 4 }};
                    int sum = 0;
                    for (int i = 0; i < neuron.GetLength(2); i++)
                        if (Review[check[mind[counter] % 4, i]] >= 0 && (mind[counter] - 120) / 4 == 0) sum += neuron[0, mind[counter] % 4, i];
                        else if (Review[check[mind[counter] % 4, i]] == -2 && (mind[counter] - 120) / 4 == 1) sum += neuron[1, mind[counter] % 4, i];
                        else if (Review[check[mind[counter] % 4, i]] == -3 && (mind[counter] - 120) / 4 == 2) sum += neuron[2, mind[counter] % 4, i];
                        else if (Review[check[mind[counter] % 4, i]] == -4 && (mind[counter] - 120) / 4 == 3) sum += neuron[3, mind[counter] % 4, i];
                    if (sum <= 0) sum = 1;
                    else sum = sum / 7 + 1;
                    counter += sum;
                }
                else if (mind[counter] < 144)
                {
                    int[] check = { 5, 6, 10, 15, 14, 13, 9, 4 };
                    Point move = new Point(0, 0);
                    int t = mind[counter] - 136;
                    counter++;
                    int field = Review[check[t]];
                    t = (t + 2 * angle) % 8;
                    if (t == 0 || t == 1 || t == 7) move.Y -= 1;
                    if (t == 1 || t == 2 || t == 3) move.X += 1;
                    if (t == 3 || t == 4 || t == 5) move.Y += 1;
                    if (t == 5 || t == 6 || t == 7) move.X -= 1;
                    lives--;
                    if (field == -1 || field == -3 || field == -4 || field >= 0)
                    {
                        coordinates.X += move.X;
                        coordinates.Y += move.Y;
                        if (field == -3 || field >= 0) lives = Math.Min(MaxLives, lives + 10);
                        else if (field == -4) lives = 0;
                        return move;
                    }
                    else return new Point(0, 0);
                }
                else if (mind[counter] < 148)
                {
                    if (mind[counter] - 144 == 2) Review = CoupArray(Review);
                    else if (mind[counter] - 144 == 1 || mind[counter] - 144 == 3)
                    {
                        int[] index = { 7, 11, 16, 2, 6, 10, 15, 19, 1, 5, 14, 18, 0, 4, 9, 13, 17, 3, 8, 12 };
                        int[] arr = new int[Review.Length];
                        Review.CopyTo(arr, 0);
                        for (int i = 0; i < Review.Length; i++)
                            Review[i] = arr[index[i]];
                        if (mind[counter] - 144 == 3) Review = CoupArray(Review);
                    }
                    angle = (angle + mind[counter] - 144) % 4;
                    counter++;
                }
                attempts--;
            }
            lives--;
            return new Point(0, 0);
        }
        public void Reset()
        {
            for (int i = 0; i < memory.Length; i++)
                memory[i] = 0;
            angle = rand.Next(4);
            counter = 0;
            isMurant = false;
            lives = MaxLives;
        }
        public string Save()
        {
            string save = "";
            for (int i = 0; i < mind.Length; i++)
            {
                if (i != 0) save += " ";
                save += Convert.ToString(mind[i]);
            }
            save += "=";
            for (int i1 = 0; i1 < neuron.GetLength(0); i1++)
                for (int i2 = 0; i2 < neuron.GetLength(1); i2++)
                    for (int i3 = 0; i3 < neuron.GetLength(2); i3++)
                    {
                        if (i1 + i2 + i3 != 0) save += " ";
                        save += Convert.ToString(neuron[i1, i2, i3]);
                    }
            save += "=";
            for (int i = 0; i < 10; i++)
            {
                if (i != 0) save += " ";
                save += Convert.ToString(memory[i]);
            }
            save += "=";
            save += Convert.ToString(coordinates.X) + " ";
            save += Convert.ToString(coordinates.Y) + " ";
            save += Convert.ToString(angle) + " ";
            save += Convert.ToString(lives) + " ";
            save += Convert.ToString(counter);
            return save;
        }

        private static int[] CoupArray(int[] arr)
        {
            int[] arr2 = new int[arr.Length];
            arr.CopyTo(arr2, 0);
            for (int i = 0; i < arr.Length; i++)
                arr2[i] = arr[arr.Length - 1 - i];
            return arr2;
        }
        private static T[,,] Copy<T>(T[,,] array)
        {
            T[,,] newArray = new T[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    for (int k = 0; k < array.GetLength(2); k++)
                        newArray[i, j, k] = array[i, j, k];
            return newArray;
        }
    }
}