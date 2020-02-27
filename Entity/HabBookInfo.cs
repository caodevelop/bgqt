using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class HabAddressGroupInfo
    {
        private List<string> _Members = new List<string>();

        public List<string> Members
        {
            get { return _Members; }
            set { _Members = value; }
        }
        public bool IsOrganizational { get; set; }
        public int SAMAccountType { get; set; }
        public int GroupType { get; set; }
        public bool isParent { get; set; }
        public object iconOpen { get; set; }
        public object iconClose { get; set; }
        public object icon { get; set; }
        public string ObjectGUID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public object Company { get; set; }
        public string CN { get; set; }
        public string ObjectClass { get; set; }
        public string SAMAccountName { get; set; }

        private List<string> _ProxyAddress = new List<string>();
        public List<string> ProxyAddress
        {
            get { return _ProxyAddress; }
            set { _ProxyAddress = value; }
        }
        public string SmtpEmail { get; set; }
        public object SipEmail { get; set; }
        public bool MSExchHideFromAddressLists { get; set; }
        public int HABSeniorityIndex { get; set; }
        public object PostalCode { get; set; }
        public int AdminCount { get; set; }
        public object Description { get; set; }
        public string LegacyExchangeDN { get; set; }
        public string[] Memberof { get; set; }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool HasThumbnailPhoto { get; set; }
        public int ColorIndex { get; set; }
        public object PhotoDisplayText { get; set; }
        public int MsExchRecipientTypeDetails { get; set; }
        public string ExtensionAttribute1 { get; set; }
        public object ExtensionAttribute2 { get; set; }
        public object ExtensionAttribute3 { get; set; }
        public object ExtensionAttribute4 { get; set; }
        public object ExtensionAttribute5 { get; set; }
        public object ExtensionAttribute6 { get; set; }
        public object ExtensionAttribute7 { get; set; }
        public object[] CanonicalName { get; set; }
        public string IpPhone { get; set; }
        public string HomePhone { get; set; }
    }

    [Serializable]
    public class HabAddressUserInfo
    {
        public string Title { get; set; }
        public object Manager { get; set; }
        public string Department { get; set; }
        public object SubMembers { get; set; }
        public string TelePhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string UserPrincipalName { get; set; }
        public bool UserAccountControl { get; set; }
        public string OfficeName { get; set; }
        public object SN { get; set; }
        public object GivenName { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public int MSExchArchiveQuota { get; set; }
        public int MSExchArchiveWarnQuota { get; set; }
        public int MSExchCalendarLoggingQuota { get; set; }
        public int MSExchDumpsterQuota { get; set; }
        public int MSExchDumpsterWarningQuota { get; set; }
        public string HomePage { get; set; }
        public object l { get; set; }
        public object St { get; set; }
        public string StreetAddress { get; set; }
        public string ObjectGUID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string Company { get; set; }
        public string CN { get; set; }
        public string ObjectClass { get; set; }
        public string SAMAccountName { get; set; }

        private List<string> _ProxyAddress = new List<string>();
        public List<string> ProxyAddress
        {
            get { return _ProxyAddress; }
            set { _ProxyAddress = value; }
        }
        public string SmtpEmail { get; set; }
        public object SipEmail { get; set; }
        public bool MSExchHideFromAddressLists { get; set; }
        public int HABSeniorityIndex { get; set; }
        public object PostalCode { get; set; }
        public int AdminCount { get; set; }
        public object Description { get; set; }
        public string LegacyExchangeDN { get; set; }

        private List<string> _Memberof = new List<string>();
        public List<string> Memberof
        {
            get { return _Memberof; }
            set { _Memberof = value; }
        }
        public DateTime WhenChanged { get; set; }
        public DateTime WhenCreated { get; set; }
        public bool HasThumbnailPhoto { get; set; }
        public int ColorIndex { get; set; }
        public object PhotoDisplayText { get; set; }
        public int MsExchRecipientTypeDetails { get; set; }
        public object ExtensionAttribute1 { get; set; }
        public object ExtensionAttribute2 { get; set; }
        public object ExtensionAttribute3 { get; set; }
        public object ExtensionAttribute4 { get; set; }
        public object ExtensionAttribute5 { get; set; }
        public object ExtensionAttribute6 { get; set; }
        public object ExtensionAttribute7 { get; set; }
        public object[] CanonicalName { get; set; }
        public string IpPhone { get; set; }
        public string HomePhone { get; set; }
    }

    [Serializable]
    public class HabListInfo
    {
        private string _error = string.Empty;
        public string error
        {
            get { return _error; }
            set { _error = value; }
        }
        private int _pageIndex = 0;
        public int pageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        private int _pageSize = 0;
        public int pageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _pageCount = 0;
        public int pageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }

        private List<Object> _data = new List<Object>();
        public List<Object> data
        {
            get { return _data; }
            set { _data = value; }
        }
    }

}
