using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher
{
    class Input
    {
        public static void DoInputAtScreenPosition(IntPtr windowHandle, string input, Point screenPosition)
        {
            Rectangle _window = InputOperations.GetWindowRectangle(windowHandle);

            if (screenPosition.X < 20)
                screenPosition.X = 20;

            if (screenPosition.X > _window.Width * 0.98)
                screenPosition.X = (int)(_window.Width * 0.98);

            if (screenPosition.Y < 20)
                screenPosition.Y = 20;

            if (screenPosition.Y > _window.Height * 0.9)
                screenPosition.Y = (int)(_window.Height * 0.9);

            MouseMove(windowHandle, screenPosition);
            System.Threading.Thread.Sleep(10);

            DoInput(input);
        }

        public static void DoInput(string input)
        {
            if (input == "+{LMB}")
            {
                InputOperations.HoldLShift();
                MouseClick();
                InputOperations.ReleaseLShift();
            }
            else if (input == "^{LMB}")
            {
                InputOperations.HoldLCtrl();
                MouseClick();
                InputOperations.ReleaseLCtrl();
            }
            else if (input == "+{RMB}")
            {
                InputOperations.HoldLShift();
                MouseClick(false);
                InputOperations.ReleaseLShift();
            }
            else if (input == "{CTRL}")
            {
                InputOperations.HoldLCtrl();
                System.Threading.Thread.Sleep(50);
                InputOperations.ReleaseLCtrl();
            }
            else if (input == "{LMB}")
            {
                MouseClick();
            }
            else if (input == "{RMB}")
            {
                MouseClick(false);
            }
            else
            {
                SendKeys.SendWait(input);
            }
        }

        public static void MouseMove(IntPtr windowHandle, Point p)
        {
            var point = new InputOperations.MousePoint((int)p.X, (int)p.Y);
            InputOperations.ClientToScreen(windowHandle, ref point);
            InputOperations.SetCursorPosition(point.X, point.Y);
        }

        private static void MouseClick(bool left = true)
        {
            if (left)
            {
                InputOperations.MouseEvent(InputOperations.MouseEventFlags.LeftDown);
                System.Threading.Thread.Sleep(50);
                InputOperations.MouseEvent(InputOperations.MouseEventFlags.LeftUp);
            }
            else
            {
                InputOperations.MouseEvent(InputOperations.MouseEventFlags.RightDown);
                System.Threading.Thread.Sleep(50);
                InputOperations.MouseEvent(InputOperations.MouseEventFlags.RightUp);
            }
        }

        public static void ResizeWindow(IntPtr windowHandle, WindowConfig config)
        {
            Console.WriteLine("Resizing window!");
            const int SWP_SHOWWINDOW = 0x0040;

            InputOperations.SetWindowPos(windowHandle, IntPtr.Zero, config.X, config.Y, config.Width, config.Height, SWP_SHOWWINDOW);
            System.Threading.Thread.Sleep(500);
        }
    }
}
