using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;

namespace Identicon
{
    class Identicon
    {
        private string input;
        private string hash;
        private Bitmap image;
        private List<int> matrix;
        private Color color;

        static private int width = 1000;
        static private int height = 1000;

        public Identicon(string input)
        {
            this.input = input;
            image = new Bitmap(height, width);
            hash = HashString(this.input);
            color = SelectColor(hash);
            matrix = SetMatrix(hash);
        }

        private static List<int> SetMatrix(string hash)
        {
            List<int> left = new List<int>(15);
            List<int> right = new List<int>(15);
            List<int> full = new List<int>(30);
            int w = 3;

            // Create Left side
            for (int i = 0; i < 30; i += 2)
            {
                string h = hash.Substring(i, 2);
                int v = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                left.Add(v);
            }

            // Create Right side
            int r = 0;
            int col = 0;
            for (int i = 0; i < left.Count; i++)
            {
                int offset = r * w;
                right.Insert(offset + col, left[(offset + (w - col)) - 1]);
                col++;
                if (col >= w)
                {
                    col = 0;
                }
                if ((i + 1) % w == 0)
                {
                    r++;
                }
            }

            // Assemble the sides
            int row = 0;
            for (int i = 0; i < 30; i += (w * 2))
            {
                for (int j = 0; j < (w * 2); j++)
                {
                    if (j >= w)
                    {
                        full.Insert(i + j, right[(row * w) + (j - w)]);
                    }
                    else
                    {
                        int index1 = i + j;
                        int index2 = (row * w) + j;
                        full.Insert(index1, left[index2]);
                    }
                }
                row++;
            }

            return full;
        }

        private static string HashString(string input)
        {
            using (MD5 hash = MD5.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private static Color SelectColor(string hash)
        {
            string rhex = hash.Substring(1, 2);
            string ghex = hash.Substring(4, 2);
            string bhex = hash.Substring(5, 2);

            int r = int.Parse(rhex, System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(ghex, System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(bhex, System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(255, r, g, b);
        }

        private bool ShouldColor(int x, int y)
        {
            int squareX = (int)(((float)x / width) * 5);
            int squareY = (int)(((float)y / height) * 6);

            int index = (squareY * 5) + squareX;

            return matrix[index] > 127;
        }

        private void SetPixel(int x, int y)
        {
            if (ShouldColor(x, y))
            {
                image.SetPixel(x, y, color);
                return;
            }
            image.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
        }

        public void Write(string path)
        {
            image = new Bitmap(width, height);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    SetPixel(i, j);
                }
            }

            image.Save(path);
        }
    }
}
