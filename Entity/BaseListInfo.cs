using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class BaseListInfo
    {
        private int _RecordCount = 0;
        public int RecordCount
        {
            get { return _RecordCount; }
            set { _RecordCount = value; }
        }

        private int _PageCount = 0;
        public int PageCount
        {
            get { return _PageCount; }
            set { _PageCount = value; }
        }

        private List<Object> _Lists = new List<Object>();
        public List<Object> Lists
        {
            get { return _Lists; }
            set { _Lists = value; }
        }
    }
}
