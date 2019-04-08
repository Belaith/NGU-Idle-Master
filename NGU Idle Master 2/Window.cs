using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Tesseract;
using System.Numerics;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace NGU_Idle_Master
{
    public struct RECT
    {
        public int Left;       // Specifies the x-coordinate of the upper-left corner of the rectangle.
        public int Top;        // Specifies the y-coordinate of the upper-left corner of the rectangle.
        public int Right;      // Specifies the x-coordinate of the lower-right corner of the rectangle.
        public int Bottom;     // Specifies the y-coordinate of the lower-right corner of the rectangle.
    }

    public enum WMessages : int
    {
        WM_KEYDOWN = 0x100,
        WM_KEYUP = 0x101,

        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14,
        VK_SHIFT = 16,
        VK_CONTROL = 17,
        VK_MENU = 18,
        WM_MOUSEMOVE = 512,
        WM_LBUTTONDOWN = 513,
        WM_LBUTTONUP = 514,
        WM_RBUTTONDOWN = 516,
        WM_RBUTTONUP = 517
    }

    public class BigIntegerFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(NumberFormatInfo))
            {
                NumberFormatInfo numberFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                numberFormat.NumberGroupSeparator = ",";
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 1;
                return numberFormat;
            }
            else
            {
                return null;
            }
        }
    }

    static class NGUIdleMasterWindowConstants
    {
        #region rects

        public static readonly RECT rectBase = new RECT()
        {
            Left = 0,
            Top = 0,
            Right = 1000,
            Bottom = 1000
        };

        #endregion

        #region points

        public static readonly Point pointMouseMove = new Point()
        {
            X = 958,
            Y = 17
        };

        public static readonly Point pointInput = new Point()
        {
            X = 440,
            Y = 74
        };

        public static readonly Point pointHalfCapEnergy = new Point()
        {
            X = 512,
            Y = 29
        };

        public static readonly Point pointQuarterCapEnergy = new Point()
        {
            X = 541,
            Y = 29
        };

        public static readonly Point pointHalfIdleEnergy = new Point()
        {
            X = 512,
            Y = 55
        };

        public static readonly Point pointQuarterIdleEnergy = new Point()
        {
            X = 541,
            Y = 55
        };

        public static readonly Point pointFullEnergy = new Point()
        {
            X = 522,
            Y = 77
        };

        public static readonly Point pointPercentageIdleEnergy = new Point()
        {
            X = 818,
            Y = 77
        };

        public static readonly Point pointHalfCapMagic = new Point()
        {
            X = 655,
            Y = 29
        };

        public static readonly Point pointQuarterCapMagic = new Point()
        {
            X = 684,
            Y = 29
        };

        public static readonly Point pointHalfIdleMagic = new Point()
        {
            X = 655,
            Y = 55
        };

        public static readonly Point pointQuarterIdleMagic = new Point()
        {
            X = 684,
            Y = 55
        };

        public static readonly Point pointFullMagic = new Point()
        {
            X = 670,
            Y = 78
        };

        public static readonly Point pointPercentageIdleMagic = new Point()
        {
            X = 848,
            Y = 77
        };

        #endregion
    }

    public class NGUIdleMasterWindow : IDisposable
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(AddHawndDelegate dg, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        public static extern byte VkKeyScan(char ch);

        const NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands;

        public bool Locked { get { return lockedSession || (lockedDisplay && !System.Windows.Forms.SystemInformation.TerminalServerSession) || noOcr; } }
        private bool lockedSession = false;
        private bool lockedDisplay = false;
        private bool noOcr = false;

        private bool initilialized = false;
        private IntPtr id;
        private Point point;

        private List<IntPtr> hwnds;

        private BigIntegerFormatProvider numberFormatProvider;
        private TesseractEngine ocr;
        private StreamWriter streamWriter;

        private Bitmap bitmap;
        private bool needReloadBmp = true;

        private DummyWindowForPowerBroadcast dummyWindow;

        private bool is_disposed = false;

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock && lockedSession == false)
            {
                lockedSession = true;
                Log("SessionLocked - " + (Locked ? "Locked" : "Unlocked"));
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock && lockedSession == true)
            {
                lockedSession = false;
                Log("SessionUnlocked - " + (Locked ? "Locked" : "Unlocked"));
            }
        }

        void SystemEvents_DisplaySwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock && lockedDisplay == false)
            {
                lockedDisplay = true;
                Log("DisplayLocked - " + (Locked ? "Locked" : "Unlocked"));
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock && lockedDisplay == true)
            {
                lockedDisplay = false;
                Log("DisplayUnlocked - " + (Locked ? "Locked" : "Unlocked"));
            }
        }

        public delegate bool AddHawndDelegate(int hwnd, int lParam);

        public NGUIdleMasterWindow(string tesseractPath, string logPath)
        {
            this.numberFormatProvider = new BigIntegerFormatProvider();

            if (!string.IsNullOrWhiteSpace(tesseractPath))
            {
                this.ocr = new TesseractEngine(tesseractPath, "eng", EngineMode.Default);
            }
            else
            {
                noOcr = true;
            }

            if (!string.IsNullOrWhiteSpace(logPath))
            {
                int tries = 10;
                while (this.streamWriter == null && tries > 0)
                {
                    try
                    {
                        this.streamWriter = new StreamWriter(logPath + @"\NDU_Idle_Master.log", true);
                    }
                    catch
                    {
                        tries--;
                        Thread.Sleep(1000);
                    }
                }

                this.streamWriter.AutoFlush = true;
            }

            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            Initialize();
            
            Thread dummyWindowThread = new Thread(new ThreadStart(() =>
            {
                // create and show the window
                dummyWindow = new DummyWindowForPowerBroadcast();

                dummyWindow.OnSessionSwitch += new DummyWindowForPowerBroadcast.SessionSwitchHandler(SystemEvents_DisplaySwitch);

                // start the Dispatcher processing  
                System.Windows.Threading.Dispatcher.Run();
            }));
            dummyWindowThread.SetApartmentState(ApartmentState.STA);
            dummyWindowThread.IsBackground = true;
            dummyWindowThread.Start();
        }

        ~NGUIdleMasterWindow()
        {
            Dispose(false);
            Console.WriteLine("In destructor.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!is_disposed)
            {
                if (disposing)
                {
                    ocr?.Dispose();
                    ocr = null;
                    streamWriter?.Dispose();
                    streamWriter = null;
                }

                this.is_disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            Log($"Initialize");
            initilialized = false;
            hwnds = new List<IntPtr>();
            List<IntPtr> ids = new List<IntPtr>();

            IntPtr tempId = FindWindow(null, "play ngu idle, a free online game on kongregate");

            if (tempId != null && tempId != new IntPtr(0x00000000))
            {
                ids.Add(tempId);
            }

            AddHawndDelegate myCallBack = new AddHawndDelegate(addHawndCallback);
            EnumWindows(myCallBack, IntPtr.Zero);

            foreach (IntPtr hwnd in hwnds)
            {
                StringBuilder sb = new StringBuilder();
                try
                {
                    int length = GetWindowTextLength((IntPtr)hwnd);
                    sb = new StringBuilder(length + 1);
                    GetWindowText((IntPtr)hwnd, sb, sb.Capacity);
                }
                catch { }
                if (sb.ToString().ToLower().Contains("play ngu idle"))
                {
                    ids.Add((IntPtr)hwnd);
                }
            }

            Point point = new Point();
            foreach (IntPtr id in ids.Distinct().ToList())
            {
                if (PixelSearch(out point, Color.FromArgb(0,4,8), NGUIdleMasterWindowConstants.rectBase, true, id))
                {
                    Log($"{id,8} - Found - Point.X: {point.X} - Point.Y: {point.Y}");
                    this.id = id;
                    this.point = point;
                    initilialized = true;
                    break;
                }
                else
                {
                    Log($"{id, 8} - Not Found");
                }
            }

            Log($"Point.X: {point.X} - Point.Y: {point.Y}");
        }

        public BigInteger Parse(string input)
        {
            input = input.Replace(" ", string.Empty);

            while (!Char.IsDigit((input[0])))
            {
                input = input.Substring(1);
            }

            while (!Char.IsDigit((input[input.Length - 1])))
            {
                input = input.Substring(0, input.Length - 1);
            }

            if (input.Contains("."))
            {
                input = input.Substring(0, input.IndexOf("."));
            }

            return BigInteger.Parse(input, numberStyle, numberFormatProvider);
        }

        public bool addHawndCallback(int hwnd, int lParam)
        {
            hwnds.Add((IntPtr)hwnd);

            return true;
        }

        public void Click(Point point, bool right, bool needReloadBmp)
        {
            if (!initilialized)
            {
                return;
            }

            if (needReloadBmp)
            {
                this.needReloadBmp = needReloadBmp;
            }

            point.X += this.point.X;
            point.Y += this.point.Y;

            SendMessage(id, (int)WMessages.WM_MOUSEMOVE, (IntPtr)0x1, (IntPtr)((point.Y << 16) | (point.X & 0xFFFF)));

            Thread.Sleep(20);

            while (GetKeyState((int)WMessages.VK_CONTROL) < 0 || GetKeyState((int)WMessages.VK_SHIFT) < 0 || GetKeyState((int)WMessages.VK_MENU) < 0)
            {
                Thread.Sleep(5);
            }

            if (right)
            {
                SendMessage(id, (int)WMessages.WM_RBUTTONDOWN, (IntPtr)0x1, (IntPtr)((point.Y << 16) | (point.X & 0xFFFF)));
                Thread.Sleep(20);
                SendMessage(id, (int)WMessages.WM_RBUTTONUP, (IntPtr)0x1, (IntPtr)((point.Y << 16) | (point.X & 0xFFFF)));
            }
            else
            {
                SendMessage(id, (int)WMessages.WM_LBUTTONDOWN, (IntPtr)0x1, (IntPtr)((point.Y << 16) | (point.X & 0xFFFF)));
                Thread.Sleep(20);
                SendMessage(id, (int)WMessages.WM_LBUTTONUP, (IntPtr)0x1, (IntPtr)((point.Y << 16) | (point.X & 0xFFFF)));
            }

            Thread.Sleep(100);
        }

        public void SendString(string str, bool needReloadBmp)
        {
            if (!initilialized)
            {
                return;
            }

            if (needReloadBmp)
            {
                this.needReloadBmp = needReloadBmp;
            }

            for (int i = 0; i < str.Length; i++)
            {
                while (GetKeyState((int)WMessages.VK_CONTROL) < 0 || GetKeyState((int)WMessages.VK_SHIFT) < 0 || GetKeyState((int)WMessages.VK_MENU) < 0)
                {
                    Thread.Sleep(5);
                }

                if (!Char.IsDigit(str[i]))
                {
                    PostMessage(id, (int)WMessages.WM_KEYDOWN, (IntPtr)VkKeyScan(str[i]), (IntPtr)0x1);
                    Thread.Sleep(20);
                }
                PostMessage(id, (int)WMessages.WM_KEYUP, (IntPtr)VkKeyScan(str[i]), (IntPtr)0x1);
                Thread.Sleep(100);
            }
        }

        public Bitmap GetBitmap(bool reloadBmp, IntPtr id)
        {
            if (bitmap == null || needReloadBmp || reloadBmp)
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }

                RECT rect;
                GetWindowRect(id, out rect);

                if (rect.Left < 0)
                {
                    rect.Left = 0;
                }
                if (rect.Top < 0)
                {
                    rect.Top = 0;
                }
                if (rect.Right < 1)
                {
                    rect.Right = 1;
                }
                if (rect.Bottom < 1)
                {
                    rect.Bottom = 1;
                }

                Bitmap bmp = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top, PixelFormat.Format32bppArgb);

                using (Graphics gfxBmp = Graphics.FromImage(bmp))
                {
                    IntPtr hdcBitmap = gfxBmp.GetHdc();

                    PrintWindow(id, hdcBitmap, 0);

                    gfxBmp.ReleaseHdc(hdcBitmap);
                }

                bmp.Save(@"C:\\temp\GetBitmap.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                bitmap = bmp;
                this.needReloadBmp = false;
            }

            return bitmap;
        }

        public Bitmap GetBitmap(bool reloadBmp)
        {
            if (!initilialized)
            {
                return new Bitmap(0,0);
            }

            return GetBitmap(reloadBmp, this.id);
        }

        public bool PixelSearch(out Point point, Color searchColor, RECT rect, bool reloadBmp, IntPtr id)
        {
            Bitmap bmp = GetBitmap(reloadBmp, id);
            {
                if (initilialized)
                {
                    rect.Left = rect.Left + this.point.X;
                    rect.Top = rect.Top + this.point.Y;
                    rect.Right = rect.Right + this.point.X;
                    rect.Bottom = rect.Bottom + this.point.Y;
                }

                if (rect.Left < 0)
                {
                    rect.Left = 0;
                }
                if (rect.Top < 0)
                {
                    rect.Top = 0;
                }
                if (rect.Right > bmp.Width)
                {
                    rect.Right = bmp.Width;
                }
                if (rect.Bottom > bmp.Height)
                {
                    rect.Bottom = bmp.Height;
                }

                for (int x = rect.Left; x < rect.Right; x++)
                {
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        Color color = bmp.GetPixel(x, y);

                        if (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") == searchColor.R.ToString("X2") + searchColor.G.ToString("X2") + searchColor.B.ToString("X2"))
                        {
                            point = new Point(x - 8, y - 8);
                            return true;
                        }
                    }
                }
            }
            point = new Point();
            return false;
        }

        public bool PixelSearch(out Point point, Color searchColor, RECT rect, bool reloadBmp)
        {
            if (!initilialized)
            {
                point = new Point();
                return false;
            }

            return PixelSearch(out point, searchColor, rect, reloadBmp, this.id);
        }

        public string OCRTextSearch(RECT rect, bool reloadBmp)
        {
            if (!initilialized || Locked)
            {
                return string.Empty;
            }

            Click(NGUIdleMasterWindowConstants.pointMouseMove, false, reloadBmp);

            Bitmap bmp = GetBitmap(reloadBmp);
            {
                rect.Left = rect.Left + this.point.X;
                rect.Top = rect.Top + this.point.Y;
                rect.Right = rect.Right + this.point.X;
                rect.Bottom = rect.Bottom + this.point.Y;

                if (rect.Left < 0)
                {
                    rect.Top = 0;
                }
                if (rect.Bottom < 0)
                {
                    rect.Bottom = 0;
                }
                if (rect.Right > bmp.Width)
                {
                    rect.Right = bmp.Width;
                }
                if (rect.Bottom > bmp.Height)
                {
                    rect.Bottom = bmp.Height;
                }

                using (Bitmap bmpOcr = bmp.Clone(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top), bmp.PixelFormat))
                {
                    using (Bitmap bmpOcrResized = ResizeImage(bmpOcr, bmpOcr.Width * 3, bmpOcr.Height * 3))
                    {
                        bmpOcrResized.Save(@"C:\temp\ocr.bmp");

                        using (Page process = ocr.Process(bmpOcrResized))
                        {
                            string text = process.GetText();
                            text = text.Replace("\n", " ");
                            text = text.Trim();
                            return text;
                        }
                    }
                }
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public Color GetPixelColor(Point point, bool reloadBmp)
        {
            if (!initilialized)
            {
                return new Color();
            }

            Bitmap bmp = GetBitmap(reloadBmp);
            {
                return bmp.GetPixel(point.X + this.point.X, point.Y + this.point.Y);
            }
        }

        public string GetHTMLColor(Point point, bool reloadBmp)
        {
            if (!initilialized)
            {
                return string.Empty;
            }

            Color color = GetPixelColor(point, reloadBmp);

            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public void SaveBitmap(string name)
        {
            if (!initilialized)
            {
                return;
            }

            SaveBitmap(new RECT() { Top = 0, Bottom = 9999, Left = 0, Right = 9999 }, true, name);
        }

        public void SaveBitmap(RECT rect, bool reloadBmp, string name)
        {
            if (!initilialized)
            {
                return;
            }

            Bitmap bmp = GetBitmap(reloadBmp);
            {
                rect.Left = rect.Left + this.point.X;
                rect.Top = rect.Top + this.point.Y;
                rect.Right = rect.Right + this.point.X;
                rect.Bottom = rect.Bottom + this.point.Y;

                if (rect.Left < 0)
                {
                    rect.Top = 0;
                }
                if (rect.Bottom < 0)
                {
                    rect.Bottom = 0;
                }
                if (rect.Right > bmp.Width)
                {
                    rect.Right = bmp.Width;
                }
                if (rect.Bottom > bmp.Height)
                {
                    rect.Bottom = bmp.Height;
                }

                Bitmap bmpOcr = GetBitmap(reloadBmp).Clone(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top), bmp.PixelFormat);
                {
                    bmpOcr.Save($@"C:\\temp\{name}.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        public void Log(string message)
        {
            message = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} - {message}";

            Console.WriteLine(new String('\b', Console.CursorLeft) + message);

            streamWriter?.WriteLine(message);
        }

        public void Wait(int seconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (seconds > 59)
            {
                Log($"Waiting for {seconds} seconds");
            }

            if (!Console.IsOutputRedirected)
            {
                Console.Write($"{seconds,5}");

                while (sw.ElapsedMilliseconds < seconds * 1000)
                {
                    Console.Write(new String('\b', 5) + $"{(seconds * 1000 - sw.ElapsedMilliseconds) / 1000,5}");

                    Thread.Sleep(200);
                }

                Console.Write(new String('\b', 5));
            }
            else
            {
                while (sw.ElapsedMilliseconds < seconds * 1000)
                {
                    Thread.Sleep(200);
                }
            }
        }

        public void SetInput(int input, bool useCap = true, bool useEnergy = true)
        {
            switch (input)
            {
                case 0:

                    if (useEnergy)
                    {
                        Click(NGUIdleMasterWindowConstants.pointPercentageIdleEnergy, false, false);
                    }
                    else
                    {
                        Click(NGUIdleMasterWindowConstants.pointPercentageIdleMagic, false, false);
                    }

                    break;

                case -1:
                    //all

                    if (useEnergy)
                    {
                        Click(NGUIdleMasterWindowConstants.pointFullEnergy, false, false);
                    }
                    else
                    {
                        Click(NGUIdleMasterWindowConstants.pointFullMagic, false, false);
                    }

                    break;

                case -2:
                    //half

                    if (useEnergy)
                    {
                        if (useCap)
                        {
                            Click(NGUIdleMasterWindowConstants.pointHalfCapEnergy, false, false);
                        }
                        else
                        {
                            Click(NGUIdleMasterWindowConstants.pointHalfIdleEnergy   , false, false);
                        }
                    }
                    else
                    {
                        if (useCap)
                        {
                            Click(NGUIdleMasterWindowConstants.pointHalfCapMagic, false, false);
                        }
                        else
                        {
                            Click(NGUIdleMasterWindowConstants.pointHalfIdleMagic, false, false);
                        }
                    }

                    break;

                case -4:
                    //quarter

                    if (useEnergy)
                    {
                        if (useCap)
                        {
                            Click(NGUIdleMasterWindowConstants.pointQuarterCapEnergy, false, false);
                        }
                        else
                        {
                            Click(NGUIdleMasterWindowConstants.pointQuarterIdleEnergy, false, false);
                        }
                    }
                    else
                    {
                        if (useCap)
                        {
                            Click(NGUIdleMasterWindowConstants.pointQuarterCapMagic, false, false);
                        }
                        else
                        {
                            Click(NGUIdleMasterWindowConstants.pointQuarterIdleMagic, false, false);
                        }
                    }

                    break;

                case var expression when input > 0:
                    //set to value

                    Click(NGUIdleMasterWindowConstants.pointInput, false, false);
                    SendString(input.ToString(), false);
                    break;

            }
        }
    }
}
