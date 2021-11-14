using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Drawing;
using System.Windows.Forms;

namespace CollectPicture
{
    public partial class Form1 : Form
    {
        string[] imgs = null;
        Size size = new Size(100, 100);
        Random rnd = new Random();
        Config cfg = new Config();
        SoundPlayer sPlayer = new SoundPlayer();
        Point old_picture_pos, old_mouse_pos;
        MyPictureBox[,] picturesBox = new MyPictureBox[6, 6];

        public Form1()
        {
            InitializeComponent();
            FormClosed += Form1_FormClosed;
            button1.KeyDown += Button1_KeyDown;
            Init();
        }

        private void Button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.Control && e.KeyValue == 'O')
            {
                System.Diagnostics.Process.Start("https://avatars.alphacoders.com/by_resolution/600");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            cfg.fields.Location = Location;
            cfg.Save();
        }

        private string[] GetPictures(string path, params string[] pattern)
        {
            return (from f in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    let ext = Path.GetExtension(f)
                    where pattern.Contains(ext)
                    select f).ToArray();
        }

        private void LoadConfiguration()
        {
            try
            {
                cfg.Open();
                Location = cfg.fields.Location;
                imgs = GetPictures(cfg.fields.PicturesFolder, cfg.fields.Extensions);
                string soundFile = Path.Combine(cfg.fields.SoundsFolder, "Click.wav");
                if (File.Exists(soundFile))
                {
                    sPlayer.SoundLocation = soundFile;
                    sPlayer.Load();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Init()
        {
            LoadConfiguration();
            CreateGrid();
            SelectPicture(imgs);
        }

        private void CreateGrid()
        {
            for (int i = 0; i < picturesBox.GetLength(0); ++i)
            {
                for (int j = 0; j < picturesBox.GetLength(1); ++j)
                {
                    picturesBox[i, j] = new MyPictureBox(new Point(j * 100, i * 100),
                        size, BorderStyle.FixedSingle);
                    picturesBox[i, j].MouseDown += Form1_MouseDown;
                    picturesBox[i, j].MouseMove += Form1_MouseMove;
                    picturesBox[i, j].MouseUp += Form1_MouseUp;
                    Controls.Add(picturesBox[i, j]);
                }
            }
        }

        private void SelectPicture(string[] images)
        {
            if (images == null || images.Length == 0) return;
            string pictureName = images.Choise();
            LoadPicture(pictureName);
        }

        private void LoadPicture(string fileName)
        {
            using (Bitmap picture = new Bitmap(fileName))
            {
                for (int i = 0; i < picturesBox.GetLength(0); ++i)
                {
                    for (int j = 0; j < picturesBox.GetLength(1); ++j)
                    {
                        picturesBox[i, j].Enabled = false;
                        picturesBox[i, j].ResetLocation();
                        picturesBox[i, j].Image?.Dispose();
                        Rectangle rec = new Rectangle(new Point(j * 100, i * 100), size);
                        picturesBox[i, j].Image = picture.Clone(rec, picture.PixelFormat);
                    }
                }
            }
        }

        private void Shake()
        {
            foreach (MyPictureBox pBox in picturesBox)
            {
                pBox.Enabled = true;
                int i = rnd.Next(picturesBox.GetLength(0));
                int j = rnd.Next(picturesBox.GetLength(1));

                Point tempPos = pBox.Location;
                pBox.Location = picturesBox[i, j].Location;
                picturesBox[i, j].Location = tempPos;
            }
        }

        private void Reset()
        {
            foreach (MyPictureBox pBox in picturesBox)
            {
                pBox.Enabled = false;
                pBox.ResetLocation();
            }
        }

        private bool CheckPicturesPosition()
        {
            foreach (MyPictureBox pBox in picturesBox)
            {
                if (!pBox.IsDefaultPosition()) return false;
            }
            return true;
        }

        private void PlaySound()
        {
            if (sPlayer.IsLoadCompleted)
            {
                sPlayer.Play();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MyPictureBox pBox = sender as MyPictureBox;
                pBox.BringToFront(); // mpb.SendToBack();
                old_picture_pos = pBox.Location;
                old_mouse_pos = e.Location;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MyPictureBox pBox = sender as MyPictureBox;
                int dx = e.Location.X - old_mouse_pos.X;
                int dy = e.Location.Y - old_mouse_pos.Y;
                pBox.Location = new Point(pBox.Location.X + dx, pBox.Location.Y + dy);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int x = MousePosition.X - Left - 8;
            int y = MousePosition.Y - Top - 32;
            MyPictureBox currentBox = sender as MyPictureBox;

            foreach (MyPictureBox pBox in picturesBox)
            {
                if (pBox != currentBox &&
                    x > pBox.Location.X &&
                    x < pBox.Location.X + pBox.Width &&
                    y > pBox.Location.Y &&
                    y < pBox.Location.Y + pBox.Height)
                {
                    Point pBoxLocation = pBox.Location;
                    pBox.MoveTo(old_picture_pos);
                    old_picture_pos = pBoxLocation;

                    if (CheckPicturesPosition())
                    {
                        if (MessageBox.Show("Вы хотите сыграть еще раз?", "Внимание",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SelectPicture(imgs);
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }

                    break;
                }
            }

            currentBox.MoveTo(old_picture_pos);
            PlaySound();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectPicture(imgs);
            PlaySound();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Shake();
            PlaySound();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Reset();
            PlaySound();
        }
    }
}