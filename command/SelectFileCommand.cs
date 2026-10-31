using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace FileTransfer.command
{
    /// <summary>
    /// 选择需要发送的文件
    /// </summary>
    public class SelectFileCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public delegate bool SelectedFile(string filePath);
        public event SelectedFile? SelectedFileEvent;


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            OpenFileDialog fileSelectWindow= new OpenFileDialog();
            fileSelectWindow.Filter = "全部文件*.*|*.*";
            fileSelectWindow.ShowDialog();
            

            SelectedFileEvent?.Invoke("");
        }
    }
}
