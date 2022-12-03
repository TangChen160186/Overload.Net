using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace OvDebug
{
    public class OvLogger
    {
        private readonly NLog.Logger _logger;

        private OvLogger(NLog.Logger logger)
        {
            _logger = logger;
        }

        public static OvLogger Default { get; private set; }

        static OvLogger()
        {

            Default = new OvLogger(NLog.LogManager.GetCurrentClassLogger());
        }

        #region Debug

        public void Debug(string msg, params object[] args)
        {
            _logger.Debug(msg, args);
        }

        public void Debug(string msg, Exception err)
        {
            _logger.Debug(err, msg);
        }

        #endregion

        #region Info

        public void Info(string msg, params object[] args)
        {
            _logger.Info(msg, args);
        }

        public void Info(string msg, Exception err)
        {
            _logger.Info(err, msg);
        }

        #endregion

        #region Warn

        public void Warn(string msg, params object[] args)
        {
            _logger.Warn(msg, args);
        }

        public void Warn(string msg, Exception err)
        {
            _logger.Warn(err, msg);
        }

        #endregion

        #region Trace

        public void Trace(string msg, params object[] args)
        {
            _logger.Trace(msg, args);
        }

        public void Trace(string msg, Exception err)
        {
            _logger.Trace(err, msg);
        }

        #endregion

        #region Error

        public void Error(string msg, params object[] args)
        {
            _logger.Error(msg, args);
        }

        public void Error(string msg, Exception err)
        {
            _logger.Error(err, msg);
        }

        #endregion

        #region Fatal

        public void Fatal(string msg, params object[] args)
        {
            _logger.Fatal(msg, args);
        }

        public void Fatal(string msg, Exception err)
        {
            _logger.Fatal(err, msg);
        }
        #endregion
    }
}
