using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Topshelf.Builders;
using Topshelf.Logging;
using Topshelf.Runtime;
using Timer = System.Timers.Timer;

namespace DeleteService
{
    public class DeleteService
    {

        private static System.Timers.Timer aTimer;
        private static int retry = 0;

        public DeleteService(HostSettings settings)
        {
            
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(3000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // if(DateTime.Now.Day != CheckRunTimer().Day)
            // {
            //     DeleteFiles();
            // }
            Console.WriteLine("This service is running.");
            MoveFiles();
        }
        
        // private SemaphoreSlim _semaphoreSlim;
        // private Thread _thread;
        
        public void Start()
        {
            SetTimer();
            // _semaphoreSlim = new SemaphoreSlim(0);
            // _thread = new Thread(Delete);
            // _thread.Start();
        }

        public void Stop()
        {
            aTimer.Stop();
            // _semaphoreSlim.Release();
            // _thread.Join();
        }

        public void SaveRunTimer()
        {
            string datetime = DateTime.Now.ToString();
            System.IO.File.WriteAllText(@"C:\apps\env\date.txt", datetime);
        }

        public static DateTime CheckRunTimer()
        {
            StreamReader reader = new StreamReader(@"C:\apps\env\date.txt");
            string stringDate = reader.ReadToEnd();
            DateTime dateTime = DateTime.Parse(stringDate);
            return dateTime;
        }
        

        // private void Delete()
        // {
        //     while (true)
        //     {
        //         Console.WriteLine("hello.");
        //         if (_semaphoreSlim.Wait(5000))
        //         {
        //             break;
        //         }
        //         else
        //         {
        //             
        //         }
        //     }
        // }

        private static void MoveFiles()
        {
            string textfile = @"C:\apps\env\files.txt";
            string dest = @"C:\trash";
            //
            var text = File.ReadLines(textfile);
            
            var textFile = new System.IO.StreamReader(textfile);
            string line;

            while((line = textFile.ReadLine()) != null)
            {
                if (line.Equals("")) break;
                
                try
                {
                    StreamReader filesTxt;
                    string data;
                    StreamWriter fileTxt;
                    //
                    if (retry == 30)
                    {
                        Logger("The following folder could not be moved: " + line);
                        
                        filesTxt = new System.IO.StreamReader(textfile);
                        data = filesTxt.ReadToEnd();
                        filesTxt.Close();
                        data = Regex.Replace(data, "<.*\n", "");
                        fileTxt = new System.IO.StreamWriter(textfile, false);
                        fileTxt.Write(data);
                        filesTxt.Close();
                    }
                    
                    System.IO.Directory.Move(line, dest + "\\" + Guid.NewGuid().ToString());
                    Console.WriteLine("Moved Folder:" + line);
                    //
                    filesTxt = new System.IO.StreamReader(textfile);
                    data = filesTxt.ReadToEnd();
                    filesTxt.Close();
                    data = Regex.Replace(data, "<.*\n", "");
                    fileTxt = new System.IO.StreamWriter(textfile, false);
                    fileTxt.Write(data);
                    filesTxt.Close();
                    //
                    retry = 0;
                }
                //
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e + " Retry: " +  retry.ToString());
                    retry++;
                }
                // string[] files = Directory.GetFiles(item);
                // foreach (var file in files)
                // {
                //File.Move(file, dest);
                // }
            }
            
            textFile.Close();
        }

        private static void Logger(string log)
        {
            var fileTxt = new System.IO.StreamWriter(@"C:\apps\env\log.txt", true);
            fileTxt.WriteLine(log);
            fileTxt.Close();
        }

        private static void DeleteFiles()
        {
            string[] filePaths = Directory.GetFiles(@"C:\Trash");
            //
            foreach (var item in filePaths)
            {
                File.Delete(item);
            }
        }
    }
}