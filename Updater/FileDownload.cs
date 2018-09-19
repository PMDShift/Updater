// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

namespace PMDCP.Updater
{
    using PMDCP.Updater.Linker;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;

    public class FileDownload : IFileDownload
    {
        #region Fields

        private BackgroundWorker downloadBWorker;
        private readonly string downloadURI;
        private readonly string filePath;

        #endregion Fields

        #region Constructors

        public FileDownload(string downloadURI, string filePath)
        {
            this.downloadURI = downloadURI;
            this.filePath = filePath;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<FileDownloadingEventArgs> DownloadComplete;

        public event EventHandler<FileDownloadingEventArgs> DownloadUpdate;

        #endregion Events

        #region Methods

        public void Download()
        {
            downloadBWorker = new BackgroundWorker();
            downloadBWorker.DoWork += new DoWorkEventHandler(DownloadBWorker_DoWork);
            downloadBWorker.WorkerReportsProgress = true;
            downloadBWorker.ProgressChanged += new ProgressChangedEventHandler(DownloadBWorker_ProgressChanged);
            FileStream stream = new FileStream(filePath + ".tmp", FileMode.Create, FileAccess.Write);
            downloadBWorker.RunWorkerAsync(new object[] { downloadURI, stream });

            //WebClient webClient = new WebClient();
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            //webClient.DownloadFileAsync(new Uri(downloadURI), filePath + ".tmp");
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.Move(filePath + ".tmp", filePath);

            DownloadComplete?.Invoke(this, new FileDownloadingEventArgs(0, filePath, 100, 0));
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine(e.BytesReceived + "/" + e.TotalBytesToReceive);
#endif
            DownloadUpdate?.Invoke(this, new FileDownloadingEventArgs(e.TotalBytesToReceive, filePath, e.ProgressPercentage, e.BytesReceived));
        }

        private void DownloadBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool downloadComplete = false;

            long length = 0;

            string downloadPath = ((object[])e.Argument)[0] as string;
            using (Stream stream = ((object[])e.Argument)[1] as Stream)
            {
                try
                {
                    HttpWebResponse theResponse;
                    HttpWebRequest theRequest;
                    //Checks if the file exist

                    try
                    {
                        theRequest = (HttpWebRequest)WebRequest.Create(downloadPath);
                        theResponse = (HttpWebResponse)theRequest.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        downloadBWorker.ReportProgress(0, new object[] { "error", ex });
                        return;
                    }
                    length = theResponse.ContentLength;
                    //Size of the response (in bytes)

                    //FileStream writeStream = new FileStream(filePath + ".tmp", FileMode.Create);

                    //Replacement for Stream.Position (webResponse stream doesn't support seek)
                    long nRead = 0;

                    do
                    {
                        byte[] readBytes = new byte[1024];
                        int bytesread = theResponse.GetResponseStream().Read(readBytes, 0, 1024);

                        nRead += bytesread;

                        if (bytesread == 0)
                            break;

                        int percent = (int)Updater.Math.CalculatePercent(nRead, length);
                        stream.Write(readBytes, 0, bytesread);
                        if (DownloadUpdate != null)
                        {
                            downloadBWorker.ReportProgress(percent, new object[] { "downloading", new FileDownloadingEventArgs(length, "", percent, nRead) });
                        }
                    }
                    while (true);

                    //Close the streams
                    theResponse.GetResponseStream().Close();

                    downloadComplete = true;
                }
                catch (Exception ex)
                {
                    downloadBWorker.ReportProgress(0, new object[] { "error", ex });
                }
            }

            if (downloadComplete)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(filePath + ".tmp", filePath);

                if (DownloadComplete != null)
                {
                    downloadBWorker.ReportProgress(100, new object[] { "done", new FileDownloadingEventArgs(length, "", 100, length) });
                }
            }
        }

        private void DownloadBWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] data = e.UserState as object[];
            switch ((string)data[0])
            {
                case "downloading":
                    {
                        FileDownloadingEventArgs downloadInfo = data[1] as FileDownloadingEventArgs;
                        DownloadUpdate?.Invoke(this, downloadInfo);
                    }
                    break;

                case "done":
                    {
                        FileDownloadingEventArgs downloadInfo = data[1] as FileDownloadingEventArgs;
                        DownloadComplete?.Invoke(this, downloadInfo);
                    }
                    break;

                case "error":
                    {
                        if (data[1] is Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.ToString());
                        }
                    }
                    break;
            }
        }

        #endregion Methods
    }
}