using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class SensitiveMailInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Keywords = string.Empty;
        public string Keywords
        {
            get { return _Keywords; }
            set { _Keywords = value; }
        }

        private DateTime _StartTime = DateTime.Now;
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        public string StartTimeName
        {
            get { return _StartTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private DateTime _EndTime = DateTime.Now;
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        public string EndTimeName
        {
            get { return _EndTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

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

        private DateTime _UpdateTime = DateTime.Now;
        public DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { _UpdateTime = value; }
        }

        public string UpdateTimeName
        {
            get { return _UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private Guid _RoleID = Guid.Empty;
        public Guid RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }

        private Guid _ExecuteID = Guid.Empty;
        public Guid ExecuteID
        {
            get { return _ExecuteID; }
            set { _ExecuteID = value; }
        }

        private Guid _CreateUserID = Guid.Empty;
        public Guid CreateUserID
        {
            get { return _CreateUserID; }
            set { _CreateUserID = value; }
        }

        private DateTime _ExecuteTime = DateTime.Now;
        public DateTime ExecuteTime
        {
            get { return _ExecuteTime; }
            set { _ExecuteTime = value; }
        }

        public string ExecuteTimeName
        {
            get
            {
                string _ExecuteTimeName = string.Empty;
                if (_Status == SensitiveMailStatus.End || _Status == SensitiveMailStatus.Failed || _Status == SensitiveMailStatus.Success)
                {
                    _ExecuteTimeName = _ExecuteTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    _ExecuteTimeName = "-";
                }
                return _ExecuteTimeName;
            }
        }

        private string _ExecuteResult = string.Empty;
        public string ExecuteResult
        {
            get { return _ExecuteResult; }
            set { _ExecuteResult = value; }
        }

        private string _PercentageComplete = string.Empty;
        public string PercentageComplete
        {
            get { return _PercentageComplete; }
            set { _PercentageComplete = value; }
        }

        private SensitiveMailType _MailType = SensitiveMailType.SearchMailbox;
        public SensitiveMailType MailType
        {
            get { return _MailType; }
            set { _MailType = value; }
        }

        private List<SensitiveMailObject> _Objects = new List<SensitiveMailObject>();
        public List<SensitiveMailObject> Objects
        {
            get { return _Objects; }
            set { _Objects = value; }
        }

        private List<UserSensitiveMailQueueInfo> _QueueLists = new List<UserSensitiveMailQueueInfo>();
        public List<UserSensitiveMailQueueInfo> QueueLists
        {
            get { return _QueueLists; }
            set { _QueueLists = value; }
        }

        private string _ObjectNames = string.Empty;
        public string ObjectNames
        {
            get { return _ObjectNames; }
            set { _ObjectNames = value; }
        }

        private SensitiveMailStatus _Status = SensitiveMailStatus.Enable;
        public SensitiveMailStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private string _StatusName = string.Empty;
        public string StatusName
        {
            get
            {
                switch (_Status)
                {
                    case SensitiveMailStatus.Enable:
                        _StatusName = "-";
                        break;
                    case SensitiveMailStatus.Submit:
                        _StatusName = "已提交";
                        break;
                    case SensitiveMailStatus.Executing:
                        _StatusName = "执行中";
                        break;
                    case SensitiveMailStatus.Success:
                        _StatusName = "执行成功";
                        break;
                    case SensitiveMailStatus.Failed:
                        _StatusName = "执行失败";
                        break;
                    case SensitiveMailStatus.End:
                        _StatusName = "执行完成";
                        break;
                }
                return _StatusName;
            }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (string.IsNullOrEmpty(_Name))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(_Keywords))
            {
                error.Code = ErrorCode.KeywordsEmpty;
            }
            else if (_Objects.Count <= 0)
            {
                error.Code = ErrorCode.ObjectsEmpty;
            }
            return error;
        }

        public ErrorCodeInfo ChangeCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_ID == null || _ID == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            else if (_Objects.Count <= 0)
            {
                error.Code = ErrorCode.ObjectIDEmpty;
            }
            else if (string.IsNullOrEmpty(_Keywords))
            {
                error.Code = ErrorCode.KeywordsEmpty;
            }

            return error;
        }
    }

    [Serializable]
    public class UserSensitiveMailQueueInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Guid _UserID = Guid.Empty;
        public Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private Guid _SensitiveMailID = Guid.Empty;
        public Guid SensitiveMailID
        {
            get { return _SensitiveMailID; }
            set { _SensitiveMailID = value; }
        }

        private string _Keywords = string.Empty;
        public string Keywords
        {
            get { return _Keywords; }
            set { _Keywords = value; }
        }

        private DateTime _StartTime = DateTime.Now;
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        private DateTime _EndTime = DateTime.Now;
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }

        private SensitiveMailStatus _Status = SensitiveMailStatus.Enable;
        public SensitiveMailStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private Guid _ExecuteID = Guid.Empty;
        public Guid ExecuteID
        {
            get { return _ExecuteID; }
            set { _ExecuteID = value; }
        }

        private string _ExecuteResult = string.Empty;
        public string ExecuteResult
        {
            get { return _ExecuteResult; }
            set { _ExecuteResult = value; }
        }

        private DateTime _ExecuteStartTime = DateTime.Now;
        public DateTime ExecuteStartTime
        {
            get { return _ExecuteStartTime; }
            set { _ExecuteStartTime = value; }
        }

        private DateTime _ExecuteEndTime = DateTime.Now;
        public DateTime ExecuteEndTime
        {
            get { return _ExecuteEndTime; }
            set { _ExecuteEndTime = value; }
        }

        public string ExecuteStartTimeName
        {
            get
            {
                string _ExecuteStartTimeName = string.Empty;
                if (_Status != SensitiveMailStatus.Enable)
                {
                    _ExecuteStartTimeName = _ExecuteStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    _ExecuteStartTimeName = "-";
                }
                return _ExecuteStartTimeName;
            }
        }

        public string ExecuteEndTimeName
        {
            get
            {
                string _ExecuteEndTimeName = string.Empty;
                if (_Status == SensitiveMailStatus.Success || _Status == SensitiveMailStatus.Failed)
                {
                    _ExecuteEndTimeName = _ExecuteStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    _ExecuteEndTimeName = "-";
                }
                return _ExecuteEndTimeName;
            }
        }

        public string StartTimeName
        {
            get { return _StartTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string EndTimeName
        {
            get { return _EndTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
    }

    [Serializable]
    public class SensitiveMailObject
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Guid _SensitiveMailID = Guid.Empty;
        public Guid SensitiveMailID
        {
            get { return _SensitiveMailID; }
            set { _SensitiveMailID = value; }
        }

        private Guid _ObjectID = Guid.Empty;
        public Guid ObjectID
        {
            get { return _ObjectID; }
            set { _ObjectID = value; }
        }

        private NodeType _ObjectType = NodeType.organizationalUnit;
        public NodeType ObjectType
        {
            get { return _ObjectType; }
            set { _ObjectType = value; }
        }

        private string _ObjectName = string.Empty;
        public string ObjectName
        {
            get { return _ObjectName; }
            set { _ObjectName = value; }
        }
    }

    [Serializable]
    public enum SensitiveMailType
    {
        SearchMailbox = 1,
        ComplianceSearch = 2
    }

    [Serializable]
    /// <summary>
    /// Enable = 正常
    /// Submit = 已提交
    /// Executing = 执行中
    /// Success = 成功
    /// Failed = 失败
    /// End = 执行完成
    /// </summary>
    public enum SensitiveMailStatus
    {
        Enable = 1,
        Submit = 2,
        Executing = 3,
        Success = 4,
        Failed = 5,
        End = 6,
    }
}
