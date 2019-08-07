using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RegKeyChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Getting HKCR");
                var hkcr = Registry.ClassesRoot;
                Console.WriteLine("Getting HKCR\\TypeLib");
                var typeLib = hkcr.OpenSubKey("TypeLib");
                Console.WriteLine("Checking for HKCR\\TypeLib\\{00020905-0000-c000-000000000046}");
                var lib = typeLib.OpenSubKey("{00020905-0000-c000-000000000046}");
                if (lib == null)
                {
                    Console.WriteLine("... NOT FOUND");
                }
                else
                {
                    PrintSubKeys(lib);
                    lib.Close();
                }

                typeLib.Close();
                hkcr.Close();
            }
            catch (Exception exc)
            {
                ReportException(exc);
            }

            try
            {
                Console.WriteLine("Getting HKLM");
                var hklm = Registry.LocalMachine;
                Console.WriteLine("Getting HKLM\\SOFTWARE");
                var software = hklm.OpenSubKey("SOFTWARE");
                Console.WriteLine("Checking for HKLM\\SOFTWARE\\R-core");
                var rcore = software.OpenSubKey("R-core");
                if (rcore == null)
                {
                    Console.WriteLine("... NOT FOUND");
                }
                else
                {
                    PrintSubKeys(rcore);
                    rcore.Close();
                }

                Console.WriteLine("Checking for HKLM\\SOFTWARE\\WOW6432Node");
                var wow6432 = software.OpenSubKey("WOW6432Node");
                if (wow6432 == null)
                {
                    Console.WriteLine("... NOT FOUND");
                }
                else
                {
                    Console.WriteLine("Checking for HKLM\\SOFTWARE\\WOW6432Node\\R-core");
                    var rcoreWow = wow6432.OpenSubKey("R-core");
                    if (rcoreWow == null)
                    {
                        Console.WriteLine("... NOT FOUND");
                    }
                    else
                    {
                        PrintSubKeys(rcoreWow);
                        rcoreWow.Close();
                    }
                    wow6432.Close();
                }
                software.Close();
                hklm.Close();
            }
            catch (Exception exc)
            {
                ReportException(exc);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void ReportException(Exception exc)
        {
            Console.WriteLine("There was an unhandled exception");
            Console.WriteLine(exc.Message);
            Console.WriteLine(exc.StackTrace);
        }

        static void PrintSubKeys(RegistryKey key, uint depth = 1)
        {
            var prefix = new String(' ', (int)(2 * depth));
            Console.WriteLine("{0}Preparing to enumerate {1}", prefix, key.Name);
            var subkeys = key.GetSubKeyNames();
            Console.WriteLine("{0}There is/are {1} sub-key(s)", prefix, subkeys.Length);
            foreach (var subkey in subkeys)
            {
                var regSubKey = key.OpenSubKey(subkey);
                PrintSubKeys(regSubKey, (depth + 1));
                regSubKey.Close();
            }

            var valueNames = key.GetValueNames();
            Console.WriteLine("{0}There is/are {1} value(s)", prefix, valueNames.Length);
            foreach (var valueName in valueNames)
            {
                Console.WriteLine("  {0}{1} ({2}) = {3}", prefix, valueName, key.GetValueKind(valueName).ToString(), key.GetValue(valueName).ToString());
            }
            key.Close();
        }
    }
}
