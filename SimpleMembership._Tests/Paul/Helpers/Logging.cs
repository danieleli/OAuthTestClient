using System;
using System.Collections.Generic;
using log4net;
using System.IO;
using log4net.Config;
using System.Globalization;
using System.Threading.Tasks;

namespace PPS.API.Common.Helpers
{
    //public static class Logger<T> where T : class
    //{
    ////    internal static ILog logger(Type declaringType)
    ////    {
    ////        //if (!_logs.ContainsKey(declaringType))
    ////        //    _logs.Add(declaringType, );

    ////        //return _logs[declaringType];
    ////    }
    //    private static Dictionary<Type, ILog> _logs = new Dictionary<Type, ILog>();

    //    public static void Debug(string msg)
    //    {
    //        Task.Factory.StartNew(() => log.Info("My Info"));


    //        Debug(msg);
    //    }
    //    public static void Debug(string msg) 
    //    {
    //        LogManager.GetLogger(declaringType).Debug(msg);
    //    }
    //    public static void Debug(string msg, Exception ex)
    //    {
    //        Debug(AppDomain.CurrentDomain.C, msg, ex);
    //    }
    //    public static void Debug(Type declaringType, string msg, Exception ex)
    //    {
    //        LogManager.GetLogger(declaringType).Debug(msg);
    //    }

    //    public static void DebugFormat(string format, params object[] args)
    //    {
    //        DebugFormat(Constants.Logs.Default, format, args);
    //    }
    //    public static void DebugFormat(Type declaringType, string format, params object[] args)
    //    {
    //        LogManager.GetLogger(declaringType).DebugFormat(format, args);
    //    }
    //}

    /// <summary>
    /// Log4Net wrapper, lifted from codecampserver (http://code.google.com/p/codecampserver/source/list)
    /// </summary>
    public static class Logger
    {
        static Logger()
        {
            initialize();
        }

        private static void initialize()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(
                Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "bin\\log4net.config")));
        }

        private static readonly Dictionary<Type, ILog> _loggers = new Dictionary<Type, ILog>();
        private static readonly object _lock = new object();

        //public static string SerializeException(Exception exception)
        //{
        //    return SerializeException(exception, string.Empty);
        //}

        //private static string SerializeException(Exception e, string exceptionMessage)
        //{
        //    if (e == null) return string.Empty;

        //    exceptionMessage = string.Format("{0}{1}{2}\n{3}",
        //                                     exceptionMessage,
        //                                     string.IsNullOrEmpty(exceptionMessage) ? string.Empty : "\n\n",
        //                                     e.Message,
        //                                     e.StackTrace);

        //    if (e.InnerException != null)
        //        exceptionMessage = SerializeException(e.InnerException, exceptionMessage);

        //    return exceptionMessage;
        //}

        private static ILog getLogger(Type source)
        {
            lock (_lock)
            {
                if (_loggers.ContainsKey(source))
                {
                    return _loggers[source];
                }
                else
                {
                    ILog logger = LogManager.GetLogger(source);
                    _loggers.Add(source, logger);
                    return logger;
                }
            }
        }

        /* Log a message object */
        public static void Debug(object source, object message)
        {
            Debug(source.GetType(), message);
        }
        public static void Debug(object source, object message, params object[] ps)
        {
            Debug(source.GetType(), string.Format(message.ToString(), ps));
        }
        public static void Debug(Type source, object message)
        {
            Task.Factory.StartNew(() =>
            {
                ILog logger = getLogger(source);
                if (logger.IsDebugEnabled)
                    logger.Debug(message);
            });
            
        }

        public static void Info(object source, object message)
        {
            Info(source.GetType(), message);
        }
        public static void Info(Type source, object message)
        {
            Task.Factory.StartNew(() =>
            {
                ILog logger = getLogger(source);
                if (logger.IsInfoEnabled)
                    logger.Info(message);
            });
        }

        public static void Warn(object source, object message)
        {
            Warn(source.GetType(), message);
        }
        public static void Warn(Type source, object message)
        {
            Task.Factory.StartNew(() =>
            {
                ILog logger = getLogger(source);
                if (logger.IsWarnEnabled)
                    logger.Warn(message);
            });
        }

        public static void Error(object source, object message)
        {
            Error(source.GetType(), message);
        }
        public static void Error(Type source, object message)
        {
            Task.Factory.StartNew(() =>
            {
                ILog logger = getLogger(source);
                if (logger.IsErrorEnabled)
                    logger.Error(message);
            });
        }

        public static void Fatal(object source, object message)
        {
            Fatal(source.GetType(), message);
        }
        public static void Fatal(Type source, object message)
        {
            Task.Factory.StartNew(() =>
            {
                ILog logger = getLogger(source);
                if (logger.IsFatalEnabled)
                    logger.Fatal(message);
            });
        }

        /* Log a message object and exception */

        public static void Debug(object source, object message, Exception exception)
        {
            Debug(source.GetType(), message, exception);
        }
        public static void Debug(Type source, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                getLogger(source).Debug(message, exception);
            });
        }

        public static void Info(object source, object message, Exception exception)
        {
            Info(source.GetType(), message, exception);
        }
        public static void Info(Type source, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                getLogger(source).Info(message, exception);
            });
        }

        public static void Warn(object source, object message, Exception exception)
        {
            Warn(source.GetType(), message, exception);
        }
        public static void Warn(Type source, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                getLogger(source).Warn(message, exception);
            });
        }

        public static void Error(object source, object message, Exception exception)
        {
            Error(source.GetType(), message, exception);
        }
        public static void Error(Type source, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                getLogger(source).Error(message, exception);
            });
        }

        public static void Fatal(object source, object message, Exception exception)
        {
            Fatal(source.GetType(), message, exception);
        }
        public static void Fatal(Type source, object message, Exception exception)
        {
            Task.Factory.StartNew(() =>
            {
                getLogger(source).Fatal(message, exception);
            });
        }

    }
}
