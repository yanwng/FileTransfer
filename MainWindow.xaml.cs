using FileTransfer.entity;
using FileTransfer.views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

namespace FileTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //保存本地收发文件的文件信息
        public ObservableCollection<FileEntity> Files { get; set; } = new ObservableCollection<FileEntity>();

        //应用设置信息
        public AppSettings Settings { get; set; } = AppSettings.GetAppSettings();

        //发送给对象的连接
        Socket? client;


        //接受文件的连接
        Socket master = new Socket(SocketType.Stream,ProtocolType.Tcp);
        Socket? server;
        //选择的文件
        FileEntity? SelectFileEntity;

        //是否在发送文件中
        bool Sending;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Settings;
            ls.DataContext = this;
            //接受连接
            master.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9090));
            master.Listen();

            Task.Run(async () =>
            {
                /* 
                 server = await master.AcceptAsync();

                 await Task.Run(async () =>
                 {
                     while (true)
                     {
                         if (server.Poll(0, SelectMode.SelectRead) && server.Available == 0)
                         {
                             server = null;
                             await Task.Run(async () =>
                             {
                                 server = await master.AcceptAsync();
                             });
                         }
                         await Task.Delay(500);
                     }


                 });
                */

                do
                {
                    server = await master.AcceptAsync();
                    CancellationTokenSource cancellation = new CancellationTokenSource();
                    _=Task.Run(async () => {

                        while (!cancellation.Token.IsCancellationRequested)
                        {
                            try
                            {
                                if(server!=null)
                                await Receive();
                            }
                            catch (Exception e)
                            {

                                MessageBox.Show(e.Message);
                            }
                            
                            await Task.Delay(500);
                        }
                    
                    },cancellation.Token);
                    while (true)
                    {
                        if (server.Poll(0, SelectMode.SelectRead) && server.Available == 0)
                        {
                            server = null;
                            cancellation.Cancel();
                            break;
                        }
                        
                        await Task.Delay(500);
                    }


                } while (true);
            });

            

        }

        /// <summary>
        /// 接收文件，第一个字节表示文件名长度为short
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        //private void _Receive()
        //{
        //    int count = 1024;
        //    int readLen;
        //    byte[] buffer = new byte[count];
        //    string fileName="";
        //    long fileLen;
        //    bool flag = true;
        //    bool firstEnter = true;
        //    FileStream? fileStream=null;
        //    int reciveCount = 0;
            
        //    do
        //     {
        //       readLen= server.Receive(buffer); //一次只会接收一个方法包
        //        //接收文件大小
        //        if (reciveCount == 0)
        //        {
        //            fileLen = BitConverter.ToInt64(buffer,0);
        //            reciveCount++;
        //            continue;
        //        }
        //        //接受文件名称
        //        if (reciveCount == 1)
        //        {
        //            fileName = Encoding.UTF8.GetString(buffer, 0, readLen);
        //            reciveCount++;
        //            continue;
        //        }

        //        int fileNameCount=1;
        //        while(!File.Exists(Settings.DownLoadFolderPath + "\\" + fileName)&&firstEnter)
        //        {
        //            fileName.Insert(fileName.LastIndexOf('.'), $"({fileNameCount++})");
        //            if(!File.Exists(Settings.DownLoadFolderPath + "\\" + fileName))
        //            {
        //                File.Create(Settings.DownLoadFolderPath + "\\" + fileName);
        //                firstEnter = false;
        //                fileStream = new FileStream(Settings.DownLoadFolderPath + "\\" + fileName, FileMode.Append);
        //            }
        //        }

        //        fileStream!.Write(buffer,0,readLen);

        //        if (readLen == 0)
        //        {
        //            reciveCount++;
        //            if (reciveCount == 10)
        //                break;
        //        }

        //    }
        //    while (true);

        //    FileEntity fileEntity = new FileEntity(Settings.DownLoadFolderPath+"\\"+fileName, fileName, false, true);

        //    Files.Add(fileEntity);

        //}

        /// <summary>
        /// 关闭窗口时保存files
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

        }
   
        /// <summary>
        /// 选择并发送文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFile(object sender, MouseButtonEventArgs e)
        {
            if (Sending) return;
            OpenFileDialog fileSelectWindow = new OpenFileDialog();
            fileSelectWindow.Filter = "全部文件*.*|*.*";
            fileSelectWindow.ShowDialog();

            string filePath = fileSelectWindow.FileName;
            string fileName = filePath.Substring(filePath.LastIndexOf('\\')+1,filePath.Length- filePath.LastIndexOf('\\')-1);
            if (filePath != "")
            {
                selectFileBox.Text = fileName;
                //MessageBoxResult result = MessageBox.Show("是否发送文件：" + fileName, "发送提醒", MessageBoxButton.YesNo, MessageBoxImage.Question);

                //if (result == MessageBoxResult.Yes)
                //{
                   // bool isSucess = SendFile(filePath,fileName);
                    SelectFileEntity = new FileEntity(filePath, fileName, true, false);
                    
               //   Files.Add(fileEntity);
                //}
               // selectFileBox.Text = "选择文件";
            }
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<bool> SendFile(string filePath,string fileName)
        {
            //创建TCP连接发送文件
            if (client == null|| SelectFileEntity == null) 
            {
                MessageBox.Show("未与接收端建立连接");
                return false; 
            }

            //int count = 1024;
            //FileStream fileStream = File.Create(filePath);
            //byte[] buffer = new byte[count];
            //while ()
            //{
            //   int readLen =  fileStream.Read(buffer, 0, count);
            //    client.send

            //}
            try
            {
                //前4字节为文件名长度，后接文件名，后8字节为文件总字节长度
                FileInfo fileInfo = new FileInfo(filePath);
                SelectFileEntity.FileSize = fileInfo.Length;
                byte[] fileSize = BitConverter.GetBytes(fileInfo.Length);
                byte[] filename = Encoding.UTF8.GetBytes(fileName);
                //发送文件名字节长度
                client.Send(BitConverter.GetBytes(filename.Length));
                //发送文件名
                client.Send(filename);
                //发送文件大小
                client.Send(fileSize);
                //发送文件
                using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[30*1024];
                    int readLen = 0;
                    long totalCount = 0;
                    Process(SelectFileEntity);
                    while (totalCount<SelectFileEntity.FileSize)
                    {
                        readLen= stream.Read(buffer);
                        await client.SendAsync(buffer);
                        totalCount += readLen;
                        SelectFileEntity.SendSize = totalCount;

                    }
                    SelectFileEntity.IsSucess = true;
                }
                //client.SendFile(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 接收文件，前4字节为文件名长度，后接文件名，后8字节为文件总字节长度，最后接文件体
        /// </summary>
        private async Task Receive()
        {
            //接收文件名长度
            byte[] fileLenBytes = new byte[4];
            await server!.ReceiveAsync(fileLenBytes);
            int fileLen = BitConverter.ToInt32(fileLenBytes, 0);

            //接收文件名
            byte[] fileNameBytes = new byte[fileLen];
            await server.ReceiveAsync(fileNameBytes);
            string fileName = Encoding.UTF8.GetString(fileNameBytes);

            //接受文件总字节大小
            byte[] fileSizeBytes = new byte[8];
            await server.ReceiveAsync(fileSizeBytes);
            long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

            FileEntity fileEntity = new FileEntity(Settings.DownLoadFolderPath + "\\" + fileName, fileName, false, false) ;
            fileEntity.FileSize= fileSize;

            try
            {
                //创建文件流
                using (FileStream fileStream = new FileStream(Settings.DownLoadFolderPath + "\\" + fileName, FileMode.Create | FileMode.Append))
                {

                    //接收文件体
                    int MaxSize = 30 * 1024;
                    byte[] buffer = new byte[MaxSize]; //1MB
                    long totalCount = 0;
                    int readLen=0;
                    Process(fileEntity);
                    do
                    {
                        readLen = await server.ReceiveAsync(buffer);
                        //fileStream.Write(buffer, 0, readLen);
                        await fileStream.WriteAsync(buffer, 0, readLen);
                        fileStream.Flush(true); //写入数据
                        totalCount+= readLen;
                        lock (fileEntity)
                        {
                            fileEntity.ReceiveSize = totalCount;
                        }
                    }
                    while (readLen == MaxSize);
                    fileEntity.IsSucess = true;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

            Dispatcher.Invoke(() => { Files.Add(fileEntity); });
        }

        /// <summary>
        /// 检测文件收发状态
        /// </summary>
        /// <param name="fileEntity"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Process(FileEntity fileEntity)
        {
            Task.Run(async () => 
            {

                //初始化界面UI
                Dispatcher.Invoke(() =>
                {
                    prog.Visibility = Visibility.Visible; ;
                    prog.Maximum = fileEntity.FileSize;
                });

                while (true)
                {
                    //更新界面UI
                    Dispatcher.Invoke(() =>
                    {
                        prog.Value = fileEntity.IsSend ? fileEntity.SendSize : fileEntity.ReceiveSize;
                    });

                    await Task.Delay(20);

                    
                        if (fileEntity.SendSize >= fileEntity.FileSize||fileEntity.ReceiveSize>=fileEntity.FileSize)
                        {
                            //更新界面UI
                            Dispatcher.Invoke(() =>
                            {
                                prog.Value = fileEntity.IsSend ? fileEntity.SendSize : fileEntity.ReceiveSize;
                            });
                            break;
                        }
                    
                }

                await Task.Delay(3000);
                Dispatcher.Invoke(() =>
                {
                    prog.Visibility = Visibility.Collapsed;
                   
                });

            });
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ChangeDownLoadPlace(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolder = new OpenFolderDialog();
            openFolder.ShowDialog();
            Settings.DownLoadFolderPath =  openFolder.FolderName;
        }

        private void CreateConnection(object sender, MouseButtonEventArgs e)
        {
            if (client != null) return;
            ConnectiionView connectiionView = new ConnectiionView();
            connectiionView.Owner=this;
            connectiionView.ShowDialog();

            try
            {
                client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.Connect(connectiionView.EndPoint);

                //检测连接是否断开
                Task.Run(async() =>
                {
                    while (true)
                    {
                        if (client.Poll(0, SelectMode.SelectRead) && client.Available == 0)
                        {
                            client = null;
                            
                            break;
                        }

                        await Task.Delay(500);
                    }
                });


                ConnectRect.Fill = new SolidColorBrush(Colors.Green);
                ConnectText.Text = "已连接";

                Task.Run(async() =>
                {
                    while (true)
                    {
                        if (client!=null&&client.Poll(0, SelectMode.SelectRead) && client.Available == 0)
                        {
                            client = null;
                            Dispatcher.Invoke(() =>
                            {
                                ConnectRect.Fill = new SolidColorBrush(Colors.Red);
                                ConnectText.Text = "未连接";
                            });
                            break;
                        }

                        await Task.Delay(500);
                    }

                });

            }
            catch (Exception)
            {
                client = null;
                MessageBox.Show("连接错误");
            }

        }

        private void SendFileConfirm(object sender, RoutedEventArgs e)
        {
            if (SelectFileEntity != null)
            {

                Task.Run(()=> 
                {
                    Sending = true;
                     

                    return SendFile(SelectFileEntity.FilePath, SelectFileEntity.FileName); 
                
                }).ContinueWith(async isSucess=>
                {
                    bool _isSucess = await isSucess;
                    Dispatcher.Invoke(() => 
                    {
                        SelectFileEntity.SendSucess = _isSucess;
                        Files.Add(SelectFileEntity);
                        Sending = false;
                        SelectFileEntity = null;
                        selectFileBox.Text = "点击选择文件";
                    });
                    
                });
                selectFileBox.Text = "文件传输中...";
            }
        }
    }
}