using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace URWME // Unreal World MemoryManager
{
    public static class Extensions
    {
        public static string ReplaceModifiedString(this string s)
        {
            return s;//.Replace('Σ', 'ä').Replace('Ø', '¥').Replace('─', 'Ä').Replace('÷', 'ö');
        }

        public static string ToOrdinal(this int i)
        {
            if (i <= 0) return i.ToString();

            switch (i % 100)
            {
                case 11:
                case 12:
                case 13:
                    return i + "th";
            }

            switch (i % 10)
            {
                case 1:
                    return i + "st";
                case 2:
                    return i + "nd";
                case 3:
                    return i + "rd";
                default:
                    return i + "th";
            }
        }
        public static string ToTitleCase(this string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static string ToJson(this object obj)
        {
            // Return an empty object if the input is null
            if (obj == null)
                return "{}";

            // Use the built-in JsonSerializer to handle the object
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        public static async Task<Image> GetImageFromUrlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var data = await client.GetByteArrayAsync(url);
                using (var ms = new MemoryStream(data))
                {
                    return Image.FromStream(ms);
                }
            }
        }
    }

    public class PipeServer
    {
        private Thread _listenThread;
        private NamedPipeServerStream _pipeServer;
        private StreamWriter _writer;
        private StreamReader _reader;

        public void Start()
        {
            _listenThread = new Thread(async () => await ServerLoop())
            {
                IsBackground = true
            };
            _listenThread.Start();
        }


        private async Task ServerLoop()
        {
            _pipeServer = new NamedPipeServerStream("URWMEPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            //File.AppendAllText("pipe.log", "[SERVER] Waiting for client...\n");
            await _pipeServer.WaitForConnectionAsync();
            //File.AppendAllText("pipe.log", "[SERVER] Client Connected\n");

            _reader = new StreamReader(_pipeServer, Encoding.UTF8);
            //File.AppendAllText("pipe.log", "[SERVER] reader created\n");

            string? clientHandshake = await _reader.ReadLineAsync();

            _writer = new StreamWriter(_pipeServer, Encoding.UTF8) { AutoFlush = true };
            //File.AppendAllText("pipe.log", "[SERVER] writer created\n");

            while (true)
            {
                try
                {
                    string? line = await _reader.ReadLineAsync();
                    if (line == null) break; // client disconnected
                    Debug.WriteLine(line);
                }
                catch (IOException)
                {
                    //File.AppendAllText("pipe.log", "[SERVER] Error\n");
                    break; // connection lost
                }
            }
        }

        public async Task Send(string msg)
        {
            await _writer?.WriteLineAsync(msg);
        }
    }

    public class PipeClient
    {
        private Thread _listenThread;
        private NamedPipeClientStream _pipeClient;
        private StreamWriter _writer;
        private StreamReader _reader;

        private readonly Action<string> _onMessage;

        public PipeClient(Action<string> onMessage)
        {
            _onMessage = onMessage;
        }

        public void Start()
        {
            _listenThread = new Thread(async () => await ClientLoop())
            {
                IsBackground = true
            };
            _listenThread.Start();
        }


        private async Task ClientLoop()
        {
            _pipeClient = new NamedPipeClientStream(".", "URWMEPipe", PipeDirection.InOut, PipeOptions.Asynchronous);
            //File.AppendAllText("pipe.log", "[CLIENT] Connecting\n");
            await _pipeClient.ConnectAsync();
            //File.AppendAllText("pipe.log", "[CLIENT] Connected\n");

            _reader = new StreamReader(_pipeClient, Encoding.UTF8);
            //File.AppendAllText("pipe.log", "[CLIENT] reader created\n");

            _writer = new StreamWriter(_pipeClient, Encoding.UTF8) { AutoFlush = true };
            //File.AppendAllText("pipe.log", "[CLIENT] writer created\n");
            await Send("handshake");

            while (true)
            {
                try
                {
                    string? line = await _reader.ReadLineAsync();
                    if (line == null)
                    {
                        Application.Exit();
                        break;
                    }

                    //File.AppendAllText("pipe.log", "[CLIENT] Received: " + line + "\n");

                    InvokeOnUi(() =>
                    {
                        _onMessage(line);
                    });
                }
                catch (IOException)
                {
                    //File.AppendAllText("pipe.log", "[CLIENT] catched error\n");
                    MessageBox.Show("Minimap connection error.");
                    break; // connection lost
                }
            }
        }

        public async Task Send(string msg)
        {
            await _writer?.WriteLineAsync(msg);
            //File.AppendAllText("pipe.log", "[CLIENT] sent: " + msg + "\n");
        }

        private void InvokeOnUi(Action action)
        {
            if (Application.OpenForms.Count > 0)
            {
                var form = Application.OpenForms[0];
                if (form.InvokeRequired)
                    form.BeginInvoke(action);
                else
                    action();
            }
            else
            {
                action();
            }
        }
    }
}
