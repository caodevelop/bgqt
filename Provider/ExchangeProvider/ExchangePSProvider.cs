using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ExchangeProvider
{
    public class ExchangePSProvider
    {
        #region Global Address List Interface
        /// <summary>
        /// New-GlobalAddressList
        /// </summary>
        /// <remarks>
        /// -Name
        /// -RecipientFilter
        /// </remarks>
        public class PSCommandNewGAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-GlobalAddressList", parms);
            }
        }

        /// <summary>
        /// Remove-GlobalAddressList
        /// </summary>
        /// <remarks>
        /// -Identity
        /// </remarks>
        public class PSCommandRemoveGAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-GlobalAddressList", parms);
            }
        }

        public class PSCommandSetGAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-GlobalAddressList", parms);
            }
        }

        public class PSCommandGetGAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Get-GlobalAddressList", parms);
            }
        }
        #endregion

        #region Address List Interface
        /// <summary>
        /// New-AddressList
        /// </summary>
        /// <remarks>
        /// -Name
        /// -RecipientFilter
        /// </remarks>
        public class PSCommandNewAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-AddressList", parms);
            }
        }

        public class PSCommandSetAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-AddressList", parms);
            }
        }

        public class PSCommandGetAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Get-AddressList", parms);
            }
        }
        /// <summary>
        /// Remove-AddressList
        /// </summary>
        /// <remarks>
        /// -Identity
        /// </remarks>
        public class PSCommandRemoveAL : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-AddressList", parms);
            }
        }
        #endregion

        #region Offline AddressBook Interface
        public class PSCommandNewOAB : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-OfflineAddressBook", parms);
            }
        }

        public class PSCommandSetOAB : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-OfflineAddressBook", parms);
            }
        }

        public class PSCommandRemoveOAB : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-OfflineAddressBook", parms);
            }
        }

        public class PSCommandGetOABDir : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet()
            {
                return WSCommandBase.ExecuteCmdlet("Get-OabVirtualDirectory", new PSParameters());
            }
        }
        #endregion

        #region Accepted Domain Interface
        /// <summary>
        /// New-AcceptedDomain
        /// </summary>
        /// <remarks>
        /// -Name
        /// -DomainName
        /// </remarks>
        public class PSCommandNewAcceptedDomain : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-AcceptedDomain", parms);
            }
        }
        /// <summary>
        /// Remove-AcceptedDomain
        /// </summary>
        /// <remarks>
        /// -Identity
        /// </remarks>
        public class PSCommandRemoveAcceptedDomain : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-AcceptedDomain", parms);
            }
        }
        #endregion

        #region Contact Interface
        /// <summary>
        /// New-MailContact
        /// </summary>
        /// <remarks>
        /// -ExternalEmailAddress
        /// -Name
        /// -DisplayName
        /// -OrganizationalUnit
        /// -PrimarySmtpAddress
        /// </remarks>
        public class PSCommandNewContact : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("New-MailContact", parms);
            }
        }

        /// <summary>
        /// Remove-MailContact
        /// </summary>
        /// <remarks>
        /// -Identity
        /// </remarks>
        public class PSCommandRemoveContact : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Remove-MailContact", parms);
            }
        }

        /// <summary>
        /// Set-MailContact
        /// </summary>
        /// <remarks>
        /// -Identity
        /// -CustomAttribute1
        /// -DisplayName
        /// -EmailAddresses
        /// -EmailAddressPolicyEnabled
        /// -ExternalEmailAddress
        /// -PrimarySmtpAddress
        /// </remarks>
        public class PSCommandSetContact : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Set-MailContact", parms);
            }
        }

        public class PSCommandSetContact_1 : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-Contact", parms);
            }
        }

        #endregion

        #region DistributionGroup
        /// <summary>
        /// New-DistributionGroup
        /// </summary>
        /// <remarks>
        /// -Name
        /// -DisplayName 
        /// -Members 
        /// -OrganizationalUnit 
        /// -PrimarySmtpAddress 
        /// -Type
        /// </remarks>
        public class PSCommandNewDistribitionGroup : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("New-DistributionGroup", parms);
            }
        }

        /// <summary>
        /// Set-DistributionGroup
        /// </summary>
        /// <remarks>
        /// -Identity
        /// -CustomAttribute1
        /// -DisplayName
        /// -PrimarySmtpAddress
        /// </remarks>
        public class PSCommandSetDistributionGroup : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Set-DistributionGroup", parms);
            }
        }

        /// <summary>
        /// Remove-DistributionGroup
        /// </summary>
        /// <remarks>
        /// -Identity
        /// </remarks>
        public class PSCommandRemoveDistribitionGroup : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Remove-DistributionGroup", parms);
            }
        }

        /// <summary>
        /// Enable-DistributionGroup
        /// </summary>
        public class PSCommandEnableDistributionGroup : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Enable-DistributionGroup", parms);
            }
        }

        /// <summary>
        /// Add-DistributionGroupMember
        /// </summary>
        /// <remarks>
        /// -Identity
        /// -Member
        /// </remarks>
        public class PSCommandAddDistribitionGroupMember : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Add-DistributionGroupMember", parms);
            }
        }

        /// <summary>
        /// Remove-DistributionGroupMember
        /// </summary>
        /// <remarks>
        /// -Identity
        /// -Member
        /// </remarks>
        public class PSCommandRemoveDistribitionGroupMember : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Remove-DistributionGroupMember", parms);
            }
        }

        public class PSCommandGetDistributionGroup : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Get-DistributionGroup", parms);
            }
        }

        public class PSCommandNewMoveRequest : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("New-MoveRequest", parms);
            }
        }

        //public class PSCommandUpdateDistribitionEmail : WSCommandBase
        //{
        //    public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
        //    {
        //        return WSCommandBase.ExecuteCmdlet("Set-DistributionGroup", parms);
        //    }
        //}
        #endregion

        #region MailBoxDataBase Interface
        public class PSCommandGetMailboxDatabase : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet()
            {
                string cmd = "Get-MailboxDatabase -IncludePreExchange2013";
                return PSCommandBase.ExecuteCmdlet(cmd);
            }
        }
        #endregion

        #region MailBox/CASMailBox Interface

        /// <summary>
        /// New-Mailbox
        /// </summary>
        /// <remarks>
        /// -Name
        /// -DisplayName 
        /// -OrganizationalUnit
        /// -UserPrincipalName
        /// -SamAccountName
        /// -PrimarySmtpAddress
        /// -Password
        /// -Database 
        /// </remarks>
        public class PSCommandNewMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("New-MailBox", parms);
            }
        }
        
        public class PSCommandEnableMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Enable-MailBox", parms);
            }
        }

        /// <summary>
        /// Disable-Mailbox
        /// </summary>
        /// <remarks>
        /// Disable-Mailbox will remove the Active Directory Exchange Server attributes of the user. 
        /// The user is not deleted from Active Directory.
        /// The Mailbox in the database will be marked as removal(Disconnect).
        /// 
        /// -Identity
        /// </remarks>
        public class PSCommandDisableMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Disable-MailBox", parms);
            }
        }

        /// <summary>
        /// Remove-Mailbox
        /// </summary>
        /// <remarks>
        /// Remove-Mailbox will delete user object in the Active Directory.
        /// The Mailbox in the database will be disconnected or deleted.
        /// 
        /// -Identity
        /// -Permanent: True:Mailbox will be deleted.
        /// </remarks>
        public class PSCommandRemoveMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Remove-MailBox", parms);
            }
        }

        /// <summary>
        /// Set-Mailbox
        /// </summary>
        /// <remarks>
        /// -Identity
        /// -CustomAttribute1
        /// -DisplayName 
        /// -EmailAddresses 
        /// -EmailAddressPolicyEnabled 
        /// -IssueWarningQuota 
        /// -Languages 
        /// -MaxBlockedSenders 
        /// -MaxReceiveSize 
        /// -MaxSafeSenders 
        /// -MaxSendSize 
        /// -OfflineAddressBook
        /// -PrimarySmtpAddress 
        /// -ProhibitSendQuota 
        /// -ProhibitSendReceiveQuota 
        /// -RecipientLimits 
        /// -RulesQuota 
        /// -SamAccountName
        /// -SCLDeleteEnabled 
        /// -SCLDeleteThreshold 
        /// -SCLJunkEnabled 
        /// -SCLJunkThreshold 
        /// -SCLQuarantineEnabled 
        /// -SCLQuarantineThreshold 
        /// -SCLRejectEnabled 
        /// -SCLRejectThreshold 
        /// -UseDatabaseQuotaDefaults 

        /// </remarks>
        public class PSCommandSetMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Set-MailBox", parms);
            }
        }

        /// <summary>
        /// Set-Casmailbox
        /// </summary>
        /// <remarks>
        /// Identity 
        /// ActiveSyncEnabled 
        /// DisplayName 
        /// ECPEnabled 
        /// EmailAddresses 
        /// EmwsEnabled 
        /// ImapEnabled 
        /// MAPIBlockOutlookNonCachedMode 
        /// MAPIBlockOutlookRpcHttp 
        /// MAPIBlockOutlookVersions 
        /// MAPIEnabled
        /// OWAEnabled  
        /// PopEnabled 
        /// PrimarySmtpAddress 
        /// SamAccountName 
        /// </remarks>
        public class PSCommandSetCASMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Set-CASMailBox", parms);
            }
        }

        public class PSCommandGetMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Get-MailBox", parms);
            }
        }
        
        public class PSCommandGetCASMailBox : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Get-CASMailBox", parms);
            }
        }

        public class PSCommandSetMailboxRegionalConfiguration : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Set-MailboxRegionalConfiguration", parms);
            }
        }
        

        public class PSCommandRemoveDevice : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Get-ActiveSyncDeviceStatistics", parms);
            }
        }

        public class PSCommandGetMailboxStatistics : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return PSCommandBase.ExecuteCmdlet("Get-MailboxStatistics", parms);
            }
        }


        public class PSCommandNewTransportRule : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-TransportRule", parms);
            }
        }

        // Set-TransportRule
        public class PSCommandSetTransportRule : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-TransportRule", parms);
            }
        }

        public class PSCommandRemoveTransportRule : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-TransportRule", parms);
            }
        }
        #endregion

        #region
        public class PSCommandSetUserPhoto : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-UserPhoto", parms);
            }
        }

        public class PSCommandSaveUserPhoto : WSCommandBase
        {
            //public static ICollection<PSObject> ExecuteCmdlet(string cmdlet, ConfigInfo config)
            //{
            //    return WSCommandBase.ExecuteCmdlet(cmdlet, config);
            //}
        }

        #endregion

        public class PSCommandSetADForest : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(string strOUName, string strType)
            {
                string a = string.Format("Set-ADForest -Identity {2} -UPNSuffixes @{{{0}=\"{1}\"}}", strType, strOUName, Domain);
                //string a = string.Format("Get-Mailbox  -OrganizationalUnit \"{0}\" -ResultSize Unlimited | Search-Mailbox -SearchQuery '{1}  AND 发送时间:>{2} AND 发送时间:<{3}'  -DeleteContent -Force", strOUscope, strSearchQuery, startTime, endTime);
                //Logger.Info("RemoveSensitiveMail : " + a);
                return PSCommandBase.ExecuteCmdlet(a);
            }
        }

        #region AddressBookPolicy Interface
        public class PSCommandNewAddressBookPolicy : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-AddressBookPolicy", parms);
            }
        }

        public class PSCommandRemoveAddressBookPolicy : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-AddressBookPolicy", parms);
            }
        }
        #endregion

        #region NewEmailAddressPolicy Interface
        public class PSCommandNewEmailAddressPolicy : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-EmailAddressPolicy", parms);
            }
        }

        public class PSCommandRemoveEmailAddressPolicy : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-EmailAddressPolicy", parms);
            }
        }

        public class PSCommandUpdateEmailAddressPolicy : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Update-EmailAddressPolicy", parms);
            }
        }

        #endregion

        #region 
        public class PSCommandRemoveSensitiveMail : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(string strscope, string strSearchQuery, DateTime startTime, DateTime endTime)
            {
                string cmd = string.Empty;
                int type = 2;
                //string a = string.Format("Get-Mailbox  -OrganizationalUnit \"{0}\" -ResultSize Unlimited | Search-Mailbox -SearchQuery '{1}  AND Sent:>{2} AND Sent:<{3}'  -DeleteContent -Force", strOUscope, strSearchQuery, startTime, endTime);
                if (type == 1)
                {
                    cmd = string.Format("Get-Mailbox  -OrganizationalUnit \"{0}\" -ResultSize Unlimited | Search-Mailbox -SearchQuery '{1}  AND sent>=\"{2}\" AND sent<=\"{3}\"'  -DeleteContent -Force", strscope, strSearchQuery, startTime, endTime);
                    Log4netHelper.Info("RemoveSensitiveMail : " + cmd);
                }
                else
                {
                    cmd = string.Format("Search-Mailbox -Identity \"{0}\" -SearchQuery '{1}  AND sent>=\"{2}\" AND sent<=\"{3}\"'  -DeleteContent -Force", strscope, strSearchQuery, startTime, endTime);
                    Log4netHelper.Info("RemoveSensitiveMail : " + cmd);
                }
                return PSCommandBase.ExecuteCmdlet(cmd);
            }
        }

        public class PSCommandNewComplianceSearch : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("New-ComplianceSearch",false, parms);
            }
        }

        public class PSCommandSetComplianceSearch : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Set-ComplianceSearch", false, parms);
            }
        }
        
        public class PSCommandStartComplianceSearch : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Start-ComplianceSearch", false, parms);
            }
        }

        public class PSCommandGetComplianceSearch : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Get-ComplianceSearch", false, parms);
            }
        }

        public class PSCommandNewComplianceSearchAction : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(string cmd, bool isdc)
            {
                //return PSCommandBase.ExecuteCmdlet(cmd);
                return WSCommandBase.ExecuteCmdlet(cmd);
            }
        }

        public class PSCommandRemoveComplianceSearch : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Remove-ComplianceSearch", false, parms);
            }
        }

        public class PSCommandGetComplianceSearchAction : WSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(PSParameters parms)
            {
                return WSCommandBase.ExecuteCmdlet("Get-ComplianceSearchAction", false, parms);
            }
        }

        #endregion

        #region mailcount
        public class GetServerreceiveMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId deliver -resultsize unlimited).count", starTime, endTime);
                return PSCommandBase.ExecuteCmdlet(a);
            }
        }
        public class GetServerSendMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId receive -resultsize unlimited | {2}).count", starTime, endTime, "where {$_.Source -eq 'STOREDRIVER'}");
                //string a = string.Format("(get-transportserver *-MHC* | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId deliver -resultsize unlimited).count", starTime, endTime);
                return PSCommandBase.ExecuteCmdlet(a);
            }
        }

        public class GetSMTPSendMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId send -resultsize unlimited| {2}).count", starTime, endTime, "where {$_.Source -eq 'smtp'}");

                return PSCommandBase.ExecuteCmdlet(a);
            }
        }
        public class GetUserreceiveMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime, string mail)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId deliver -Recipients \"{2}\" -resultsize unlimited).count", starTime, endTime, mail);

                return PSCommandBase.ExecuteCmdlet(a);
            }
        }

        public class GetUserSendServerMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime, string mail)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId receive -Sender \"{2}\" -resultsize unlimited| {3}).count", starTime, endTime, mail, "where {$_.Source -eq 'STOREDRIVER'}");

                return PSCommandBase.ExecuteCmdlet(a);
            }
        }
        public class GetUserSendSMTPMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime, string mail)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId send -Sender \"{2}\" -resultsize unlimited| {3}).count ", starTime, endTime, mail, "where {$_.Source -eq 'smtp'}");

                return PSCommandBase.ExecuteCmdlet(a);
            }
        }

        public class GetUserSendExSMTPMailCount : PSCommandBase
        {
            public static ICollection<PSObject> ExecuteCmdlet(DateTime starTime, DateTime endTime, string mail)
            {
                string a = string.Format("@(Get-TransportService | Get-MessageTrackingLog -Start \"{0}\" -End \"{1}\" -EventId receive -Sender \"{2}\" -resultsize unlimited| {3}).count ", starTime, endTime, mail, "where {$_.Source -eq 'smtp'}");

                return PSCommandBase.ExecuteCmdlet(a);
            }
        }
        #endregion
    }
}
