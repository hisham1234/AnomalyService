using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace AnomalyService.Helpers
{
    public class LoggerHelper
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
        public string getMessage(string methodName)
        {
            var stackTrace = new StackTrace();
            var logmsg = "| " + Assembly.GetCallingAssembly().GetName().Name + " | " + methodName + " Called!" ;
            return logmsg;
        }
        public string getMessage(string methodName,int statusCode)
        {
            var stackTrace = new StackTrace();
            var logmsg = "| " + Assembly.GetCallingAssembly().GetName().Name + " | " + methodName + " returned Code "+statusCode + "!";
            return logmsg;
        }

        public string getMessage(string methodName,string msg)
        {
           
            var logmsg = "| " + Assembly.GetCallingAssembly().GetName().Name + " | [Error]" + msg +"!";
            return logmsg;
        }
    }
}
