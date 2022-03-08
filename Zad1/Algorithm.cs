using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace Zad1
{
    public static class Algorithm
    {
        public static Bitmap HistogramBitmap(Bitmap bitmap)
        {
            var data = bitmap.LockBits(
               new Rectangle(0, 0, bitmap.Width, bitmap.Height),
               System.Drawing.Imaging.ImageLockMode.ReadWrite,
               System.Drawing.Imaging.PixelFormat.Format24bppRgb
           );

            return bitmap;
        }
        public static Bitmap Histogram(Bitmap bmp_original)
        {
            Bitmap bmp = new Bitmap(bmp_original);
            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );

            var bmpData = new byte[data.Width * 3 * data.Height];
            Marshal.Copy(data.Scan0, bmpData, 0, bmpData.Length);
            // Przerzuci z Bitmapy do tablicy

            int[] histogram = new int[256];

            foreach (byte i in bmpData)
                ++histogram[i];

            double max = histogram.Max();

            for (int i = 0; i < histogram.Length; i++)
                histogram[i] = (int)(histogram[i] / max * 512.0);



            bmpData = new byte[bmpData.Length];

            for (int i = 0; i < bmpData.Length; i++)
                bmpData[i] = 255;

            MessageBox.Show("Width: " + data.Width +
    "\nStride: " + data.Stride +
    "\nHeight: " + data.Height +
    "Width * 3: " + data.Width * 3);


            for (int i = 0; i < histogram.Length; i++)
            {
                for (int j = 0; j < histogram[i]; j++)
                {
                    int index = i * 3 + (511 - j) * data.Width * 3;

                    //bmpData[index + 0] = 0;
                    //bmpData[index + 1] = 0;
                    //bmpData[index + 2] = 0;
                    // bmpData[index] = 0;

                }
            }

            for (int i = 0; i < bmpData.Length / 3; i++)
            {
                if(i > 69 && i < 2137)
                     bmpData[i * 3] = 50;
            } 

            Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);
            // Przerzuci z tablicy do Bitmapy

            bmp.UnlockBits(data);
            return bmp;
        }

        public static Bitmap BinaryThreshold(Bitmap bmp, byte threshold)
        {
            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );

            /*
             * 3x4 => 12 pikseli
             * 9x4 => 36 bajtów (RGB)
             */

            // Width - szerokość w pikselach
            // Stride - szerokość w bajtach
            // Stride => Width * 3
            var bmpData = new byte[data.Width * 3 * data.Height];

            Marshal.Copy(data.Scan0, bmpData, 0, bmpData.Length);
            // Przerzuci z Bitmapy do tablicy


            for (int i = 0; i < bmpData.Length; i += 3)
            {
                byte r = bmpData[i + 0];
                byte g = bmpData[i + 1];
                byte b = bmpData[i + 2];

                byte mean = (byte)((r + g + b) / 3);

                bmpData[i + 0] =
                bmpData[i + 1] =
                bmpData[i + 2] = mean > threshold ? byte.MaxValue : byte.MinValue;
            }

            Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);

            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
