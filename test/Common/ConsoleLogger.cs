/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Common
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using log4net;

  public class ConsoleLogger : ILog
  {
    public void Debug(object message, Exception exception)
    {
      throw new NotImplementedException();
    }

    public void Debug(object message)
    {
      throw new NotImplementedException();
    }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void DebugFormat(string format, object arg0, object arg1, object arg2)
    {
      throw new NotImplementedException();
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
      throw new NotImplementedException();
    }

    public void DebugFormat(string format, object arg0)
    {
      throw new NotImplementedException();
    }

    public void DebugFormat(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void Error(object message, Exception exception)
    {
      Console.WriteLine("ERROR: {0}\n{1}", message, exception);
    }

    public void Error(object message)
    {
      throw new NotImplementedException();
    }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2)
    {
      throw new NotImplementedException();
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
      throw new NotImplementedException();
    }

    public void ErrorFormat(string format, object arg0)
    {
      throw new NotImplementedException();
    }

    public void ErrorFormat(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void Fatal(object message, Exception exception)
    {
      throw new NotImplementedException();
    }

    public void Fatal(object message)
    {
      throw new NotImplementedException();
    }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void FatalFormat(string format, object arg0, object arg1, object arg2)
    {
      throw new NotImplementedException();
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
      throw new NotImplementedException();
    }

    public void FatalFormat(string format, object arg0)
    {
      throw new NotImplementedException();
    }

    public void FatalFormat(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void Info(object message, Exception exception)
    {
      throw new NotImplementedException();
    }

    public void Info(object message)
    {
      throw new NotImplementedException();
    }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
      throw new NotImplementedException();
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
      throw new NotImplementedException();
    }

    public void InfoFormat(string format, object arg0)
    {
      throw new NotImplementedException();
    }

    public void InfoFormat(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public bool IsDebugEnabled
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsErrorEnabled
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsFatalEnabled
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsInfoEnabled
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsWarnEnabled
    {
      get { throw new NotImplementedException(); }
    }

    public void Warn(object message, Exception exception)
    {
      throw new NotImplementedException();
    }

    public void Warn(object message)
    {
      throw new NotImplementedException();
    }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void WarnFormat(string format, object arg0, object arg1, object arg2)
    {
      throw new NotImplementedException();
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
      throw new NotImplementedException();
    }

    public void WarnFormat(string format, object arg0)
    {
      throw new NotImplementedException();
    }

    public void WarnFormat(string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public log4net.Core.ILogger Logger
    {
      get { throw new NotImplementedException(); }
    }
  }
}
