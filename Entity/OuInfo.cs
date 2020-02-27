using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class OuInfo
    {
        private string _distinguishedName = string.Empty;
        /// <summary>
        /// AD distinguishedName属性值
        /// </summary>
        public string distinguishedName
        {
            get { return _distinguishedName; }
            set { _distinguishedName = value; }
        }
        private string _name = string.Empty;
        /// <summary>
        /// AD name属性值
        /// </summary>
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _ou;
        /// <summary>
        /// AD ou 属性值
        /// </summary>
        public string ou
        {
            get { return _ou; }
            set { _ou = value; }
        }

        private Guid _id = Guid.Empty;
        public Guid id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Guid _parentid = Guid.Empty;
        public Guid parentid
        {
            get { return _parentid; }
            set { _parentid = value; }
        }

        private string _parentDistinguishedName = string.Empty;
        public string parentDistinguishedName
        {
            get { return _parentDistinguishedName; }
            set { _parentDistinguishedName = value; }
        }

        private string _description = string.Empty;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private bool _IsProfessionalGroups = false;
        public bool IsProfessionalGroups
        {
            get { return _IsProfessionalGroups; }
            set { _IsProfessionalGroups = value; }
        }

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_parentid == null || _parentid == Guid.Empty)
            {
                error.Code = ErrorCode.ParentIdEmpty;
            }
            if (string.IsNullOrEmpty(_name))
            {
                error.Code = ErrorCode.NameEmpty;
            }

            return error;
        }

        public ErrorCodeInfo InterfaceAddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (string.IsNullOrEmpty(_parentDistinguishedName))
            {
                error.Code = ErrorCode.ParentDnEmpty;
            }
            if (string.IsNullOrEmpty(_name))
            {
                error.Code = ErrorCode.NameEmpty;
            }

            return error;
        }

        public ErrorCodeInfo ModifyCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (_id == null || _id == Guid.Empty)
            {
                error.Code = ErrorCode.IdEmpty;
            }
            else if (string.IsNullOrEmpty(_name))
            {
                error.Code = ErrorCode.NameEmpty;
            }

            return error;
        }
    }
}
