using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        const string website_URL = "youtube.com";

        Console.WriteLine("Do you want to block youtube b/u (block/unblock)");
        string userInput = Console.ReadLine();


        if(userInput == "b") BlockWebsite(website_URL);
        if(userInput == "u") UnblockWebsite(website_URL);

        FlushDNSCache();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static string BuildString(string wesite_URL)
    {
        string entry = "127.0.0.1 " + wesite_URL + "\r\n" + "127.0.0.1 " + "www." + wesite_URL;
        return entry;
    }

    static void BlockWebsite(string website_URL)
    {
        try
        {
            string host_file_path = Path.Combine(Environment.SystemDirectory, @"drivers\etc\hosts");

            if (File.Exists(host_file_path))
            {
                string hostFileContent = File.ReadAllText(host_file_path);

                if (!hostFileContent.Contains(website_URL))
                {
                    string entry = BuildString(website_URL);
                    File.AppendAllText(host_file_path, entry);
                }
                Console.WriteLine($"Blocked {website_URL}");
            }
            else
            {
                throw new FileNotFoundException("Host file not found. Make sure you have administrative privileges.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to block website: {ex.Message}");
        }
    }


    static void UnblockWebsite(string website_URL)
    {
        try
        {
            string host_file_path = Path.Combine(Environment.SystemDirectory, @"drivers\etc\hosts");

            if (File.Exists(host_file_path))
            {
                string host_file_content = File.ReadAllText(host_file_path);

                if (host_file_content.Contains(website_URL))
                {
                    string[] lines = File.ReadAllLines(host_file_path);
                    // Create a new host file content excluding the blocked website entry
                    string new_host_file_content = "";
                    foreach (string line in lines)
                    {
                        if (!line.Contains(website_URL))
                        {
                            new_host_file_content += line + Environment.NewLine;
                        }
                    }
                    File.WriteAllText(host_file_path, new_host_file_content);
                    Console.WriteLine($"Unblocked {website_URL}");
                }
                else
                {
                    Console.WriteLine($"Website {website_URL} is not blocked.");
                }
            }
            else
            {
                throw new FileNotFoundException("Host file not found. Make sure you have administrative privileges.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to unblock website: {ex.Message}");
        }
    }


    static void FlushDNSCache()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            process.StandardInput.WriteLine("ipconfig /flushdns");
            process.StandardInput.Flush();
            process.StandardInput.Close();

            process.WaitForExit();
            Console.WriteLine("Flushed DNS Cache");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error flushing DNS cache: " + ex.Message);
        }
    }
}
