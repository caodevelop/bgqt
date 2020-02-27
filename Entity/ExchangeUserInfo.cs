using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    /// <summary>
    /// 类名：定义用户Service服务相关属性实体类
    /// 版本号：V1.0
    /// 创建人：cs
    /// 创建时间：2008-07-23
    /// 版权：www.hostchn.com
    /// 类名：OrganizationManager 
    /// 和用户Exchange+OCS服务参数相关的实体类
    /// </summary>
    public class ExchangeUserInfo
    {
        public ExchangeUserInfo()
        {
        }

        #region 必要参数,创建用户邮箱必须设定
        private string m_strUserName = string.Empty;
        /// <summary>
        /// 用户帐号，组织内唯一标识用户，一般格式为：user@domain.com； 
        /// </summary>
        public string userName
        {
            get
            {
                return m_strUserName;
            }
            set
            {
                m_strUserName = value;
            }
        }

        private string m_strOrganization = string.Empty;
        /// <summary>
        /// 用户的组织名称，唯一标识一个组织，一般格式为：mydomain.com
        /// 可以对应用户的OU
        /// </summary>
        public string Organization
        {
            get
            {
                return m_strOrganization;
            }
            set
            {
                m_strOrganization = value;
            }
        }

        private string m_strExchangePlanName = string.Empty;
        /// <summary>
        /// 分配给用户的Email服务类型(产品包)
        /// </summary>
        public string exchangePlanName
        {
            set
            {
                m_strExchangePlanName = value;
            }
            get
            {
                return m_strExchangePlanName;
            }
        }

        private string m_strOCSPlanName = string.Empty;
        /// <summary>
        /// 分配给用户的Email服务类型(产品包)
        /// </summary>
        public string ocsPlanName
        {
            set
            {
                m_strOCSPlanName = value;
            }
            get
            {
                return m_strOCSPlanName;
            }
        }

        private int m_nMailSize = 0;
        /// <summary>
        /// 分配给用户的邮箱空间(单位MB),默认设置为禁止收(发)的空间
        /// </summary>
        public int mailSize
        {
            set { m_nMailSize = value; }
            get { return m_nMailSize; }
        }


        private ArrayList m_emailAddresses = new ArrayList();
        /// <summary>
        /// 用户的邮箱地址集合,默认情况下只会有一个邮箱地址
        /// 如果用户有多个别名,需要列出所有的别名邮件地址,
        /// 为用户所有邮件地址的集合(包含别名地址)
        /// </summary>
        public ArrayList emailAddresses
        {
            set
            {
                m_emailAddresses = value;
            }
            get
            {
                return m_emailAddresses;
            }
        }

        private string m_PrimaryURI = string.Empty;
        /// <summary>
        /// 用户的OCS地址,默认情况下与邮箱地址相同
        /// </summary>
        public string PrimaryURI
        {
            set
            {
                m_PrimaryURI = value;
            }
            get
            {
                return m_PrimaryURI;
            }
        }

        private string m_LineURI = string.Empty;
        /// <summary>
        /// OCS号码
        /// </summary>
        public string LineURI
        {
            set
            {
                m_LineURI = value;
            }
            get
            {
                return m_LineURI;
            }
        }

        public string m_UMNumber = string.Empty;
        /// <summary>
        /// UM邮箱号码
        /// </summary>
        public string UMNumber
        {
            set
            {
                m_UMNumber = value;
            }
            get
            {
                return m_UMNumber;
            }
        }


        private string m_poolFQDN = string.Empty;
        /// <summary>
        /// ECPPool
        /// </summary>
        public string PoolFQDN
        {
            set
            {
                m_poolFQDN = value;
            }
            get
            {
                return m_poolFQDN;
            }
        }

        #endregion

        #region 设置邮箱空间参数(可选)
        private int m_nWarnMailSize = 0;
        /// <summary>
        /// 分配给用户的邮箱警告空间(单位MB),如果用户不设置值,采用和邮箱空间相同的值
        /// </summary>
        public int WarnMailSize
        {
            set { m_nWarnMailSize = value; }
            get { return m_nWarnMailSize; }
        }

        private int m_nProhibitSendMailSize = 0;
        /// <summary>
        /// 分配给用户的禁止用户发送邮件的邮箱空间(单位MB),如果用户不设置值,采用和邮箱空间相同的值
        /// </summary>
        public int ProhibitSendMailSize
        {
            set { m_nProhibitSendMailSize = value; }
            get { return m_nProhibitSendMailSize; }
        }

        private int m_nMaxAttachFileSize = 0;
        /// <summary>
        /// 最大可接收和发送一封邮件附件最大值(MB),实际上对于Exchange是包含正文和附件,控制最大发送和接收邮箱大小
        /// </summary>
        public int MaxAttachFileSize
        {
            set { m_nMaxAttachFileSize = value; }
            get { return m_nMaxAttachFileSize; }
        }

        private int m_nMaxSendMailSize = 0;
        /// <summary>
        /// 最大发送一封邮件大小(MB),包含正文和附件
        /// </summary>
        public int MaxSendMailSize
        {
            set { m_nMaxSendMailSize = value; }
            get { return m_nMaxSendMailSize; }
        }

        private int m_nMaxReceivedMailSize = 0;
        /// <summary>
        /// 最大可接收一封邮件大小(MB),包含正文和附件
        /// </summary>
        public int MaxReceivedMailSize
        {
            set { m_nMaxReceivedMailSize = value; }
            get { return m_nMaxReceivedMailSize; }
        }

        private int m_RecepientNum = 0;
        /// <summary>
        /// 最大可接收一封邮件大小(MB),包含正文和附件
        /// </summary>
        public int RecepientNum
        {
            set { m_RecepientNum = value; }
            get { return m_RecepientNum; }
        }

        #endregion

        #region mailboxFeatures 参数
        private bool m_ActiveSyncEnabled = false;
        private bool m_ImapEnabled = false;
        private bool m_MAPIEnabled = false;
        private bool m_OWACalendarEnabled = false;
        private bool m_OWAContactsEnabled = false;
        private bool m_OWAEnabled = false;
        private bool m_OWAJournalEnabled = true;
        private bool m_OWAJunkEmailEnabled = false;
        private bool m_OWANotesEnabled = false;
        private bool m_OWATasksEnabled = false;
        private bool m_PopEnabled = false;
        private bool m_OWARulesEnabled = false;
        private bool m_OWASMimeEnabled = false;
        private string m_SamAccountName = null;

        public string SamAccountName
        {
            get { return m_SamAccountName; }
            set { m_SamAccountName = value; }
        }

        public bool ActiveSyncEnabled
        {
            set { m_ActiveSyncEnabled = value; }
            get { return m_ActiveSyncEnabled; }
        }
        public bool ImapEnabled
        {
            set { m_ImapEnabled = value; }
            get { return m_ImapEnabled; }
        }
        public bool MAPIEnabled
        {
            set { m_MAPIEnabled = value; }
            get { return m_MAPIEnabled; }
        }
        public bool OWACalendarEnabled
        {
            set { m_OWACalendarEnabled = value; }
            get { return m_OWACalendarEnabled; }
        }
        public bool OWAContactsEnabled
        {
            set { m_OWAContactsEnabled = value; }
            get { return m_OWAContactsEnabled; }
        }

        public bool OWAJunkEmailEnabled
        {
            set { m_OWAJunkEmailEnabled = value; }
            get { return m_OWAJunkEmailEnabled; }
        }

        public bool OWAJournalEnabled
        {
            set { m_OWAJournalEnabled = value; }
            get { return m_OWAJournalEnabled; }
        }
        public bool OWANotesEnabled
        {
            set { m_OWANotesEnabled = value; }
            get { return m_OWANotesEnabled; }
        }
        public bool OWATasksEnabled
        {
            set { m_OWATasksEnabled = value; }
            get { return m_OWATasksEnabled; }
        }
        public bool PopEnabled
        {
            set { m_PopEnabled = value; }
            get { return m_PopEnabled; }
        }
        public bool OWARulesEnabled
        {
            set { m_OWARulesEnabled = value; }
            get { return m_OWARulesEnabled; }
        }
        public bool OWASMimeEnabled
        {
            set { m_OWASMimeEnabled = value; }
            get { return m_OWASMimeEnabled; }
        }
        public bool OWAEnabled
        {
            set { m_OWAEnabled = value; }
            get { return m_OWAEnabled; }
        }
        #endregion

        #region 其它参数(可选)
        private string m_strAlias = string.Empty;
        /// <summary>
        /// Email邮箱用户别名(必须在AD内唯一)
        /// 目前系统自动会采用用户的PreWin2k帐号来作为别名（保证唯一性）
        /// </summary>
        public string alias
        {
            set
            {
                m_strAlias = value;
            }
            get
            {
                return m_strAlias;
            }
        }

        //private SortedList m_properties = new SortedList();
        ///// <summary>
        ///// 用户邮箱的其它扩展属性
        ///// </summary>
        //public SortedList properties
        //{
        //    get
        //    {
        //        return m_properties;
        //    }
        //    set
        //    {
        //        m_properties = value;
        //    }
        //}


        private string m_strOwningOrganization = string.Empty;
        /// <summary>
        /// This parameter specifies the path of the hosted organization that contains the AL, GAL, and OAB as well-known-objects. 
        /// By default it is the LDAP parent container of the object
        /// </summary>
        public string owningOrganization
        {
            set
            {
                m_strOwningOrganization = value;
            }
            get
            {
                return m_strOwningOrganization;
            }
        }

        private string m_strDatabase = string.Empty;
        /// <summary>
        /// 邮箱存储数据库,一般系统自动管理
        /// </summary>
        public string database
        {
            set
            {
                m_strDatabase = value;
            }
            get
            {
                return m_strDatabase;
            }
        }

        #endregion
    }

    [Serializable]
    /// <summary>
    /// 用户邮箱可用的属性名称Key 列表 
    /// </summary>
    public struct UserEmailPropertyName
    {
        /// <summary>
        /// 一封邮件的最多收件人数目
        /// </summary>
        public const string RecipientLimits = "recipientLimits";

        /// <summary>
        /// This parameter specifies whether this mailbox is hidden from other address lists.
        /// </summary>
        public const string HiddenFromAddressListsEnabled = "hiddenFromAddressListsEnabled";

        /// <summary>
        /// 用户邮箱是否支持IMap4协议
        /// </summary>
        public const string ImapEnabled = "imapEnabled";

        /// <summary>
        /// 用户邮箱是否支持Pop3协议
        /// </summary>
        public const string PopEnabled = "popEnabled";
        /// <summary>
        /// 用户邮箱是否支持OWA访问
        /// </summary>
        public const string OwaEnabled = "owaEnabled";
        /// <summary>
        /// 用户邮箱是否支持MAPI协议
        /// </summary>
        public const string MapiEnabled = "mapiEnabled";
        /// <summary>
        /// Outlook 是否可以使用 non-cached 模式
        /// </summary>
        public const string MapiBlockOutlookNonCachedMode = "mapiBlockOutlookNonCachedMode";

        /// <summary>
        /// 客户端是否能够使RPC连接到Outlook
        /// </summary>
        public const string MapiBlockOutlookRpcHttp = "mapiBlockOutlookRpcHttp";

        /// <summary>
        /// 是否所有地址博能在OWA中使用
        /// </summary>
        public const string OwaAllAddressListsEnabled = "owaAllAddressListsEnabled";

        /// <summary>
        /// 是否联系人能在OWA中使用
        /// </summary>
        public const string OwaContactsEnabled = "owaContactsEnabled";

        /// <summary>
        /// 是否日历能在OWA中使用
        /// </summary>
        public const string OwaCalendarEnabled = "owaCalendarEnabled";
        /// <summary>
        /// 是否OWA能修改密码
        /// </summary>
        public const string OwaChangePasswordEnabled = "owaChangePasswordEnabled";
        /// <summary>
        /// 是否Journal能在OWA中使用
        /// </summary>
        public const string OwaJournalEnabled = "owaJournalEnabled";
        /// <summary>
        /// 是否任务能在OWA中使用
        /// </summary>
        public const string OwaTasksEnabled = "owaTasksEnabled";
        /// <summary>
        /// owa模式下,是否可以使用notes
        /// </summary>
        public const string OwaNotesEnabled = "owaNotesEnabled";
        /// <summary>
        /// OWA下的日历提醒功能是否可用
        /// </summary>
        public const string OwaRemindersAndNotificationsEnabled = "owaRemindersAndNotificationsEnabled";
        /// <summary>
        /// 是否可以使用OWA的高级模式
        /// </summary>
        public const string OwaPremiumClientEnabled = "owaPremiumClientEnabled";
        /// <summary>
        /// OWA是否可以使用拼写检查
        /// </summary>
        public const string OwaSpellCheckerEnabled = "owaSpellCheckerEnabled";
        /// <summary>
        /// OWA是否可以搜索文件夹
        /// </summary>
        public const string OwaSearchFoldersEnabled = "owaSearchFoldersEnabled";
        /// <summary>
        /// OWA是否可以签名
        /// </summary>
        public const string OwaSignaturesEnabled = "owaSignaturesEnabled";
        /// <summary>
        /// OWA是否可以主题选择
        /// </summary>
        public const string OwaThemeSelectionEnabled = "owaThemeSelectionEnabled";
        /// <summary>
        /// OWA是否有垃圾邮件
        /// </summary>
        public const string OwaJunkEmailEnabled = "owaJunkEmailEnabled";
        /// <summary>
        /// OWA是否集成UM
        /// </summary>
        public const string OwaUMIntegrationEnabled = "owaUMIntegrationEnabled";
        /// <summary>
        /// OWA是否可以使用Active Sync mobile选项
        /// </summary>
        public const string OwaActiveSyncIntegrationEnabled = "owaActiveSyncIntegrationEnabled";
        /// <summary>
        /// This parameter specifies whether Universal Naming Convention (UNC) access is permitted when users select This is a private computer on the logon page.
        /// </summary>
        public const string OwaUNCAccessOnPrivateComputersEnabled = "owaUNCAccessOnPrivateComputersEnabled";
        /// <summary>
        /// This parameter specifies whether UNC access is permitted when users select This is a public or shared computer on the logon page.Access.
        /// </summary>
        public const string OwaUNCAccessOnPublicComputersEnabled = "owaUNCAccessOnPublicComputersEnabled";
        /// <summary>
        /// This parameter specifies whether SharePoint Portal Server access is permitted when users select This is a private computer on the logon page.
        /// </summary>
        public const string OwaWSSAccessOnPrivateComputersEnabled = "owaWSSAccessOnPrivateComputersEnabled";
        /// <summary>
        /// This parameter specifies whether SharePoint Portal Server access is permitted when users select This is a public or shared computer on the logon page.
        /// </summary>
        public const string OwaWSSAccessOnPublicComputersEnabled = "owaWSSAccessOnPublicComputersEnabled";

    }

    [Serializable]
    /// <summary>
    /// 用户收发邮件的状态
    /// </summary>
    public enum UserEmailReceiveSendStatus
    {
        /// <summary>
        /// 正常收发
        /// </summary>
        Normal,
        /// <summary>
        /// 不能发
        /// </summary>
        ProhibitSend,
        /// <summary>
        /// 不能收
        /// </summary>
        ProhibitReceive,
        /// <summary>
        /// 不能收发
        /// </summary>
        PrehibitSendReceive,
        /// <summary>
        /// 未知状态
        /// </summary>
        Invailid

    }

    [Serializable]
    public enum MailStatus
    {
        /// <summary>
        ///  正常收发
        /// </summary>
        EN,
        /// <summary>
        /// 不能发
        /// </summary>
        PE,
        /// <summary>
        /// 不能收发
        /// </summary>
        DS,
        /// <summary>
        /// 删除
        /// </summary>
        DE,
        /// <summary>
        /// 未知状态
        /// </summary>
        Invailid
    }
}
