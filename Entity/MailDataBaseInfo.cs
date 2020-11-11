using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class MailDataBaseInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Guid _OuID = Guid.Empty;
        public Guid OuID
        {
            get { return _OuID; }
            set { _OuID = value; }
        }

        private string _OuName = string.Empty;
        public string OuName
        {
            get { return _OuName; }
            set { _OuName = value; }
        }

        private string _OUdistinguishedName = string.Empty;
        public string OUdistinguishedName
        {
            get { return _OUdistinguishedName; }
            set { _OUdistinguishedName = value; }
        }

        private Guid _MailboxDBID = Guid.Empty;
        public Guid MailboxDBID
        {
            get { return _MailboxDBID; }
            set { _MailboxDBID = value; }
        }


        private string _MailboxDB = string.Empty;
        public string MailboxDB
        {
            get { return _MailboxDB; }
            set { _MailboxDB = value; }
        }

        private string _MailboxServer = string.Empty;
        public string MailboxServer
        {
            get { return _MailboxServer; }
            set { _MailboxServer = value; }
        }

        public string MailSizeName
        { get; set; } = string.Empty;

        public long MailSize
        { get; set; } = 0;

        private DateTime _CreateTime = DateTime.Now;
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }

        public string CreateTimeName
        {
            get { return _CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private State _Status = State.Enable;
        public State Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_OuID == null || _OuID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (string.IsNullOrEmpty(_MailboxDB))
            {
                error.Code = ErrorCode.MailboxDBEmpty;
            }

            return error;
        }

        public ErrorCodeInfo ModifyCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_ID == null || _ID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (_OuID == null || _OuID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            if (string.IsNullOrEmpty(_MailboxDB))
            {
                error.Code = ErrorCode.MailboxDBEmpty;
            }

            return error;
        }
    }

    [Serializable]
    public enum State
    {
        Enable =1,
        Delete =-1,
        Disable =2,
    }
}
