using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;

namespace ChiiTrans
{
    class AgthMessage
    {
        private static int idCtr = 0;
        
        public string agthThread;
        public int id;
        public uint pid;
        public uint time;
        public string text;

        public AgthMessage(string agthThread, uint pid, uint time, string text)
        {
            this.id = ++idCtr;
            this.agthThread = agthThread;
            this.pid = pid;
            this.time = time;
            this.text = text;
        }
    }

    class AgthMessageList : List<AgthMessage>
    {
        private Mutex mutex;

        public AgthMessageList() : base()
        {
            mutex = new Mutex();
        }
        
        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }
    }

    class BlockList : LinkedList<string>
    {
        public bool IsMonitored { get; set; }

        public BlockList()
            : base()
        {
            IsMonitored = false;
        }
    }
    
    class Agth
    {
        public bool is_on {get; private set;}
        public bool isConnected { get; private set; }
        private const string pipeName = "agth";
        private NamedPipeServerStream pipe;
        private Thread readingThread;
        private Thread parsingThread;
        private AutoResetEvent newMessages;
        private AgthMessageList messages;
        public Dictionary<string, BlockList> blocks;
        public bool manualMonitoring;
        private string lastText;
        public string CurrentApp { get; private set; }
        private string CurrentAppKeys;
        private Dictionary<uint, string> pidToApp;
        public JsObject appProfiles;
        
        public Agth()
        {
            is_on = false;
            isConnected = false;
            lastGoodName = false;
            messages = new AgthMessageList();
            blocks = new Dictionary<string, BlockList>();
            newMessages = new AutoResetEvent(false);
            manualMonitoring = false;
            lastText = "";
            CurrentApp = null;
            pidToApp = new Dictionary<uint, string>();
            oldPid = 0;
        }

        public bool TryConnectPipe()
        {
            if (isConnected)
                return true;
            else
            {
                try
                {
                    pipe = new NamedPipeServerStream(pipeName, PipeDirection.In);
                    isConnected = true;
                    StartReadingThread();
                    if (parsingThread == null)
                        StartParsingThread();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                TurnOff();
                pipe.Close();
                pipe.Dispose();
            }
        }

        public bool TurnOn()
        {
            if (TryConnectPipe())
            {
                is_on = true;
                Form1.thisForm.UpdateButtonOnOff(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void TurnOff()
        {
            is_on = false;
            Form1.thisForm.UpdateButtonOnOff(false);
        }

        private void StartReadingThread()
        {
            readingThread = new Thread(ReadingThreadProc);
            readingThread.IsBackground = true;
            readingThread.Start();
        }

        private void ReadingThreadProc()
        {
            try
            {
                byte[] buf = new byte[100];
                messages.Lock();
                messages.Clear();
                messages.Unlock();
                while (isConnected)
                {
                    if (!pipe.IsConnected)
                    {
                        pipe.WaitForConnection();
                    }
                    pipe.Read(buf, 0, 20);
                    uint seg = BitConverter.ToUInt32(buf, 0);
                    uint ofs = BitConverter.ToUInt32(buf, 4);
                    uint pid = BitConverter.ToUInt32(buf, 8);
                    uint time = BitConverter.ToUInt32(buf, 12);
                    int size = BitConverter.ToInt32(buf, 16);
                    if (time == 0)
                        break;
                    size = size * 2;
                    pipe.Read(buf, 0, 48);
                    int len = 0;
                    while (len < 48 && (buf[len] != 0 || buf[len + 1] != 0))
                        len += 2;
                    if (len == 0 || len > 48)
                        break;
                    string funcName = Encoding.Unicode.GetString(buf, 0, len);
                    string text;
                    if (size <= buf.Length)
                    {
                        pipe.Read(buf, 0, size);
                        text = Encoding.Unicode.GetString(buf, 0, size);
                    }
                    else
                    {
                        byte[] buf2 = new byte[size];
                        pipe.Read(buf2, 0, size);
                        text = Encoding.Unicode.GetString(buf2, 0, size);
                    }
                    if (isConnected)
                        AddMessage(funcName, seg, ofs, pid, time, text);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                isConnected = false;
                TurnOff();
                try
                {
                    pipe.Close();
                }
                catch (Exception) { }
                if (Global.fullscreen)
                    Global.FullscreenOff();
            }
        }

        private void StartParsingThread()
        {
            parsingThread = new Thread(ParsingThreadProc);
            parsingThread.IsBackground = true;
            parsingThread.Start();
        }

        private bool isGoodName(string agthThread)
        {
            string threadName = agthThread.Substring(agthThread.LastIndexOf(' ') + 1);
            return threadName == "KiriKiri" || threadName == "System40" || threadName == "RealLive" || threadName.StartsWith("TextOut");
        }

        uint oldPid;
        private void ParsingThreadProc()
        {
            Dictionary<string, string> parsed = new Dictionary<string, string>();
            while (true)
            {
                newMessages.WaitOne();
                parsed.Clear();
                Thread.Sleep(Global.options.messageDelay);
                messages.Lock();
                string lastThreadName = "";
                try
                {
                    if (messages.Count > 0)
                    {
                        foreach (AgthMessage msg in messages)
                        {
                            if (msg.pid != oldPid)
                            {
                                oldPid = msg.pid;
                                SetCurrentApp(msg.pid);
                            }
                            string s;
                            if (!parsed.TryGetValue(msg.agthThread, out s))
                            {
                                s = "";
                            }
                            parsed[msg.agthThread] = s + msg.text;
                        }
                        lastThreadName = messages[messages.Count - 1].agthThread;
                        messages.Clear();
                    }
                }
                finally
                {
                    messages.Unlock();
                }
                
                alreadyAdded = false;
                bool good = false;
                if (!manualMonitoring)
                {
                    foreach (string name in parsed.Keys)
                    {
                        if (isGoodName(name))
                        {
                            good = true;
                            break;
                        }
                    }
                }

                foreach (KeyValuePair<string, string> pair in parsed)
                {
                    AddBlock(pair.Key, pair.Value, good);
                }
            }
        }

        private void AddMessage(string funcName, uint seg, uint ofs, uint pid, uint time, string text)
        {
            string agthThread = string.Format("0x{0:X8}:{1:X8} {2}", seg, ofs, funcName);
            messages.Lock();
            messages.Add(new AgthMessage(agthThread, pid, time, text));
            messages.Unlock();
                newMessages.Set();
        }

        bool lastGoodName;
        bool alreadyAdded;
        private void AddBlock(string agthThread, string text, bool haveGood)
        {
            if (!blocks.ContainsKey(agthThread))
            {
                blocks.Add(agthThread, new BlockList());
                if (!manualMonitoring && !alreadyAdded)
                {
                    bool good = isGoodName(agthThread);
                    if (good || !lastGoodName && !haveGood)
                    {
                        foreach (BlockList bl in blocks.Values)
                        {
                            bl.IsMonitored = false;
                        }
                        blocks[agthThread].IsMonitored = true;
                        lastGoodName = good;
                        alreadyAdded = true;
                    }
                }
                if (FormMonitor.instance.Visible)
                    FormMonitor.instance.UpdateThreadList();
            }

            blocks[agthThread].AddLast(text);
            if (blocks[agthThread].Count > 100)
            {
                blocks[agthThread].RemoveFirst();
            }
            if (FormMonitor.instance.Visible)
                FormMonitor.instance.AddThreadBlock(agthThread, text);
            if (is_on && blocks[agthThread].IsMonitored && text != lastText)
            {
                Translation.Translate(text, null);
                lastText = text;
            }
        }

        public void SetCurrentApp(string app)
        {
            if (app != null && app != CurrentApp)
            {
                if (CurrentApp != null)
                    SaveAppProfile(CurrentApp);
                CurrentApp = app;
                CurrentAppKeys = "";

                blocks.Clear();
                Global.ResetGameWindow();
                LoadAppProfile(CurrentApp);
                SaveAppProfile(CurrentApp);
            }
        }

        public void SetCurrentApp(uint pid)
        {
            if (!pidToApp.ContainsKey(pid))
            {
                pidToApp[pid] = Global.AppNameFromPid(pid);
            }
            SetCurrentApp(pidToApp[pid]);
        }

        private void SaveAppProfile(string app)
        {
            JsObject data = appProfiles["profiles"][app];
            data.str["keys"] = CurrentAppKeys;
            JsArray mon = new JsArray();
            foreach (KeyValuePair<string, BlockList> kvp in blocks)
            {
                if (kvp.Value.IsMonitored)
                    mon.Add(new JsAtom(kvp.Key));
            }
            data["monitor"] = mon;
            data.num["manual"] = manualMonitoring ? 1 : 0;
        }

        private void LoadAppProfile(string app)
        {
            if (appProfiles["profiles"].dict.ContainsKey(app))
            {
                JsObject data = appProfiles["profiles"][app];
                foreach (string threadName in data["monitor"].str)
                {
                    if (!blocks.ContainsKey(threadName))
                    {
                        blocks.Add(threadName, new BlockList());
                        blocks[threadName].IsMonitored = true;
                    }
                }
                manualMonitoring = data.num["manual"] != 0;
                CurrentAppKeys = data.str["keys"];
            }
        }

        public void SaveAppProfiles()
        {
            if (CurrentApp != null)
                SaveAppProfile(CurrentApp);
            try
            {
                string filename = Path.Combine(Global.cfgdir, "apps.txt");
                File.WriteAllText(filename, appProfiles.Serialize());
            }
            catch (Exception)
            { }
        }

        public void LoadAppProfiles()
        {
            try
            {
                string filename = Path.Combine(Global.cfgdir, "apps.txt");
                appProfiles = Json.Parse(File.ReadAllText(filename));
            }
            catch (Exception)
            {
                appProfiles = new JsObject();
                appProfiles.str["last_run"] = "";
            }
        }

        public void SetCurrentAppKeys(string keys)
        {
            CurrentAppKeys = keys;
        }
    }
}
