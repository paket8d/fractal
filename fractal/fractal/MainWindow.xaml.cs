using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace fractal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class cmpl
    {
        public double re;
        public double im;

        public cmpl (double x, double y)
        {
            re = x;
            im = y;
        }
        public cmpl () 
        {
            re = 0;
            im = 0;
        }

        public static cmpl operator +(cmpl c1, cmpl c2)
            => new cmpl(c1.re + c2.re, c1.im + c2.im);

        public static cmpl operator *(cmpl c1, cmpl c2)
            => new cmpl(c1.re * c2.re - c1.im * c2.im, c1.re * c2.im + c2.re * c1.im);

        public static cmpl operator -(cmpl c1, cmpl c2)
            => new cmpl(c1.re - c2.re, c1.im - c2.im);

        public double mod()
        {
            return re * re + im * im;
        }

        public void print()
        {
            if (im >= 0) Console.WriteLine($"{re}+{im}i");
            else Console.WriteLine($"{re}{im}i");
        }
    };

    public partial class MainWindow : Window
    {
        cmpl c1 = new cmpl(-2, 2), c2 = new cmpl(2, -2); // комплексные координаты углов
        Rectangle rect = new Rectangle();
        bool mouse = false;
        Point p1, p2;
        byte[] arr = new byte[((600 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8) * 600];
        double par1, par2, pars;
        bool type_fractal = false;
        cmpl parj = new cmpl();

        cmpl Stc(double x, double y)
        {
            double prop = Math.Abs(c1.re - c2.re) / 600.0;
            double xn = prop * x + c1.re;
            double yn = - prop * y + c1.im;
            cmpl res = new cmpl(xn, yn);
            return res;
        }
        
        int IsMandelbrot(cmpl c)
        {
            cmpl z = new cmpl();
            int cnt = 0;
            double modul = z.mod();
            while (cnt < 300 && modul < 4.0)
            {
                z = z * z + c;
                cnt++;
                modul = z.mod();
            }
            if (modul < 4.0) return 0;
            else return cnt;
        }

        int IsJulia(cmpl c)
        {
            cmpl z = c;
            cmpl c1 = parj;
            int cnt = 0;
            double modul = z.mod();
            while (cnt < 300 && modul < 4.0)
            {
                z = z * z + c1;
                cnt++;
                modul = z.mod();
            }
            if (modul < 4.0) return 0;
            else return cnt;
        }

        void Mandelbrot(ref byte[] arr)
        {
            for (int i = 0; i < 600; ++i)
            {
                for (int j = 0; j < 600; ++j)
                {
                    cmpl c = Stc(i, j);
                    int t = IsMandelbrot(c);
                    if (t == 0)
                    {
                        for (int ind = 0; ind < 3; ++ind)
                        {
                            arr[((600 * j) + i) * 4 + ind] = 0;
                        }
                    }
                    else
                    {
                        for (int ind = 0; ind < 3; ++ind)
                        {
                            arr[((600 * j) + i) * 4 + ind] = (byte)(Math.Sin(t / pars + ind / par1 + par2) * 255);
                        }
                    }
                }
            }
            BitmapSource bmp = BitmapSource.Create(600, 600, 96, 96, PixelFormats.Bgr32, null, arr, (600 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8);
            image1.Source = bmp;
            return;
        }

        void Julia(ref byte[] arr)
        {
            
            for (int i = 0; i < 600; ++i)
            {
                for (int j = 0; j < 600; ++j)
                {
                    cmpl c = Stc(i, j);
                    int t = IsJulia(c);
                    if (t == 0)
                    {
                        for (int ind = 0; ind < 3; ++ind)
                        {
                            arr[((600 * j) + i) * 4 + ind] = 0;
                        }
                    }
                    else
                    {
                        for (int ind = 0; ind < 3; ++ind)
                        {
                            arr[((600 * j) + i) * 4 + ind] = (byte)(Math.Sin(t / pars + ind / par1 + par2) * 255);
                        }
                    }
                }
            }
            BitmapSource bmp = BitmapSource.Create(600, 600, 96, 96, PixelFormats.Bgr32, null, arr, (600 * PixelFormats.Bgr32.BitsPerPixel + 7) / 8);
            image1.Source = bmp;
            return;
        }

        private void MM(object sender, MouseEventArgs e)
        {
            if (mouse)
            {
                p2 = e.GetPosition(this);
                canvas1.Children.Remove(rect);
                double h = Math.Abs(p1.Y - p2.Y), w = Math.Abs(p1.X - p2.X);
                rect.Width = Math.Min(h, w);
                rect.Height = Math.Min(h, w);
                if (p2.X > p1.X && p2.Y > p1.Y)
                { 
                    Canvas.SetLeft(rect, p1.X - 50);
                    Canvas.SetTop(rect, p1.Y - 50);
                }
                else if (p2.X < p1.X && p2.Y > p1.Y)
                {
                    Canvas.SetLeft(rect, p1.X - rect.Width - 50);
                    Canvas.SetTop(rect, p1.Y - 50);
                }
                else if (p2.X < p1.X && p2.Y < p1.Y)
                {
                    Canvas.SetLeft(rect, p1.X - rect.Width - 50);
                    Canvas.SetTop(rect, p1.Y - rect.Height - 50);
                }
                else if (p2.X > p1.X && p2.Y < p1.Y)
                {
                    Canvas.SetLeft(rect, p1.X - 50);
                    Canvas.SetTop(rect, p1.Y - rect.Height - 50);
                }
                rect.Stroke = new SolidColorBrush(Colors.White);
                rect.StrokeThickness = 1;
                canvas1.Children.Add(rect);
            }
        }

        private void VC_S1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            parj.re = e.NewValue;
        }

        private void VC_S2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            parj.im = e.NewValue;
        }

        private void VC_S3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            par2 = e.NewValue;
        }

        private void VC_S4(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            par1 = e.NewValue;
        }

        private void VC_S5(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pars = e.NewValue;
        }

        private void RBC(object sender, RoutedEventArgs e)
        {
            c1 = new cmpl(-2, 2);
            c2 = new cmpl(2, -2);
            if (type_fractal == false)
            {
                Mandelbrot(ref arr);
            }
            else
            {
                Julia(ref arr);
            }
        }

        private void draw(object sender, RoutedEventArgs e)
        {
            if (type_fractal == false)
            {
                Mandelbrot(ref arr);
            }
            else
            {
                Julia(ref arr);
            }
        }

        private void MLBU(object sender, MouseButtonEventArgs e)
        {
            if (mouse)
            {
                mouse = false;
                p1 = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
                p2 = new Point(p1.X + rect.Width, p1.Y + rect.Height);
                cmpl cn1 = Stc(p1.X, p1.Y), cn2 = Stc(p2.X, p2.Y);
                c1 = cn1;
                c2 = cn2;
                if (type_fractal == false) Mandelbrot(ref arr);
                else Julia(ref arr);
                canvas1.Children.Remove(rect);
                rect.Width = 0;
                rect.Height = 0;
            }
        }

        private void type_button_click(object sender, RoutedEventArgs e)
        {
            if (type_fractal == false)
            {
                type_fractal = true;
                type_button.Content = "Mandelbrot";
                Julia(ref arr);
                c1 = new cmpl(-2, 2);
                c2 = new cmpl(2, -2);
            }
            else
            {
                type_fractal = false;
                type_button.Content = "Julia";
                Mandelbrot(ref arr);
                c1 = new cmpl(-2, 2);
                c2 = new cmpl(2, -2);
            }
        }

        private void MLBD(object sender, MouseButtonEventArgs e)
        {
            p1 = e.GetPosition(this);
            mouse = true;
            canvas1.Children.Add(rect);
        }

        public MainWindow()
        { 
            InitializeComponent();
            Mandelbrot(ref arr);
        }
    }
}
