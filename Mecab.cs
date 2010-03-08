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
    public enum MecabInitStatus
    {
        NOT_INITIALIZED,
        SUCCESS,
        FAILURE
    }
    
    class Mecab
    {
        private static string InstallPath;
        private static bool InstallPathInitialized = false;
        private static IntPtr dll;
        private static Encoding MecabEncoding;
        public static MecabInitStatus status = MecabInitStatus.NOT_INITIALIZED;
        private static Mutex mutex = new Mutex();
        private static IntPtr mecab;

        public static string GetInstallPath()
        {
            if (InstallPathInitialized)
                return InstallPath;
            InstallPathInitialized = true;
            InstallPath = null;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Mecab");
            if (key == null)
            {
                key = Registry.LocalMachine.OpenSubKey(@"Software\Mecab");
                if (key == null)
                    return null;
            }
            string res = (string)key.GetValue("mecabrc");
            if (res == null)
                return null;
            int x = res.LastIndexOf(@"etc\");
            if (x < 0)
                return null;
            InstallPath = Path.Combine(res.Substring(0, x), "bin");
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
            dll = LoadLibrary("libmecab.dll");
            if (dll == IntPtr.Zero)
                return false;
            return true;
        }

        private delegate IntPtr mecab_new2Type(string arg);
        private static mecab_new2Type mecab_new2;

        private delegate void mecab_destroyType(IntPtr mecab);
        private static mecab_destroyType mecab_destroy;

        private delegate IntPtr mecab_sparse_tostrType(IntPtr mecab, byte[] str);
        private static mecab_sparse_tostrType mecab_sparse_tostr;

        [StructLayout(LayoutKind.Sequential)]
        private class mecab_dictionary_info_t
        {
            public IntPtr filename;
            public IntPtr charset;
            public uint size;
            public int type;
            public uint lsize;
            public uint rsize;
            public ushort version;
            public IntPtr next;
        }

        private delegate IntPtr mecab_dictionary_infoType(IntPtr mecab);
        private static mecab_dictionary_infoType mecab_dictionary_info;
        
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
            mecab_new2 = (mecab_new2Type)LoadFunc(dll, "mecab_new2", typeof(mecab_new2Type));
            mecab_destroy = (mecab_destroyType)LoadFunc(dll, "mecab_destroy", typeof(mecab_destroyType));
            mecab_sparse_tostr = (mecab_sparse_tostrType)LoadFunc(dll, "mecab_sparse_tostr", typeof(mecab_sparse_tostrType));
            mecab_dictionary_info = (mecab_dictionary_infoType)LoadFunc(dll, "mecab_dictionary_info", typeof(mecab_dictionary_infoType));
        }

        public static bool Initialize(string installPath)
        {
            while (initializing)
                Thread.Sleep(1);
            if (status == MecabInitStatus.NOT_INITIALIZED)
            {
                if (InitializeInt(installPath))
                {
                    status = MecabInitStatus.SUCCESS;
                    return true;
                }
                else
                {
                    status = MecabInitStatus.FAILURE;
                    Deinitialize();
                    return false;
                }
            }
            else
            {
                return status == MecabInitStatus.SUCCESS;
            }
        }

        public static void Deinitialize()
        {
            try
            {
                if (mecab != IntPtr.Zero)
                    mecab_destroy(mecab);
                UnloadLibrary(dll);
            }
            catch (Exception)
            { }
        }

        private static bool initializing = false;
        public static bool Ready()
        {
            return Initialize(null);
        }
        
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
                mecab = mecab_new2("");
                if (mecab == IntPtr.Zero)
                    return false;
                if (!GetEncoding())
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

        private static bool GetEncoding()
        {
            try
            {
                IntPtr info_ptr = mecab_dictionary_info(mecab);
                mecab_dictionary_info_t info = new mecab_dictionary_info_t();
                Marshal.PtrToStructure(info_ptr, info);
                string charset = Marshal.PtrToStringAnsi(info.charset);
                if (charset == "SHIFT-JIS")
                    MecabEncoding = Encoding.GetEncoding(932);
                else if (charset == "EUC-JP")
                    MecabEncoding = Encoding.GetEncoding(20932);
                else if (charset == "UTF-8")
                    MecabEncoding = Encoding.UTF8;
                else
                    throw new Exception(string.Format("MeCab dictionary encoding {0} is not supported.", charset));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static string Translate(string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            mutex.WaitOne();
            try
            {
                byte[] inp = MecabEncoding.GetBytes(source);
                IntPtr outp = mecab_sparse_tostr(mecab, inp);
                string result;
                if (outp != IntPtr.Zero)
                {
                    result = MecabEncoding.GetString(PInvokeFunc.ByteArrayFromPtr(outp));
                }
                else
                {
                    result = null;
                }
                return result;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

    }
}
