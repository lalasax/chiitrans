using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Threading;

namespace ChiiTrans
{
    class TranslationTask
    {
        private Thread worker;
        private Translation translation;
        private string name;
        private MethodInfo method;

        public const int IN_PROGRESS = 0;
        public const int COMPLETED = 1;
        public const int FROM_CACHE = 2;
        public const int ERROR = -1;
        
        public TranslationTask(Translation translation, string name, MethodInfo method, bool custom)
        {
            this.translation = translation;
            this.name = name;
            this.method = method;

            if (custom)
            {
                worker = new Thread(customTranslateProc);
            }
            else
            {
                if (Global.options.useCache && !name.StartsWith("Hivemind"))
                {
                    string res = translation.findCache(name);
                    if (res != null)
                    {
                        Global.RunScript("UpdateTranslation", translation.id, name, res, FROM_CACHE);
                        translation.CompleteTask();
                        return;
                    }
                }
                if (name == "Atlas" || name == "Translit (MeCab)")
                {
                    translateProc();
                    return;
                }
                else
                {
                    worker = new Thread(translateProc);
                }
            }
            worker.IsBackground = true;
            worker.Start();
        }

        void translateProc()
        {
            int status = 0;
            //Global.RunScript("UpdateTranslation", translation.id, name, "...", status);
            string result;
            try
            {
                result = (string)method.Invoke(translation, null);
                if (Global.options.translateToOtherLanguage && !name.StartsWith("Translit") && name != "Google")
                {
                    result = translation.SecondTranslate(result, Global.options.translateLanguage);
                }
                if (Global.options.useCache)
                {
                    translation.storeCache(name, result);
                }
                status = COMPLETED;
            }
            catch (Exception ex)
            {
                Exception e2 = ex.InnerException;
                if (e2 != null && !string.IsNullOrEmpty(e2.Message))
                {
                    result = "(" + e2.Message + ")";
                }
                else
                {
                    result = "(Error)";
                }
                status = ERROR;
            }

            Global.RunScript("UpdateTranslation", translation.id, name, result, status);
            translation.CompleteTask();
        }

        void customTranslateProc()
        {
            try
            {
                method.Invoke(translation, null);
            }
            catch (Exception) { }
            translation.CompleteTask();
        }

        public void Abort()
        {
            if (worker.IsAlive)
                worker.Abort();
        }
    }
}
