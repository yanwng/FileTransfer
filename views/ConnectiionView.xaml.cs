using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileTransfer.views
{
    /// <summary>
    /// ConnectiionView.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectiionView : Window
    {

        public IPEndPoint EndPoint  { get; set; }

        public ConnectiionView()
        {
            InitializeComponent();
        }

        private void connection(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(IPStr.Text);
                int port = int.Parse(PortStr.Text);
                EndPoint= new IPEndPoint(ip,port);
                this.Close();
            }
            catch (Exception )
            {

                MessageBox.Show("IP或Port错误");
                IPStr.Text= string.Empty;
                PortStr.Text= string.Empty;
            }
        }

    }
}
