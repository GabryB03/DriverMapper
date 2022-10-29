using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Principal;

class Program
{
    static void Main()
    {
        if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
        {
            Bypass.UAC();
            string WhatToElevate = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Process.Start("CMD.exe", "/c start \"" + WhatToElevate + "\"");
            RegistryKey uac_clean = Registry.CurrentUser.OpenSubKey("Software\\Classes\\ms-settings", true);
            uac_clean.DeleteSubKeyTree("shell");
            uac_clean.Close();
            Process.GetCurrentProcess().Kill();
            return;
        }

        Console.Title = "DriverMapper";
        string path = "";

        while (path == "")
        {
            Console.Write("[INFO] Please, insert the path of the driver to map: ");
            path = Console.ReadLine();

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("[ERROR] The specified file does not exist.");
                path = "";
                continue;
            }

            if (!System.IO.Path.GetExtension(path).ToLower().Equals(".sys"))
            {
                Console.WriteLine("[ERROR] The specified file has not the '.sys' extension.");
                path = "";
                continue;
            }
        }

        ProtoRandom.ProtoRandom random = new ProtoRandom.ProtoRandom(5);
        char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1);

        if (!System.IO.Directory.Exists($"{rootDir}:\\Temp"))
        {
            System.IO.Directory.CreateDirectory($"{rootDir}:\\Temp");
        }

        string randomName = random.GetRandomString(characters, random.GetRandomInt32(4, 17));
        string fileName = $"{rootDir}:\\Temp\\{randomName}";

        System.IO.File.WriteAllBytes(fileName + ".exe", DriverMapper.Properties.Resources.kdmapper);
        System.IO.File.SetAttributes(fileName + ".exe", System.IO.FileAttributes.Hidden | System.IO.FileAttributes.ReadOnly | System.IO.FileAttributes.Normal);
        
        System.IO.File.WriteAllBytes(fileName + ".sys", System.IO.File.ReadAllBytes(path));
        System.IO.File.SetAttributes(fileName + ".sys", System.IO.FileAttributes.Hidden | System.IO.FileAttributes.ReadOnly | System.IO.FileAttributes.Normal);

        ProcessStartInfo startInfo = new ProcessStartInfo();

        Process cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();

        cmd.StandardInput.WriteLine($"\"{fileName + ".exe"}\" \"{fileName + ".sys"}\"");
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();

        System.IO.File.SetAttributes(fileName + ".exe", System.IO.FileAttributes.Normal);
        System.IO.File.SetAttributes(fileName + ".sys", System.IO.FileAttributes.Normal);

        System.IO.File.Delete(fileName + ".exe");
        System.IO.File.Delete(fileName + ".sys");

        Console.WriteLine("[SUCCESS] Succesfully mapped your driver.");
        Console.WriteLine("[SUCCESS] Press ENTER to exit from the program.");

        Console.ReadLine();
    }
}