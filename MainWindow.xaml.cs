using QRCodeTools.screenshot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QRCodeTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CaptureWindow captureWindow = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void Capture_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.captureWindow == null)
            {
                this.captureWindow = new CaptureWindow();
            }
            this.captureWindow.Show();
            this.captureWindow.Owner = this;
            Hide();
            
        }


        #region 主窗口将要关闭时的操作
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.captureWindow != null)
            {
                try
                {
                    this.captureWindow.Close();
                }
                catch
                {

                }
            }
        }
        #endregion
    }
}
