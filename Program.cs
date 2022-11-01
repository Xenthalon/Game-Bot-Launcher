using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GameLauncher
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Config c = LoadConfig();

            Console.WriteLine("Loaded config.");

            while (true)
            {
                Process[] botProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(c.botpath));

                foreach (var process in botProcesses)
                {
                    Console.WriteLine("Terminating remaining bot-process " + process.Id + ".");
                    process.Kill();
                }

                SelectCharacter(c.charname, c.modname);

                IntPtr handle = StartGame(c.gamepath, c.commandline, c.modname);

                System.Threading.Thread.Sleep(c.gamestarttimeout);

                Input.ResizeWindow(handle, c.window);

                var window = InputOperations.GetWindowRectangle(handle);

                Input.DoInputAtScreenPosition(handle, "{LMB}", new System.Drawing.Point(window.Width / 2, window.Height / 2));

                System.Threading.Thread.Sleep(c.accountloadtimeout);

                Input.DoInputAtScreenPosition(handle, "{LMB}", new System.Drawing.Point((int)(window.Width / 2 + 0.07 * window.Width), (int)(window.Height * 0.87)));

                StartBot(c.botpath, c.botprofile);

                while (InputOperations.IsWindow(handle))
                {
                    System.Threading.Thread.Sleep(1000);

                    var state = GetBotState(c.botapi);

                    if (state.InGame && state.CharacterName != c.charname)
                    {
                        Console.WriteLine("What's this? Who is " + state.CharacterName + "? Let's end this!");
                        Process[] d2rProcesses = Process.GetProcessesByName("D2R");

                        foreach (var process in d2rProcesses)
                        {
                            Console.WriteLine("Terminating d2r-process " + process.Id + ".");
                            process.Kill();
                        }

                        System.Threading.Thread.Sleep(5000);

                        break;
                    }
                }

                Console.WriteLine("Looks like we crashed!");

                Process[] errorProcesses = Process.GetProcessesByName("BlizzardError");

                foreach (var process in errorProcesses)
                {
                    Console.WriteLine("Terminating error-process " + process.Id + ".");
                    process.Kill();
                }

                // add error window detection here
                System.Threading.Thread.Sleep(10000);
            }
        }

        public static IntPtr StartGame(string path, string commandline, string mod)
        {
            Console.Write("Starting game...");

            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
            process.StartInfo.Arguments = commandline + " -mod " + mod;
            process.Start();

            do
            {
                System.Threading.Thread.Sleep(100);
            } while (process.MainWindowHandle == IntPtr.Zero);

            Console.WriteLine(" hWnd: " + process.MainWindowHandle);

            return process.MainWindowHandle;
        }

        public static void StartBot(string path, string profile)
        {
            Console.WriteLine("Starting bot.");

            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
            process.StartInfo.Arguments = profile;
            process.Start();
        }

        public static void SelectCharacter(string charname, string mod)
        {
            var saveDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Saved Games\\Diablo II Resurrected\\mods\\{mod}";

            var files = Directory.GetFiles(saveDir, $"{charname}*.ctlo");
            // var saveFile = $"{saveDir}\\{charname}.ctlo";

            if (files.Length == 1)
            {
                Console.WriteLine("Selecting character " + charname + ".");
                File.SetLastWriteTime(files[0], DateTime.Now);
            }
            else
            {
                Console.WriteLine("Couldn't find character " + charname + ". Please enter game once first.");
            }
        }

        public static BotState GetBotState(string url)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "GameLauncher");

            var result = new BotState() { InGame = false, CharacterName = "none", MapSeed = 0 };

            try
            {
                var streamTask = client.GetStringAsync(url);

                result = JsonConvert.DeserializeObject<BotState>(streamTask.Result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Request went wrong: " + e.Message);
            }

            return result;
        }

        public static Config LoadConfig()
        {
            Config config = null;

            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }

            return config;
        }
    }
}
