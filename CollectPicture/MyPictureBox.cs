using System.Drawing;
using System.Windows.Forms;

namespace CollectPicture
{
    class MyPictureBox : PictureBox
    {
        private readonly Point _startPosition;

        public MyPictureBox() : this(0, 0) { }

        public MyPictureBox(int x, int y)
        {
            _startPosition.X = x;
            _startPosition.Y = y;
            Location = new Point(x, y);
        }

        public MyPictureBox(Point point) : this(point.X, point.Y) { }

        public MyPictureBox(int x, int y, int width, int height) : this(x, y)
        {
            Width = width;
            Height = height;
        }

        public MyPictureBox(int x, int y, int width, int height, BorderStyle borderStyle) :
            this(x, y, width, height)
        {
            BorderStyle = borderStyle;
        }

        public MyPictureBox(int x, int y, Size size, BorderStyle borderStyle) :
            this(x, y, size.Width, size.Height, borderStyle) { }

        public MyPictureBox(Point point, Size size, BorderStyle borderStyle) :
            this(point.X, point.Y, size.Width, size.Height, borderStyle) { }

        public void ResetLocation()
        {
            Location = _startPosition;
        }

        public bool IsDefaultPosition()
        {
            return Location == _startPosition;
        }

        public void MoveTo(Point position)
        {
            MoveTo(position.X, position.Y);
        }

        public void MoveTo(int x, int y)
        {
            int currentX = Location.X;
            int currentY = Location.Y;

            while (currentX != x || currentY != y)
            {
                if (currentX < x) currentX++;
                else if (currentX > x) currentX--;
                if (currentY < y) currentY++;
                else if (currentY > y) currentY--;
                Location = new Point(currentX, currentY);
            }
        }
    }
}