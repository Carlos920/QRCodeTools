using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ZXing;
using ZXing.Common;

namespace QRCodeTools.screenshot
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureWindow : Window
    {
        public CaptureWindow()
        {
            InitializeComponent();
            //this.Deactivated += CaptureWindow_Deactivated;
        }

        private double x;
        private double y;
        private double width;
        private double height;

        private bool isMouseDown = false;

        public ResultWindow resultWindow = null;
        private IBarcodeReader barcodeReader = null;

        private double _dpiRatio;

        public double dpiRatio
        {
            get => this._dpiRatio;
            set { }
        }

        private void FinishCapture()
        {
            if (isMouseDown) isMouseDown = false;
            x = 0.0;
            y = 0.0;
            CaptureCanvas.Children.Clear();
            Hide();
            foreach (Window win in App.Current.Windows)
            {
                if(win.Title== "QRCodeTools")
                {
                    win.Show();
                }
            }
        }

        private void CaptureWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isMouseDown = true;
                x = e.GetPosition(null).X;
                y = e.GetPosition(null).Y;
            }
            else
            {
                FinishCapture();
            }
                
        }

        private void CaptureWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                // 1. 通过一个矩形来表示目前截图区域
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                double dx = e.GetPosition(null).X;
                double dy = e.GetPosition(null).Y;
                double rectWidth = Math.Abs(dx - x);
                double rectHeight = Math.Abs(dy - y);
                SolidColorBrush brush = new SolidColorBrush(Colors.White);
                rect.Width = rectWidth;
                rect.Height = rectHeight;
                rect.Fill = brush;
                rect.Stroke = brush;
                rect.StrokeThickness = 1;
                if (dx < x)
                {
                    Canvas.SetLeft(rect, dx);
                    Canvas.SetTop(rect, dy);
                }
                else
                {
                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                }

                CaptureCanvas.Children.Clear();
                CaptureCanvas.Children.Add(rect);

                if (e.LeftButton == MouseButtonState.Released)
                {
                    CaptureCanvas.Children.Clear();
                    // 2. 获得当前截图区域
                    width = Math.Abs(e.GetPosition(null).X - x);
                    height = Math.Abs(e.GetPosition(null).Y - y);
                    Bitmap bitmap = null;

                    if (e.GetPosition(null).X > x)
                    {
                        bitmap = CaptureScreenWithCurrentDpi(x, y, width, height);
                    }
                    else
                    {
                        bitmap = CaptureScreenWithCurrentDpi(e.GetPosition(null).X, e.GetPosition(null).Y, width, height);
                    }


                    if (bitmap != null)
                    {
                        this.ShowText(this.BarScan(bitmap));
                    }
                    else
                    {
                        MessageBox.Show("Error.");
                    }

                    FinishCapture();
                }
            }
        }

        //获取当前dpi的缩放比
        public double GetDpiRatio(Window window)
        {
            var currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(window).Handle);
            //96是100%的dpi
            return currentGraphics.DpiX / 96;
        }

        //以当前DPI截取图像
        public Bitmap CaptureScreenWithCurrentDpi(double x, double y, double width, double height)
        {
            //GetDpiRatio方法，请参阅：https://huchengv5.gitee.io/post/WPF-%E5%A6%82%E4%BD%95%E8%8E%B7%E5%8F%96%E7%B3%BB%E7%BB%9FDPI.html
            this._dpiRatio = GetDpiRatio(this);
            return CaptureScreen(x * _dpiRatio, y * _dpiRatio, width * _dpiRatio, height * _dpiRatio);
        }

        private Bitmap CaptureScreen(double x, double y, double width, double height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            Bitmap bitmap = new Bitmap(iw, ih);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));

                return bitmap;
            }
        }

        //识别


        private void ShowText(Result result)
        {
            if (result == null)
            {
                Console.WriteLine("Decode failed.");
                MessageBox.Show("Failed");
            }
            else
            {
                if (this.resultWindow == null || this.resultWindow.IsClosed)
                {
                    this.resultWindow = new ResultWindow();
                }
                
                this.resultWindow.Left = this.Width / 2;
                this.resultWindow.Top = this.Height / 2;
                
                this.resultWindow.Result = result;
                Clipboard.SetText(result.Text);
                
                this.resultWindow.Show();
                this.resultWindow.Owner = this;
                if (!this.resultWindow.IsActive)
                {
                    this.resultWindow.Activate();
                }
                Console.WriteLine("BarcodeFormat: {0}", result.BarcodeFormat);
                Console.WriteLine("Result: {0}", result.Text);
            }
        }

        //使用ZXing.Net解码
        private Result BarScan(Bitmap bitmap)
        {
            var source = new BitmapLuminanceSource(bitmap);

            if (this.barcodeReader == null)
            {
                new BarcodeReader();
                this.barcodeReader = new BarcodeReader(null, null, ls => new GlobalHistogramBinarizer(ls))
                {
                    AutoRotate = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        TryInverted = true,
                      
                    }
                };
            }

            return barcodeReader.Decode(source);
        }

        private void BarGenerate()
        {

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.resultWindow != null)
            {
                try
                {
                    this.resultWindow.Close();
                }
                catch
                {

                }
            }
        }
    }
}
