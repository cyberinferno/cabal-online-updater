using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ClientUpdater
{
    public partial class Updater : Form
    {
        private string PatchHost = "http://127.0.0.1/patch";
        private string WebHost = "http://127.0.0.1";
        public string Patch = "Patch.ini";
        public string currentDirectory = Directory.GetCurrentDirectory();
        public string mainFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "CabalMain.exe";
        public string updatesDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "updates";
        public WebClient webClient;
        public Stopwatch sw = new Stopwatch();
        public bool isBulkDownload = false;
        public string lastDownloadedFilePath;
        public string lastDownloadedRemoteUrl;
        public Queue<string> fileProcessQueue;
        public int totalFilesToBeProcessed = 0;
        public int totalFilesProcessed = 0;

        private Stream strResponse;
        private Stream fileStream;
        private HttpWebRequest webRequest;
        private HttpWebResponse webResponse;

        public Updater()
        {
            InitializeComponent();
            updaterWorker.WorkerReportsProgress = true;
            updaterWorker.WorkerSupportsCancellation = true;
        }

        #region Check For Instance

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        private static bool IsAlreadyRunning()
        {
            // get all processes by Current Process name
            Process[] processes =
                Process.GetProcessesByName(
                    Process.GetCurrentProcess().ProcessName);

            // if there is more than one process...
            if (processes.Length > 1)
            {
                // if other process id is OUR process ID...
                // then the other process is at index 1
                // otherwise other process is at index 0
                int n = (processes[0].Id == Process.GetCurrentProcess().Id) ? 1 : 0;

                // get the window handle
                IntPtr hWnd = processes[n].MainWindowHandle;

                // if iconic, we need to restore the window
                if (IsIconic(hWnd)) ShowWindowAsync(hWnd, SW_RESTORE);

                // Bring it to the foreground
                SetForegroundWindow(hWnd);
                return true;
            }
            return false;
        }

        public void CloseAll()
        {
            try
            {
                if (webClient.IsBusy)
                {
                    webClient.CancelAsync();
                }
                webClient.Dispose();
                if (updaterWorker.IsBusy)
                {
                    updaterWorker.CancelAsync();
                }
                updaterWorker.Dispose();
                sw.Reset();
                if (strResponse != null)
                {
                    strResponse.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                ResetUIElements();
            }
            catch { }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            if (IsAlreadyRunning())
            {
                MessageBox.Show("Updater is already running", "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void Updater_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseAll();
            if (Directory.Exists(updatesDirectory))
            {
                Directory.Delete(updatesDirectory, true);
            }
        }

        private void Updater_Shown(object sender, EventArgs e)
        {
            CloseButton.Refresh();
            DisableFullCheckButton();
            DisableStartGameButton();
            DisableStopButton();
            if (!File.Exists(mainFile))
            {
                DoFullCheck();
            }
            else
            {
                DoVersionCheck();
            }
        }

        private void updaterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            TotalProgBar.Width = 0;
            while (fileProcessQueue.Count > 0)
            {
                CurrentProgBar.Width = 0;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                var fileData = fileProcessQueue.Dequeue();
                totalFilesProcessed++;
                var percentage = (int)Math.Round((double)(100 * totalFilesProcessed) / totalFilesToBeProcessed);
                updaterWorker.ReportProgress(percentage);
                var fileDataArray = fileData.Split('|');
                if (fileDataArray.Length < 3)
                {
                    continue;
                }
                Invoke((MethodInvoker)delegate
                {
                    SpeedLable.Text = "";
                    percentLable.Text = "";
                    StatusLable.Text = "Checking " + totalFilesProcessed + "/" + totalFilesToBeProcessed + " files...";
                });
                var localPathToCheck = currentDirectory + fileDataArray[1];
                var pathToDownload = updatesDirectory + fileDataArray[1] + ".7z";
                if (!File.Exists(currentDirectory + fileDataArray[1]) || GetFileMD5Hash(currentDirectory + fileDataArray[1]) != fileDataArray[0])
                {
                    Invoke((MethodInvoker)delegate
                    {
                        StatusLable.Text = "Downloading " + FileNameFromPath(localPathToCheck);
                    });
                    Directory.CreateDirectory(Path.GetDirectoryName(pathToDownload));
                    try
                    {
                        // Create a request to the file we are downloading
                        webRequest = (HttpWebRequest)WebRequest.Create(PatchHost + fileDataArray[2]);
                        // Set the starting point of the request
                        webRequest.AddRange(0);
                        // Set default authentication for retrieving the file
                        webRequest.Credentials = CredentialCache.DefaultCredentials;
                        // Retrieve the response from the server
                        webResponse = (HttpWebResponse)webRequest.GetResponse();
                        // Ask the server for the file size and store it
                        Int64 fileSize = webResponse.ContentLength;
                        // Start the stopwatch which we will be using to calculate the download speed
                        sw.Start();
                        // Open the URL for download
                        strResponse = webResponse.GetResponseStream();
                        // Create a new file stream where we will be saving the data (local drive)
                        // Read from response and write to file
                        fileStream = new FileStream(pathToDownload, FileMode.Create, FileAccess.Write, FileShare.None);
                        // It will store the current number of bytes we retrieved from the server
                        int bytesSize = 0;
                        // A buffer for storing and writing the data retrieved from the server
                        byte[] downBuffer = new byte[2048];

                        // Loop through the buffer until the buffer is empty
                        while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) != 0)
                        {
                            fileStream.Write(downBuffer, 0, bytesSize);
                            // Invoke a method to update form's label and progress bar
                            Invoke((MethodInvoker)delegate
                            {
                                // Calculate the download progress in percentages
                                var PercentProgress = Convert.ToInt32((fileStream.Length * 100) / fileSize);
                                // Make progress on the progress bar
                                CurrentProgBar.Width = Convert.ToInt32(Math.Round(PercentProgress / 100.0 * 374.0));
                                // Display the current progress on the form
                                percentLable.Text = PercentProgress + "%";
                                SpeedLable.Text = (Convert.ToDouble(fileStream.Length) / 1024 / sw.Elapsed.TotalSeconds).ToString("0.00") + " kb/s";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        sw.Reset();
                        strResponse.Close();
                        fileStream.Close();
                        var decompressedFile = pathToDownload.Remove(pathToDownload.LastIndexOf(".7z"));
                        DecompressFileLZMA(pathToDownload, decompressedFile);
                        File.Delete(pathToDownload);
                        if (!File.Exists(currentDirectory + fileDataArray[1]))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(currentDirectory + fileDataArray[1]));
                        }
                        else
                        {
                            File.Delete(currentDirectory + fileDataArray[1]);
                        }
                        File.Move(decompressedFile, currentDirectory + fileDataArray[1]);
                    }
                }
            }
        }

        // This event handler updates the progress.
        private void updaterWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TotalProgBar.Width = Convert.ToInt32(Math.Round(e.ProgressPercentage / 100.0 * 374.0));
        }

        private void updaterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ResetUIElements();
            DisableStopButton();
            EnableFullCheckButton();
            EnableStartGameButton();
            if (e.Cancelled == true)
            {
                StatusLable.Text = "File check was manually stopped";
            }
            else if (e.Error != null)
            {
                StatusLable.Text = e.Error.Message;
                MessageBox.Show(e.Error.Message, "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StatusLable.Text = "All files are up to date. Have fun in game!";
            }
            clearDirectory(updatesDirectory);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private int mouseStartX, mouseStartY;
        private int formStartX, formStartY;
        private bool FormDragging = false;

        private void Updater_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseStartX = MousePosition.X;
            this.mouseStartY = MousePosition.Y;
            this.formStartX = this.Location.X;
            this.formStartY = this.Location.Y;
            FormDragging = true;
        }

        private void Updater_MouseMove(object sender, MouseEventArgs e)
        {
            if (FormDragging)
            {
                this.Location = new Point(
                this.formStartX + MousePosition.X - this.mouseStartX,
                this.formStartY + MousePosition.Y - this.mouseStartY
                );
            }
        }

        private void Updater_MouseUp(object sender, MouseEventArgs e)
        {
            FormDragging = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/register");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/resend-verification-email");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/request-password-reset");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/webshop");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/login");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/download");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebHost + "/news");
        }

        #endregion Check For Instance

        private void ResetUIElements()
        {
            StatusLable.Text = "";
            DisableStopButton();
            EnableFullCheckButton();
            SpeedLable.Text = "";
            percentLable.Text = "";
            TotalLable.Text = "";
            TotalProgBar.Width = 374;
            CurrentProgBar.Width = 374;
        }

        private string GetFileMD5Hash(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        private void clearDirectory(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DecompressFileLZMA(string source, string destination)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(source, FileMode.Open);
            FileStream output = new FileStream(destination, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Flush();
            input.Close();
        }

        private void EnableStopButton()
        {
            StopButton.Enabled = true;
            StopButton.Cursor = Cursors.Hand;
        }

        private void DisableStopButton()
        {
            StopButton.Enabled = false;
            StopButton.Cursor = Cursors.No;
        }

        private void EnableFullCheckButton()
        {
            FullCheckButton.Enabled = true;
            FullCheckButton.Cursor = Cursors.Hand;
        }

        private void DisableFullCheckButton()
        {
            FullCheckButton.Enabled = false;
            FullCheckButton.Cursor = Cursors.No;
        }

        private void EnableStartGameButton()
        {
            startGameBtn.Enabled = true;
            startGameBtn.Cursor = Cursors.Hand;
        }

        private void DisableStartGameButton()
        {
            startGameBtn.Enabled = false;
            startGameBtn.Cursor = Cursors.No;
        }

        private void DoFullCheck()
        {
            if (Directory.Exists(updatesDirectory))
            {
                clearDirectory(updatesDirectory);
            }
            else
            {
                Directory.CreateDirectory(updatesDirectory);
            }
            StatusLable.Text = "Doing a full check...";
            totalFilesProcessed = 0;
            totalFilesToBeProcessed = 0;
            DisableFullCheckButton();
            DisableStartGameButton();
            EnableStopButton();
            DownloadFile("/full.ini", updatesDirectory + Path.DirectorySeparatorChar + "full.ini", OnDownloadProgressChanged, OnIniDownloadCompleted);
        }

        private void DoVersionCheck()
        {
            if (Directory.Exists(updatesDirectory))
            {
                clearDirectory(updatesDirectory);
            }
            else
            {
                Directory.CreateDirectory(updatesDirectory);
            }
            StatusLable.Text = "Checking game version...";
            EnableStopButton();
            DownloadFile("/patch.ini", updatesDirectory + Path.DirectorySeparatorChar + "patch.ini", OnDownloadProgressChanged, OnIniDownloadCompleted);
        }

        private void DownloadFile(string urlAddress, string location, DownloadProgressChangedEventHandler downloadProgressCallback, AsyncCompletedEventHandler downloadCompleteCallback)
        {
            lastDownloadedFilePath = location;
            lastDownloadedRemoteUrl = urlAddress;
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompleteCallback);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgressCallback);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri(PatchHost + urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    StatusLable.Text = ex.Message;
                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and output it to labelSpeed.
            SpeedLable.Text = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            CurrentProgBar.Width = Convert.ToInt32(Math.Round(e.ProgressPercentage / 100.0 * 374.0));

            // Show the percentage on our label.
            percentLable.Text = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            /**labelDownloaded.Text = string.Format("{0} MB's / {1} MB's",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));**/
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            CloseAll();
            if (Directory.Exists(updatesDirectory))
            {
                try
                {
                    Directory.Delete(updatesDirectory, true);
                }
                catch { }
            }
            Environment.Exit(0);
        }

        private void FullCheckButton_Click(object sender, EventArgs e)
        {
            if (!webClient.IsBusy && !updaterWorker.IsBusy)
            {
                DoFullCheck();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
            }
            if (updaterWorker.IsBusy)
            {
                updaterWorker.CancelAsync();
            }
            updaterWorker.Dispose();
            sw.Reset();
            strResponse.Close();
            fileStream.Close();
            DisableStopButton();
            EnableFullCheckButton();
            EnableStartGameButton();
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainFile))
            {
                Process.Start(mainFile, "jimmy");
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("Game client was not found. Please do a full check to fix.", "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnIniDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            sw.Reset();
            DisableStopButton();
            EnableFullCheckButton();
            DisableStartGameButton();
            SpeedLable.Text = "";
            percentLable.Text = "";
            if (e.Cancelled == true)
            {
                EnableStartGameButton();
                StatusLable.Text = "File check was manually stopped";
            }
            else if (e.Error != null)
            {
                EnableStartGameButton();
                StatusLable.Text = e.Error.Message;
                MessageBox.Show(e.Error.Message, "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var iniFile = new StreamReader(lastDownloadedFilePath);
                var iniFileData = iniFile.ReadToEnd();
                iniFile.Close();
                fileProcessQueue = new Queue<string>(iniFileData.Split(';'));
                BulkFileProcess();
            }
        }

        private void OnVersionDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();
            DisableStopButton();
            DisableStartGameButton();
            SpeedLable.Text = "";
            percentLable.Text = "";
            if (e.Cancelled == true)
            {
                EnableStartGameButton();
                StatusLable.Text = "Version check was manually stopped";
            }
            else if (e.Error != null)
            {
                EnableStartGameButton();
                StatusLable.Text = e.Error.Message;
                MessageBox.Show(e.Error.Message, "Cabal Online Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DownloadFile("/patch.ini", updatesDirectory + Path.DirectorySeparatorChar + "patch.ini", OnDownloadProgressChanged, OnIniDownloadCompleted);
            }
        }

        private void BulkFileProcess()
        {
            totalFilesToBeProcessed = fileProcessQueue.Count;
            StatusLable.Text = "Total " + totalFilesToBeProcessed + " files to process";
            DisableFullCheckButton();
            DisableStartGameButton();
            EnableStopButton();
            if (!updaterWorker.IsBusy)
            {
                updaterWorker.RunWorkerAsync();
            }
        }

        private string FileNameFromPath(string path)
        {
            var pathArray = path.Split('\\');
            return pathArray[pathArray.Length - 1];
        }

        private void MoveDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));
            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                    File.Move(file, targetFile);
                }
                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
            Directory.Delete(source, true);
        }
    }
}