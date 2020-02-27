using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class NodeInfo
    {
        private int _MemberCount = 0;
        public int MemberCount
        {
            get
            {
                return _MemberCount;
            }
            set
            {
                _MemberCount = value;
            }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private Guid _NodeID = Guid.Empty;
        public Guid NodeID
        {
            get
            {
                return _NodeID;
            }
            set
            {
                _NodeID = value;
            }
        }

        public String Desc
        {
            get; set;
        } = String.Empty;

        private string _UserAccount = string.Empty;
        public string UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                _UserAccount = value;
            }
        }

        private bool _isParent = false;
        public bool isParent
        {
            get
            {
                return _isParent;
            }
            set
            {
                _isParent = value;
            }
        }

        private bool _status = true;
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        private string _Path = string.Empty;
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        private NodeType _Type = NodeType.group;
        public NodeType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private bool _IsRoot = false;
        public bool IsRoot
        {
            get
            {
                return _IsRoot;
            }
            set
            {
                _IsRoot = value;
            }
        }

        public String id
        {
            get
            {
                return _NodeID.ToString();
            }
        }

        public String type
        {
            get
            {
                return Convert.ToUInt32(_Type).ToString();
            }
        }

        public String name
        {
            get
            {
                return _Name;
            }
        }
    }

    [Serializable]
    public class HabNodeInfo
    {
        private int _MemberCount = 0;
        public int MemberCount
        {
            get
            {
                return _MemberCount;
            }
            set
            {
                _MemberCount = value;
            }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private Guid _NodeID = Guid.Empty;
        public Guid NodeID
        {
            get
            {
                return _NodeID;
            }
            set
            {
                _NodeID = value;
            }
        }

        private string _UserAccount = string.Empty;
        public string UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                _UserAccount = value;
            }
        }

        private bool _isParent = false;
        public bool isParent
        {
            get
            {
                return _isParent;
            }
            set
            {
                _isParent = value;
            }
        }

        private bool _isRoot = false;
        public bool isRoot
        {
            get
            {
                return _isRoot;
            }
            set
            {
                _isRoot = value;
            }
        }

        private bool _status = true;
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        private string _Path = string.Empty;
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        private NodeType _Type = NodeType.group;
        public NodeType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private int _Index = 0;
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }

        public String id
        {
            get
            {
                return _NodeID.ToString();
            }
        }

        public String type
        {
            get
            {
                return Convert.ToUInt32(_Type).ToString();
            }
        }

        public String name
        {
            get
            {
                return _Name;
            }
        }
    }

    [Serializable]
    public class MoveNodeInfo
    {
        private NodeInfo _TargetNode = new NodeInfo();
        public NodeInfo TargetNode
        {
            get
            {
                return _TargetNode;
            }
            set
            {
                _TargetNode = value;
            }
        }

        private List<NodeInfo> _NodeList = new List<NodeInfo>();
        public List<NodeInfo> NodeList
        {
            get
            {
                return _NodeList;
            }
            set
            {
                _NodeList = value;
            }
        }
    }

    [Serializable]
    public enum NodeType
    {
        organizationalUnit = 1,
        user = 2,
        group = 3,
        habgroup =4,
    }
}

