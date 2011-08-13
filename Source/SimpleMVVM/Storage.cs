using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;

namespace SimpleMVVM
{
    public static partial class Storage
    {
        private const string WindowLayoutFileName = "window_layout.xml";

        private static string GetWindowLayoutFileName(Window window)
        {
            return string.Format("{0}_{1}", window.Name, WindowLayoutFileName);
        }

        private static IsolatedStorageFile GetStorageFile()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }

        public static XmlReader OpenXmlFile(string fileName)
        {
            var storage = GetStorageFile();
            if (!storage.FileExists(fileName))
            {
                return null;
            }

            var fileStream = storage.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            var reader = new XmlTextReader(fileStream)
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            return reader;
        }

        public static XmlWriter CreateXmlFile(string fileName)
        {
            var storage = GetStorageFile();
            var fileStream = storage.CreateFile(fileName);
            var writer = new XmlTextWriter(fileStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            return writer;
        }

        public static void RestoreWindowLayout(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            if (string.IsNullOrWhiteSpace(window.Name))
            {
                throw new ArgumentException("Name is not set.", "window");
            }

            using (var windowLayoutReader = OpenXmlFile(GetWindowLayoutFileName(window)))
            {
                if (windowLayoutReader == null)
                {
                    return;
                }

                windowLayoutReader.MoveToContent();

                var xml = XElement.Load(windowLayoutReader);

                var showCmdElement = xml.Element("showCmd");
                Debug.Assert(showCmdElement != null, "showCmd != null");

                var minPositionElement = xml.Element("minPosition");
                Debug.Assert(minPositionElement != null, "minPosition != null");

                var maxPositionElement = xml.Element("maxPosition");
                Debug.Assert(maxPositionElement != null, "maxPosition != null");

                var normalPositionElement = xml.Element("normalPosition");
                Debug.Assert(normalPositionElement != null, "normalPosition != null");

                var placement = new Win32.WindowPlacement
                {
                    length = Marshal.SizeOf(typeof(Win32.WindowPlacement)),
                    flags = 0,
                    showCmd = (int)showCmdElement == Win32.ShowMinimized ? Win32.ShowNormal : (int)showCmdElement,
                    minPosition =
                        {
                            X = (int)minPositionElement.Attribute("X"),
                            Y = (int)minPositionElement.Attribute("Y")
                        },
                    maxPosition =
                        {
                            X = (int)maxPositionElement.Attribute("X"),
                            Y = (int)maxPositionElement.Attribute("Y")
                        },
                    normalPosition =
                        {
                            Left = (int)normalPositionElement.Attribute("Left"),
                            Top = (int)normalPositionElement.Attribute("Top"),
                            Right = (int)normalPositionElement.Attribute("Right"),
                            Bottom = (int)normalPositionElement.Attribute("Bottom")
                        }
                };

                var interopHelper = new WindowInteropHelper(window);
                Win32.SetWindowPlacement(interopHelper.Handle, ref placement);
            }
        }

        public static void SaveWindowLayout(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            if (string.IsNullOrWhiteSpace(window.Name))
            {
                throw new ArgumentException("Name is not set.", "window");
            }

            using (var windowLayoutWriter = CreateXmlFile(GetWindowLayoutFileName(window)))
            {
                var interopHelper = new WindowInteropHelper(window);
                Win32.WindowPlacement placement;
                Win32.GetWindowPlacement(interopHelper.Handle, out placement);

                var xml =
                    new XElement("placement",
                        new XElement("showCmd", placement.showCmd),
                        new XElement("minPosition",
                            new XAttribute("X", placement.minPosition.X),
                            new XAttribute("Y", placement.minPosition.Y)),
                        new XElement("maxPosition",
                            new XAttribute("X", placement.maxPosition.X),
                            new XAttribute("Y", placement.maxPosition.Y)),
                        new XElement("normalPosition",
                            new XAttribute("Left", placement.normalPosition.Left),
                            new XAttribute("Top", placement.normalPosition.Top),
                            new XAttribute("Right", placement.normalPosition.Right),
                            new XAttribute("Bottom", placement.normalPosition.Bottom)));

                xml.Save(windowLayoutWriter);
            }
        }
    }
}
