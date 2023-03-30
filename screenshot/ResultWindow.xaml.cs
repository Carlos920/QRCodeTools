using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZXing;

namespace QRCodeTools.screenshot
{
    /// <summary>
    /// 结果集
    /// </summary>
    public partial class ResultWindow : Window
    {

        private Result _result;
        public bool IsClosed { get; private set; }

        public ResultWindow()
        {
            InitializeComponent();
        }

        public Result Result
        {
            get => _result;
            set
            {
                _result = value;
                TextBox.Text = value.Text;
                TextBox.SelectAll();
                this.Title = value.BarcodeFormat.ToString();
            }
        }
    
        private async void Copy_Btn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_result.Text);

            this.Copy_Btn.Content = "Copied!";
            await Task.Delay(1000);
            this.Copy_Btn.Content = "Copy";
        }

        private void BtnStatusChange(Button button, Visibility visibility)
        {
            button.Visibility = visibility;
            button.Visibility = Visibility.Collapsed;
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
