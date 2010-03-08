using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ChiiTrans
{
    public enum AtlasInitStatus
    {
        NOT_INITIALIZED,
        SUCCESS,
        FAILURE
    }
    
    public class Atlas
    {
        private static string InstallPath;
        private static bool InstallPathInitialized = false;
        public static int Version;
        private static IntPtr atlecont;
        //private static IntPtr awdict;
        //private static IntPtr awuenv;
        private static Encoding Encoding932 = Encoding.GetEncoding(932);
        public static AtlasInitStatus status = AtlasInitStatus.NOT_INITIALIZED;
        private static Mutex mutex = new Mutex();

        public static string GetInstallPath()
        {
            if (InstallPathInitialized)
                return InstallPath;
            InstallPathInitialized = true;
            InstallPath = null;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Fujitsu\ATLAS\V14.0\JE");
            if (key == null)
            {
                key = Registry.CurrentUser.OpenSubKey(@"Software\Fujitsu\ATLAS\V13.0\JE");
                if (key == null)
                    return null;
                Version = 13;
            }
            else
            {
                Version = 14;
            }
            string res = (string)key.GetValue("TRENV JE");
            if (res == null)
                return null;
            InstallPath = Path.GetDirectoryName(res);
            return InstallPath;
        }

        private static IntPtr LoadLibrary(string name)
        {
            string path = Path.Combine(InstallPath, name);
            return PInvokeFunc.LoadLibraryEx(path, IntPtr.Zero, PInvokeFunc.LOAD_WITH_ALTERED_SEARCH_PATH);
        }

        private static bool UnloadLibrary(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
                return PInvokeFunc.FreeLibrary(handle);
            else
                return false;
        }
        
        private static bool LoadLibraries()
        {
            //Directory.SetCurrentDirectory(InstallPath);
            atlecont = LoadLibrary("AtleCont.dll");
            if (atlecont == IntPtr.Zero)
            {
                return false;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            /*awdict = LoadLibrary("awdict.dll");
            if (awdict == IntPtr.Zero)
            {
                UnloadLibrary(atlecont);
                return false;
            }
            awuenv = LoadLibrary("awuenv.dll");
            if (awuenv == IntPtr.Zero)
            {
                UnloadLibrary(atlecont);
                UnloadLibrary(awuenv);
                return false;
            }*/
            return true;
        }

        private delegate int CreateEngineType(int x, int dir, int x3, byte[] x4);
        private static CreateEngineType CreateEngine;

        private delegate int DestroyEngineType();
        private static DestroyEngineType DestroyEngine;

        private delegate int TranslatePairType(byte[] inp, out IntPtr outp, out IntPtr dunno, out uint maybeSize);
        private static TranslatePairType TranslatePair;

        private delegate int AtlInitEngineDataType(int x1, int x2, IntPtr x3, int x4, IntPtr x5);
        private static AtlInitEngineDataType AtlInitEngineData;

        //private delegate int SetTransStateType(int dunno);
        //private static SetTransStateType SetTransState;

        private delegate int FreeAtlasDataType(IntPtr mem, IntPtr noSureHowManyArgs, IntPtr x3, IntPtr x4);
        private static FreeAtlasDataType FreeAtlasData;

        //private delegate int AwuWordDelType(int x1, byte[] type, int x3, byte[] word);
        //private static AwuWordDelType AwuWordDel;

        private static Delegate LoadFunc(IntPtr module, string name, Type T)
        {
            IntPtr addr = PInvokeFunc.GetProcAddress(module, name);
            if (addr != IntPtr.Zero)
                return Marshal.GetDelegateForFunctionPointer(addr, T);
            else
                throw new Exception("Cannot load function " + name + "!");
        }
        
        private static void LoadInterface()
        {
            CreateEngine = (CreateEngineType)LoadFunc(atlecont, "CreateEngine", typeof(CreateEngineType));
		    DestroyEngine = (DestroyEngineType)LoadFunc(atlecont, "DestroyEngine", typeof(DestroyEngineType));
            TranslatePair = (TranslatePairType)LoadFunc(atlecont, "TranslatePair", typeof(TranslatePairType));
            FreeAtlasData = (FreeAtlasDataType)LoadFunc(atlecont, "FreeAtlasData", typeof(FreeAtlasDataType));
            AtlInitEngineData = (AtlInitEngineDataType)LoadFunc(atlecont, "AtlInitEngineData", typeof(AtlInitEngineDataType));
            //SetTransState = (SetTransStateType)LoadFunc(atlecont, "SetTransState", typeof(SetTransStateType));
            // AwuWordDel = (AwuWordDelType)LoadFunc(awdict, "AwuWordDel", typeof(AwuWordDelType));
        }

        public static bool Initialize(string installPath)
        {
            while (initializing)
                Thread.Sleep(1);
            if (status == AtlasInitStatus.NOT_INITIALIZED)
            {
                if (InitializeInt(installPath))
                {
                    status = AtlasInitStatus.SUCCESS;
                    return true;
                }
                else
                {
                    status = AtlasInitStatus.FAILURE;
                    Deinitialize();
                    return false;
                }
            }
            else
            {
                return status == AtlasInitStatus.SUCCESS;
            }
        }

        public static void Deinitialize()
        {
            try
            {
                if (DestroyEngine != null)
                    DestroyEngine();
                UnloadLibrary(atlecont);
            }
            catch (Exception)
            { }
        }

        public static bool Ready()
        {
            return Initialize(null);
        }

        private static bool initializing = false;
        private static bool InitializeInt(string installPath)
        {
            try
            {
                initializing = true;
                if (installPath != null)
                {
                    InstallPath = installPath;
                    InstallPathInitialized = true;
                }
                else
                {
                    GetInstallPath();
                    if (InstallPath == null)
                        return false;
                }
                if (!LoadLibraries())
                    return false;
                LoadInterface();
                if (AtlInitEngineData(0, 2, Marshal.AllocHGlobal(20000), 0, Marshal.AllocHGlobal(20000)) != 0)
                    return false;
                string env = "General";
                if (CreateEngine(1, 1, 0, Encoding932.GetBytes(env)) != 1)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                initializing = false;
            }
        }

        public static string Translate(string source)
        {
            if (source == null)
                return source;
            bool haveLetters = false;
            foreach (char ch in source)
            {
                if (char.IsLetter(ch))
                {
                    haveLetters = true;
                    break;
                }
            }
            if (!haveLetters)
                return source;
            //long before = DateTime.Now.Ticks;
            mutex.WaitOne();
            try
            {
                IntPtr outp;
                IntPtr tmp;
                uint size;
                byte[] inp = Encoding932.GetBytes(source);
                TranslatePair(inp, out outp, out tmp, out size);
                string result;
                if (outp != IntPtr.Zero)
                {
                    result = Encoding932.GetString(PInvokeFunc.ByteArrayFromPtr(outp));
                    FreeAtlasData(outp, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    result = null;
                }
                if (tmp != IntPtr.Zero)
                {
                    /*if (result != null)
                    {
                        string s = Encoding932.GetString(PInvokeFunc.ByteArrayFromPtr(tmp));
                        result += " [" + s + "]";
                    }*/
                    FreeAtlasData(tmp, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }
                return result;
            }
            finally
            {
                mutex.ReleaseMutex();
                //Form1.Debug(((double)(DateTime.Now.Ticks - before) / 10000000).ToString());
            }
        }

    }
}
