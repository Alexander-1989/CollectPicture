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
        enum MouseButtonsState : byte
        {
            None,
            LeftButtonClick,
            RightButtonClick,
            BothButtonClick
        }

        private Point lastMousePosition;
        private Point lastPicturePosition;
        private Size size = new Size(100, 100);
        private MouseButtonsState mouseButtonState;
        private const int rowsCount = 6;
        private string[] imgs = null;
        private readonly Random random = new Random();
        private readonly Config config = new Config();
        private readonly SoundPlayer sPlayer = new SoundPlayer();
        private readonly MyPictureBox[,] picturesBoxes = new MyPictureBox[rowsCount, rowsCount];

        public Form1()
        {
            InitializeComponent();
            button1.KeyDown += Button1_KeyDown;
            Load += (s, e) => Initialization();
            FormClosed += (s, e) => SaveConfig();
        }

        private void PlaySound()
        {
            if (sPlayer.IsLoadCompleted)
            {
                sPlayer.Play();
            }
        }

        private void Initialization()
        {
            LoadConfig();
            CreateGrid();
            SelectPicture(imgs);
        }

        private string[] GetPictures(string path, params string[] pattern)
        {
            return (from f in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    let ext = Path.GetExtension(f)
                    where pattern.Contains(ext)
                    select f).ToArray();
        }

        private void SaveConfig()
        {
            config.fields.Location = Location;
            config.Write();
        }

        private void LoadConfig()
        {
            try
            {
                config.Read();
                Location = config.fields.Location;

                imgs = GetPictures(config.fields.PicturesFolder, config.fields.Extensions);
                string soundFile = config.fields.SoundFile;

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

        private void CreateGrid()
        {
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < rowsCount; j++)
                {
                    picturesBoxes[i, j] = new MyPictureBox(j * 100, i * 100, size, BorderStyle.FixedSingle);
                    picturesBoxes[i, j].MouseDown += Form1_MouseDown;
                    picturesBoxes[i, j].MouseMove += Form1_MouseMove;
                    picturesBoxes[i, j].MouseUp += Form1_MouseUp;
                    Controls.Add(picturesBoxes[i, j]);
                }
            }
        }

        private void SelectPicture(string[] images)
        {
            if (!images.IsNullOrEmpty())
            {
                string pictureName = images.Choise();
                LoadPicture(pictureName);
                PlaySound();
            }
        }

        private void LoadPicture(string fileName)
        {
            using (Bitmap picture = new Bitmap(fileName))
            {
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < rowsCount; j++)
                    {
                        picturesBoxes[i, j].Enabled = false;
                        picturesBoxes[i, j].ResetLocation();
                        picturesBoxes[i, j].Image?.Dispose();
                        Rectangle rectangle = new Rectangle(j * 100, i * 100, size.Width, size.Height);
                        picturesBoxes[i, j].Image = picture.Clone(rectangle, picture.PixelFormat);
                    }
                }
            }
        }

        private void Shake()
        {
            foreach (MyPictureBox pBox in picturesBoxes)
            {
                pBox.Enabled = true;
                int i = random.Next(rowsCount);
                int j = random.Next(rowsCount);

                Point lastPosition = pBox.Location;
                pBox.Location = picturesBoxes[i, j].Location;
                picturesBoxes[i, j].Location = lastPosition;
            }
        }

        private void Reset()
        {
            foreach (MyPictureBox pBox in picturesBoxes)
            {
                pBox.Enabled = false;
                pBox.ResetLocation();
            }
        }

        private bool CheckPicturesPosition()
        {
            foreach (MyPictureBox pBox in picturesBoxes)
            {
                if (!pBox.IsDefaultPosition())
                {
                    return false;
                }
            }
            return true;
        }

        private void RepeatGame()
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

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MyPictureBox pBox = sender as MyPictureBox;
                pBox.BringToFront(); // pBox.SendToBack();
                lastPicturePosition = pBox.Location;
                lastMousePosition = e.Location;
                mouseButtonState |= MouseButtonsState.LeftButtonClick;
            }

            if (e.Button == MouseButtons.Right)
            {
                mouseButtonState |= MouseButtonsState.RightButtonClick;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MyPictureBox pBox = sender as MyPictureBox;
                int dx = e.Location.X - lastMousePosition.X;
                int dy = e.Location.Y - lastMousePosition.Y;
                pBox.Location = new Point(pBox.Location.X + dx, pBox.Location.Y + dy);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseButtonState != MouseButtonsState.RightButtonClick)
            {
                int x = MousePosition.X - Left - 8;
                int y = MousePosition.Y - Top - 32;
                MyPictureBox currentBox = sender as MyPictureBox;

                foreach (MyPictureBox pBox in picturesBoxes)
                {
                    if (pBox != currentBox &&
                        x > pBox.Location.X &&
                        x < pBox.Location.X + pBox.Width &&
                        y > pBox.Location.Y &&
                        y < pBox.Location.Y + pBox.Height)
                    {
                        Point pBoxLocation = pBox.Location;
                        pBox.MoveTo(lastPicturePosition);
                        lastPicturePosition = pBoxLocation;
                        break;
                    }
                }

                currentBox.MoveTo(lastPicturePosition);
                PlaySound();
            }

            mouseButtonState = MouseButtonsState.None;
            if (CheckPicturesPosition())
            {
                RepeatGame();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectPicture(imgs);
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

        private void Button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.Control && e.KeyValue == 'O')
            {
                System.Diagnostics.Process.Start("https://avatars.alphacoders.com/by_resolution/600");
            }
        }
    }
}