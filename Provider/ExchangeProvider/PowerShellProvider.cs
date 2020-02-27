using Common;
using Provider.ADProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ExchangeProvider
{
    #region PSCommand Class
    public abstract class PSCommandBase
    {
        protected static PSSnapInException _psSnapinException;
        protected static Runspace _runspace;
        protected static RunspaceConfiguration _runspaceConfiguration;

        protected static string EXCHANGE_MANAGEMENT_ADMIN = "Microsoft.Exchange.Management.PowerShell.SnapIn";
        protected static object sync = new object();

        #region 数据库读取
        //protected static string PreferDC = ConfigADProvider.GetADDC();
        //protected static string DomainAdmin = ConfigADProvider.GetADAdmin();
        //protected static string Domain = ConfigADProvider.GetADDomain();
        //protected static string DomainAdminPass = ConfigADProvider.GetADPassword();
        #endregion

        protected static string PreferDC = ConfigurationManager.AppSettings["AD_DC"];
        protected static string DomainAdmin = ConfigurationManager.AppSettings["AD_Admin"];
        protected static string Domain = ConfigurationManager.AppSettings["AD_Domain"];
        protected static string DomainAdminPass = ConfigurationManager.AppSettings["AD_Password"];
       
        static PSCommandBase()
        {
            lock (sync)
            {
                if (_runspace != null && _runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                {
                    _runspace.Close();
                    _runspace.Dispose();
                }
                _runspaceConfiguration = RunspaceConfiguration.Create();
                _runspaceConfiguration.AddPSSnapIn(EXCHANGE_MANAGEMENT_ADMIN, out _psSnapinException);
                _runspace = RunspaceFactory.CreateRunspace(_runspaceConfiguration);
                _runspace.Open();

               // _runspace.SessionStateProxy.SetVariable("ConfirmPreference", "none");
            }
        }

        private static void ReOpen()
        {
            lock (sync)
            {
                #region modfiy by 2016/09/01
                if (_runspace != null && _runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                {
                    _runspace.Close();
                    _runspace.Dispose();
                }
                _runspaceConfiguration = RunspaceConfiguration.Create();
                _runspaceConfiguration.AddPSSnapIn(EXCHANGE_MANAGEMENT_ADMIN, out _psSnapinException);
                _runspace = RunspaceFactory.CreateRunspace(_runspaceConfiguration);
                _runspace.Open();

                // _runspace.SessionStateProxy.SetVariable("ConfirmPreference", "none");

                #endregion
            }
        }

        protected static ICollection<PSObject> ExecuteCmdlet(string cmdlet, PSParameters parms)
        {
            parms.Add(new PSParameter("DomainController", PreferDC));
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            if (cmdlet.Length == 0)
            {
                throw new ArgumentException("cmdlet length is zero", "cmdlet");
            }

            Command item = new Command(cmdlet);
            foreach (PSParameter parameter in parms)
            {
                item.Parameters.Add(parameter.Name, parameter.Value);
            }

            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            PipelineReader<object> error = null;
            PSObject obj2 = null;
            ErrorRecord baseObject = null;
            try
            {
                lock (sync)
                {
                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.Add(item);
                            is2 = pipeline.Invoke();
                            error = pipeline.Error;
                            if (error.Count == 1)
                            {
                                obj2 = (PSObject)error.Read();
                                baseObject = (ErrorRecord)obj2.BaseObject;
                                throw baseObject.Exception;
                            }
                            if (error.Count <= 1)
                            {
                                return is2;
                            }
                            int count = error.Count;
                            int num2 = 0;
                            ErrorRecord record2 = null;
                            while (error.Count > 0)
                            {
                                obj2 = (PSObject)error.Read();
                                baseObject = (ErrorRecord)obj2.BaseObject;
                                if (record2 == null)
                                {
                                    record2 = baseObject;
                                }
                                num2++;

                            }
                            throw record2.Exception;
                        }
                    }
                    return is2;
                }
            }
            finally
            {
                pipeline = null;
            }
        }

        public static ICollection<PSObject> ExecuteCmdlet(string cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            if (cmdlet.Length == 0)
            {
                throw new ArgumentException("cmdlet length is zero", "cmdlet");
            }

            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            List<object> errorList = new List<object>();
            try
            {
                lock (sync)
                {
                    if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                    {
                        _runspace.Close();
                        ReOpen();
                    }
                    else if (_runspace.RunspaceStateInfo.State == RunspaceState.Closed)
                    {
                        ReOpen();
                    }

                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.AddScript(cmdlet);
                            is2 = pipeline.Invoke();

                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                foreach (object item1 in pipeline.Error.ReadToEnd())
                                {
                                    errorList.Add(item1);
                                    string errorMessage = string.Format("Invoke error: {0}", item1);
                                    Log4netHelper.Error(errorMessage);
                                }
                            }
                        }
                    }

                }
            }
            finally
            {
                pipeline = null;
            }

            return is2;
        }
    }

    public class PSParameters : List<PSParameter>
    {
        public void AddPara(string key, object value)
        {
            this.Add(new PSParameter(key, value));
        }
    }

    public class PSParameter
    {
        private string _name;
        private object _value;
        public PSParameter(string name, object value)
        {
            this._name = name;
            this._value = value;
        }

        public PSParameter(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }
        public object Value
        {
            get
            {
                return this._value;
            }
        }
    }
    
    #endregion

    #region WSCommand Class
    public class PSCommandCom : WSCommandBase
    {
        public new static ICollection<PSObject> ExecuteCmdlet(string cmdlet, PSParameters parms)
        {
            return WSCommandBase.ExecuteCmdlet(cmdlet, parms);
        }
    }

    public abstract class WSCommandBase
    {
        protected static PSSnapInException _psSnapinException;
        protected static Runspace _runspace;
        //protected static RunspaceConfiguration _runspaceConfiguration;
        //protected static string EXCHANGE_MANAGEMENT_ADMIN = "Microsoft.Exchange.Management.PowerShell.Admin";
        protected static object sync = new object();
        #region 数据库读取
        //protected static string PreferDC = ConfigADProvider.GetADDC();
        ////protected static string DomainAdmin = ConfigADProvider.GetADAdmin();
        ////protected static string Domain = ConfigADProvider.GetADDomain();
        ////protected static string DomainAdminPass = ConfigADProvider.GetADPassword();
        ////protected static string PowershellUri = ConfigADProvider.GetExchangePSUri(); //ConfigurationManager.AppSettings["ExchangePS_Uri"];
        #endregion

        protected static string PreferDC = ConfigurationManager.AppSettings["AD_DC"];
        protected static string DomainAdmin = ConfigurationManager.AppSettings["AD_Admin"];
        protected static string Domain = ConfigurationManager.AppSettings["AD_Domain"];
        protected static string DomainAdminPass = ConfigurationManager.AppSettings["AD_Password"];
        protected static string PowershellUri = ConfigurationManager.AppSettings["ExchangePS_Uri"];

        static WSCommandBase()
        {
            lock (sync)
            {
                System.Security.SecureString secureString = new System.Security.SecureString();

                foreach (char pwd in DomainAdminPass.ToCharArray())
                {
                    secureString.AppendChar(pwd);
                }

                PSCredential credential = new PSCredential(DomainAdmin, secureString);
                var wsConnectionInfo = new WSManConnectionInfo(new Uri(PowershellUri),
                                                "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credential);

                wsConnectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;

                _runspace = RunspaceFactory.CreateRunspace(wsConnectionInfo);
                _runspace.Open();
                // _runspace.SessionStateProxy.SetVariable("ConfirmPreference", "none");
            }
        }

        private static void ReOpen()
        {
            lock (sync)
            {
                #region modfiy by 2016/09/01
                System.Security.SecureString secureString = new System.Security.SecureString();

                foreach (char pwd in DomainAdminPass.ToCharArray())
                {
                    secureString.AppendChar(pwd);
                }

                PSCredential credential = new PSCredential(DomainAdmin, secureString);
                var wsConnectionInfo = new WSManConnectionInfo(new Uri(PowershellUri),
                                                "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credential);
                wsConnectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;

                _runspace = RunspaceFactory.CreateRunspace(wsConnectionInfo);
                _runspace.Open();
                #endregion
                // _runspace.SessionStateProxy.SetVariable("ConfirmPreference", "none");
            }
        }
        protected static ICollection<PSObject> ExecuteCmdlet(string cmdlet, PSParameters parms)
        {
            //parms.Add(new PSParameter("DomainController", PreferDC));
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            if (cmdlet.Length == 0)
            {
                throw new ArgumentException("cmdlet length is zero", "cmdlet");
            }

            Command item = new Command(cmdlet);
            foreach (PSParameter parameter in parms)
            {
                item.Parameters.Add(parameter.Name, parameter.Value);
            }

            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            List<object> errorList = new List<object>();

            try
            {
                lock (sync)
                {
                    if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                    {
                        _runspace.Close();
                        ReOpen();
                    }
                    else if (_runspace.RunspaceStateInfo.State == RunspaceState.Closed)
                    {
                        // _runspace.Close();
                        ReOpen();
                    }

                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.Add(item);
                            is2 = pipeline.Invoke();

                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                foreach (object item1 in pipeline.Error.ReadToEnd())
                                {
                                    errorList.Add(item1);
                                    string errorMessage = string.Format("Invoke error: {0}", item1);
                                    Log4netHelper.Error(errorMessage);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                pipeline = null;
            }
            return is2;
        }

        public static ICollection<PSObject> ExecuteCmdlet(string cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            if (cmdlet.Length == 0)
            {
                throw new ArgumentException("cmdlet length is zero", "cmdlet");
            }

            //Command item = new Command(cmdlet);
            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            List<object> errorList = new List<object>();
            try
            {
                lock (sync)
                {
                    if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                    {
                        _runspace.Close();
                        ReOpen();
                    }

                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.AddScript(cmdlet);
                            is2 = pipeline.Invoke();

                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                foreach (object item1 in pipeline.Error.ReadToEnd())
                                {
                                    errorList.Add(item1);
                                    string errorMessage = string.Format("Invoke error: {0}", item1);
                                    Log4netHelper.Error(errorMessage);
                                }
                            }
                        }
                    }
                }
            }

            finally
            {
                pipeline = null;
            }

            return is2;
        }

        protected static ICollection<PSObject> ExecuteCmdlet(string cmdlet,bool isNeedDC,  PSParameters parms)
        {
            if (isNeedDC)
            {
                //parms.Add(new PSParameter("DomainController", PreferDC));
            }
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            if (cmdlet.Length == 0)
            {
                throw new ArgumentException("cmdlet length is zero", "cmdlet");
            }

            Command item = new Command(cmdlet);
            foreach (PSParameter parameter in parms)
            {
                if (parameter.Value == null)
                {
                    item.Parameters.Add(parameter.Name);
                }
                else
                {
                    item.Parameters.Add(parameter.Name, parameter.Value);
                }
                
            }

            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            List<object> errorList = new List<object>();

            try
            {
                lock (sync)
                {
                    if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                    {
                        _runspace.Close();
                        ReOpen();
                    }

                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.Add(item);
                            is2 = pipeline.Invoke();

                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                foreach (object item1 in pipeline.Error.ReadToEnd())
                                {
                                    errorList.Add(item1);
                                    string errorMessage = string.Format("Invoke error: {0}", item1);
                                    Log4netHelper.Error(errorMessage);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                pipeline = null;
            }
            return is2;
        }

         protected static ICollection<PSObject> ExecuteCmdlet(bool isNeedDC, Command cmd)
        {
            if (isNeedDC)
            {
                //CommandParameter dc = new CommandParameter("DomainController", PreferDC);
                //cmd.Parameters.Add(dc);
            }
           
            ICollection<PSObject> is2 = null;
            Pipeline pipeline = null;
            List<object> errorList = new List<object>();

            try
            {
                lock (sync)
                {
                    if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
                    {
                        _runspace.Close();
                        ReOpen();
                    }

                    SubmitSecurity subSecurity = new SubmitSecurity();
                    if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
                    {
                        pipeline = _runspace.CreatePipeline();
                        using (Pipeline pipeline2 = pipeline)
                        {
                            pipeline.Commands.Add(cmd);
                            is2 = pipeline.Invoke();

                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                foreach (object item1 in pipeline.Error.ReadToEnd())
                                {
                                    errorList.Add(item1);
                                    string errorMessage = string.Format("Invoke error: {0}", item1);
                                    Log4netHelper.Error(errorMessage);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                pipeline = null;
            }
            return is2;
        }

        //public static ICollection<PSObject> ExecuteCmdlet(string cmdlet)
        //{
        //    if (cmdlet == null)
        //    {
        //        throw new ArgumentNullException("cmdlet");
        //    }
        //    if (cmdlet.Length == 0)
        //    {
        //        throw new ArgumentException("cmdlet length is zero", "cmdlet");
        //    }

        //    Command item = new Command(cmdlet);
        //    ICollection<PSObject> is2 = null;
        //    Pipeline pipeline = null;
        //    List<object> errorList = new List<object>();
        //    try
        //    {
        //        lock (sync)
        //        {
        //            if (_runspace.RunspaceStateInfo.State == RunspaceState.Broken)
        //            {
        //                _runspace.Close();
        //                ReOpen();
        //            }

        //            SubmitSecurity subSecurity = new SubmitSecurity();
        //            if (subSecurity.impersonateValidUser(DomainAdmin, Domain, DomainAdminPass))
        //            {
        //                pipeline = _runspace.CreatePipeline();
        //                using (Pipeline pipeline2 = pipeline)
        //                {
        //                    pipeline.Commands.AddScript(cmdlet)
        //                    is2 = pipeline.Invoke();

        //                    if (pipeline.Error != null && pipeline.Error.Count > 0)
        //                    {
        //                        foreach (object item1 in pipeline.Error.ReadToEnd())
        //                        {
        //                            errorList.Add(item1);
        //                            string errorMessage = string.Format("Invoke error: {0}", item1);
        //                            Log4netHelper.Error(errorMessage);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    finally
        //    {
        //        pipeline = null;
        //    }

        //    return is2;
        //}
    }

    #endregion
}
