using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace FileTransfer.entity
{
    [Serializable]
    public class AppSettings:INotifyPropertyChanged
    {
        #region 配置文件读取保存等基本功能，修改配置会自动保存，也可手动保存
        private delegate bool EventHandle();
        //当配置信息被修改后触发该事件
        private  event EventHandle? SettingsChange;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static string ConfigDir = "C:/ProgramData/myDir";

        private void Notify([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private static AppSettings? appSettings;

        public AppSettings() {  }

        public static AppSettings GetAppSettings(string path="setting") 
        {
            if (appSettings == null)
            {
                //读取序列化文件
                if (File.Exists(ConfigDir + "/config.json")) 
                {
                   
                        try
                        {
                            path = ConfigDir + "/config.json";
                        string json ="";

                        using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            int readLen;
                            byte[] buffer= new byte[1024];
                            do
                            {
                                readLen = stream.Read(buffer);
                                json+= Encoding.UTF8.GetString(buffer,0,readLen);
                            }
                            while (readLen == buffer.Length);
                        }
                        appSettings = JsonSerializer.Deserialize<AppSettings>(json);
                        appSettings.SettingsChange+= appSettings.SaveSettings;
                    }
                        catch (Exception e)
                        {
                            int i = 0;
                        
                        }
                        
                    
                }
                else
                {
                    
                    using(FileStream file = new FileStream(ConfigDir + "/config.json", FileMode.Create|FileMode.Append))
                    {
                        appSettings = new AppSettings();
                        JsonSerializer.Serialize(file, appSettings);
                    }
                }

                //创建下载文件的文件夹
                if (!Directory.Exists(appSettings!.downLoadFolderPath))
                {
                    Directory.CreateDirectory(appSettings.downLoadFolderPath);
                }
                LoadSettingsFile(path,appSettings);

            }

            return appSettings; 
        }

        /// <summary>
        /// 加载App配置信息或同步信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool LoadSettingsFile(string path, AppSettings appSettings)
        {
            if (appSettings.downLoadFolderPath == null) 
            {
                appSettings.downLoadFolderPath = ConfigDir;                
            }
            return true;
        }

        /// <summary>
        /// 序列化该配置
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveSettings()
        {
            File.Delete(ConfigDir + "/config.json");
            using (FileStream file = new FileStream(ConfigDir + "/config.json", FileMode.Create | FileMode.Append))
            {
                JsonSerializer.Serialize(file, appSettings);
            }
            return true;
        }
        #endregion

        #region 配置项
        private string downLoadFolderPath;
        public string DownLoadFolderPath
        {
            get { return downLoadFolderPath; }
            set { downLoadFolderPath = value; SettingsChange?.Invoke(); Notify(); }
        }

        #endregion 

    }
}
