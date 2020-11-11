using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Common;
using Entity;
using Provider.ADProvider;

namespace Provider.ExchangeProvider
{
    public class ExchangeProvider
    {
        #region GAL/AL/OAB/AccepteDomain Interface
        /// <summary>
        /// AddGAL
        /// by means of "New-GlobalAddressList"
        /// </summary>
        /// <param name="pName">GAL name, example: domain1GAL</param>
        /// <param name="pOU">OU, example: domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddGAL(
            string pName,
            string pOU,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", pName);
            //New-GlobalAddressList -Name "GAL_TAIL" -RecipientFilter {(CustomAttribute15 -eq "TAIL")}
            // paras.AddPara("RecipientFilter", string.Format("{(CustomAttribute15 -eq '{0}')}", pOU));
            // paras.AddPara("CustomAttribute15", pOU);           
            paras.AddPara("RecipientContainer", pOU);
            paras.AddPara("IncludedRecipients", "AllRecipients");


            try
            {
                ExchangePSProvider.PSCommandNewGAL.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddGAL>{0}</AddGAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        /// <summary>
        /// remove GAL
        /// by means of "Remove-GlobalAddressList"
        /// </summary>
        /// <param name="pName">GAL name, example: domain1GAL</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveGAL(
            string pName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveGAL.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveGAL>{0}</RemoveGAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetGALName(
           string pIdentity,
           string pName,
           ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pIdentity);
            paras.AddPara("Name", pName);

            try
            {
                ExchangePSProvider.PSCommandSetGAL.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetGAL>{0}</SetGAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }


        /// <summary>
        /// AddAL
        /// by means of "New-AddressList"
        /// </summary>
        /// <param name="pName">AL name, example: domain1AL</param>
        /// <param name="pOU">OU, example: domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddAL(
            string pName,
            string pDisplayName,
            string pOU,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            //string a = "{((RecipientType -eq 'UserMailbox') -or (RecipientType -eq 'MailUniversalDistributionGroup') -or (RecipientType -eq 'DynamicDistributionGroup') -and (CustomAttribute15 -eq '" + pOU + "'))}";

            paras.Add(new PSParameter("Name", pName));
            paras.AddPara("DisplayName", pDisplayName);
            //New-AddressList -Name "AL_TAIL_Users_DGs" -RecipientFilter {((RecipientType -eq 'UserMailbox') -or (RecipientType -eq "MailUniversalDistributionGroup") -or (RecipientType -eq "DynamicDistributionGroup") -and (CustomAttribute15 -eq "TAIL"))}
            //paras.Add(new PSParameter("RecipientFilter", a));
            paras.AddPara("RecipientContainer", pOU);
            paras.AddPara("IncludedRecipients", "MailboxUsers,MailContacts,MailGroups");
            //paras.AddPara("Confirm", false);
            //  paras.AddPara("CustomAttribute15", pOU);

            try
            {
                ExchangePSProvider.PSCommandNewAL.ExecuteCmdlet(paras);
                return true;
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddAL>{0}</AddAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
        }

        public static bool AddRoom(
          string pName,
           string pDisplayName,
          string pOU,
          ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.Add(new PSParameter("Name", pName));
            paras.AddPara("DisplayName", pDisplayName);
            //New-AddressList -Name AL_TAIL_Rooms -RecipientFilter {(Alias -ne $null) -and (CustomAttribute15 -eq "TAIL")-and (RecipientDisplayType -eq 'ConferenceRoomMailbox') -or (RecipientDisplayType -eq 'SyncedConferenceRoomMailbox')}
            //paras.Add(new PSParameter("RecipientFilter", string.Format("{(Alias -ne $null) -and (CustomAttribute15 -eq ‘{0}')-and (RecipientDisplayType -eq 'ConferenceRoomMailbox') -or (RecipientDisplayType -eq 'SyncedConferenceRoomMailbox')", pOU)));
            paras.AddPara("RecipientContainer", pOU);
            paras.AddPara("IncludedRecipients", "Resources");
            // paras.AddPara("Confirm", false);
            //  paras.AddPara("CustomAttribute15", pOU);

            try
            {
                ExchangePSProvider.PSCommandNewAL.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddRoom>{0}</AddRoom>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetAL(
          string pIdentity,
          string pName,
          ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.Add(new PSParameter("Identity", pIdentity));


            try
            {
                ExchangePSProvider.PSCommandSetAL.ExecuteCmdlet(paras);
                return true;
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetAL>{0}</SetAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
        }



        /// <summary>
        /// RemoveAL
        /// by means of "Remove-AddressList"
        /// </summary>
        /// <param name="pName">AL name, example: domain1AL</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveAL(
            string pName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveAL.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveAL>{0}</RemoveAL>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool AddOAB(string pName, string pGALName, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", pName);
            paras.AddPara("AddressLists", pGALName);
            try
            {
                //ICollection<PSObject> result = PSCommandGetOABDir.ExecuteCmdlet();
                //paras.Add(new PSParameter("VirtualDirectories", result));
                ExchangePSProvider.PSCommandNewOAB.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += ex.ToString();
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetOAB(
            string pIdentity,
            string pName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pIdentity);
            paras.AddPara("Name", pName);

            try
            {
                ExchangePSProvider.PSCommandNewOAB.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += ex.ToString();
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool RemoveOAB(string pName, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveOAB.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += ex.ToString();
                return false;
            }
            finally
            {
            }
            return true;
        }

        /// <summary>
        /// AddAcceptDomain
        /// by means of "New-AcceptedDomain"
        /// </summary>
        /// <param name="pName">Identity of AcceptedDomain, example: domain.com</param>
        /// <param name="pDomainName">DomainName, example: domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddAcceptedDomain(
            string pName,
            string pDomainName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", pName);
            paras.AddPara("DomainName", pDomainName);
            try
            {
                ExchangePSProvider.PSCommandNewAcceptedDomain.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddAcceptedDomain>{0}</AddAcceptedDomain>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }
        /// <summary>
        /// RemoveAcceptDomain
        /// by means of "Remove-AcceptedDomain"
        /// </summary>
        /// <param name="pName">Identity of AcceptedDomain, example: domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveAcceptedDomain(string pName, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveAcceptedDomain.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveAcceptedDomain>{0}</RemoveAcceptedDomain>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }
        #endregion

        #region AddressBookPolicy Interface
        public static bool NewAddressBookPolicy(string strPolicyName, string addressList, string globaladdressList, string oab, string roomList, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", strPolicyName);
            paras.AddPara("AddressLists", addressList);
            paras.AddPara("OfflineAddressBook", oab);
            paras.AddPara("GlobalAddressList", globaladdressList);
            paras.AddPara("RoomList", roomList);

            try
            {
                ExchangePSProvider.PSCommandNewAddressBookPolicy.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<NewAddressBookPolicy>{0}</NewAddressBookPolicy>", ex.ToString());
                return false;
            }

            return true;
        }

        public static bool RemoveAddressBookPolicy(string strPolicyName, ref string strError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", strPolicyName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveAddressBookPolicy.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<RemoveABP>{0}</RemoveABP>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }
        #endregion

        #region EmailAddressPolicy Interface
        public static bool NewEmailAddressPolicy(string strPolicyName, string pOU, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", strPolicyName);
            paras.AddPara("IncludedRecipients", "AllRecipients");
            paras.AddPara("RecipientContainer", pOU);
            paras.AddPara("EnabledEmailAddressTemplates", string.Format("SMTP:%m@{0}", strPolicyName));
            // paras.AddPara("Confirm", false);

            try
            {
                ExchangePSProvider.PSCommandNewEmailAddressPolicy.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<NewEmailAddressPolicy>{0}</NewEmailAddressPolicy>", ex.ToString());
                return false;
            }

            return true;
        }

        public static bool RemoveEmailAddressPolicy(string strPolicyName, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", strPolicyName);
            paras.AddPara("Confirm", false);

            try
            {
                ExchangePSProvider.PSCommandRemoveEmailAddressPolicy.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveEmailAddressPolicy>{0}</RemoveEmailAddressPolicy>", ex.ToString());
                return false;
            }

            return true;
        }

        public static bool UpdateEmailAddressPolicy(string strPolicyName, ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", strPolicyName);


            try
            {
                ExchangePSProvider.PSCommandUpdateEmailAddressPolicy.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<UpdateEmailAddressPolicy>{0}</UpdateEmailAddressPolicy>", ex.ToString());
                return false;
            }

            return true;
        }
        #endregion

        #region Contact Interface
        /// <summary>
        /// AddMailContact
        /// by means of "New-MailContact"
        /// </summary>
        /// <param name="pName">CN, example: user1@sina.com</param>
        /// <param name="pExternalEmailAddress">ExternaleEmailAddress, example: user1@sina.com</param>
        /// <param name="pDisplayName">DisplayName, example: user1</param>
        /// <param name="pOrganizationalUnit">OU, example: domain.com</param>
        /// <param name="pPrimarySmtpAddress">PrimarySmtpAddress, example: user1@sina.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddMailContact(
            string pName,
            string pExternalEmailAddress,
            string pDisplayName,
            string pOrganizationalUnit,
            string pPrimarySmtpAddress,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", pName);
            paras.AddPara("ExternalEmailAddress", pExternalEmailAddress);
            paras.AddPara("DisplayName", pDisplayName);
            paras.AddPara("OrganizationalUnit", pOrganizationalUnit);
            //paras.AddPara("PrimarySmtpAddress", pPrimarySmtpAddress);

            try
            {
                ExchangePSProvider.PSCommandNewContact.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddMailContact>{0}</AddMailContact>", ex.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// SetMailContact
        /// by means of "Set-MailContact" 
        /// </summary>
        /// <param name="pName">Identity of Contact, example: user1@sina.com</param>
        /// <param name="pOU">OU, example: domain.com</param>
        /// <param name="DisplayName">DisplayName, example: mike</param>
        /// <param name="pEmailAddresses">EmailAddresses, example: user1@sina.com</param>
        /// <param name="EmailAddressPolicyEnabled">EmailAddressPolicyEnabled, example: false</param>
        /// <param name="pExternalEmailAddress">ExternaleEmailAddress, example: user1@sina.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool SetMailContact(
            string pName,
            string pOU,
            string pDisplayName,
            List<string> pEmailAddresses,
            Nullable<bool> pEmailAddressPolicyEnabled,
            string pExternalEmailAddress,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            //if (!string.IsNullOrEmpty(pOU))
            //{
            //    paras.AddPara("CustomAttribute1", pOU);
            //}
            if (!string.IsNullOrEmpty(pDisplayName))
            {
                paras.AddPara("DisplayName", pDisplayName);
            }
            //if (pEmailAddressPolicyEnabled.HasValue)
            //{
            //    paras.AddPara("EmailAddressPolicyEnabled", pEmailAddressPolicyEnabled.Value);
            //}
            //if (pEmailAddresses.Count > 0)
            //{
            //    paras.AddPara("EmailAddresses", pEmailAddresses);
            //}
            if (!string.IsNullOrEmpty(pExternalEmailAddress))
            {
                paras.AddPara("ExternalEmailAddress", pExternalEmailAddress);
            }


            try
            {
                ExchangePSProvider.PSCommandSetContact.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetMailContact>{0}</SetMailContact>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// RemoveMailContact
        /// by means of "Remove-MailContact"
        /// </summary>
        /// <param name="pName">Identity of Contact, example: user1@sina.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveMailContact(
            string pName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);
            paras.AddPara("Confirm", false);
            try
            {
                ExchangePSProvider.PSCommandRemoveContact.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveMailContact>{0}</RemoveMailContact>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool EnableMailContact(
            string pName,
            string pExternalEmailAddress,
            string pDisplayName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("ExternalEmailAddress", pExternalEmailAddress);
            paras.AddPara("DisplayName", pDisplayName);
            
            try
            {
                ExchangePSProvider.PSCommandEnableContact.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<EnableMailContact>{0}</EnableMailContact>", ex.ToString());
                return false;
            }
            return true;
        }
        #endregion

        #region DistributionGroup Interface

        /// <summary>
        /// AddDistributionGroup
        /// by means of "New-DistributionGroup"
        /// </summary>
        /// <param name="pName">Name, example: group@domain.com</param>
        /// <param name="pDisplayName">DisplayName, example:group</param>
        /// <param name="pMembers">Members of DistributionGroup, example: user1@domain.com</param>
        /// <param name="pOrganizationalUnit">OU, example: domain.com</param>
        /// <param name="pPrimarySmtpAddress">PrimarySmtpAddress, example: group@domain.com</param>
        /// <param name="pType">Type of DistributionGroup, example: "Distribution"</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddDistributionGroup(
            string pName,
            string pDisplayName,
            string pOrganizationalUnit,
            string pType,
            string pSamAccount,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Name", pName);
            paras.AddPara("DisplayName", pDisplayName);
            //paras.AddPara("OrganizationalUnit", System.Configuration.ConfigurationManager.AppSettings["OrganizationalUnitPath"] + pOrganizationalUnit);

            //"Distribution"
            paras.AddPara("Type", pType);
            //paras.AddPara("SamAccountName", pSamAccount);
            //paras.AddPara("SamAccountName", (System.Guid.NewGuid().ToString("n") + "@zhiteng.net"));
            try
            {
                ExchangePSProvider.PSCommandNewDistribitionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddDistributionGroup>{0}</AddDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetDistributionGroup(string pName,
            out string strError)
        {
            strError = string.Empty;
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);

            try
            {
                ExchangePSProvider.PSCommandGetDistributionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetDistributionGroup>{0}</GetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetDistributionGroup(
            string pName,
            out List<string> members,
            out string pStrError)
        {
            pStrError = string.Empty;
            members = new List<string>();
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetDistributionGroup.ExecuteCmdlet(paras);
                foreach (PSObject o in result)
                {
                    ArrayList arr = (ArrayList)((System.Management.Automation.PSObject)o.Members["ManagedBy"].Value).BaseObject;
                    if (arr.Count > 0)
                    {
                        for (int i = 0; i < arr.Count; i++)
                        {
                            members.Add(arr[i].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<GetDistributionGroup>{0}</GetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetDistributionGroupModeratedBy(
            string pName,
            out List<string> members,
            out string pStrError)
        {
            pStrError = string.Empty;
            members = new List<string>();
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetDistributionGroup.ExecuteCmdlet(paras);
                foreach (PSObject o in result)
                {
                    ArrayList arr = (ArrayList)((System.Management.Automation.PSObject)o.Members["ModeratedBy"].Value).BaseObject;
                    if (arr.Count > 0)
                    {
                        for (int i = 0; i < arr.Count; i++)
                        {
                            members.Add(arr[i].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<GetDistributionGroup>{0}</GetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// SetDistributionGroup
        /// by means of "Set-DistributionGroup"
        /// </summary>
        /// <param name="pName">Identity of DistributionGroup, example: group1@domain.com</param>
        /// <param name="pOU">OU, example: domain.com</param>
        /// <param name="pDisplayName">DisplayName, example: group1</param>
        /// <param name="pPrimarySmtpAddress">PrimarySmtpAddress, example: group1@domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool SetDistributionGroup(
            string pName,
            string pDisplayName,
            string pPrimarySmtpAddress,
            out string pStrError)
        {
            pStrError = string.Empty;

            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);
            paras.AddPara("Alias", pPrimarySmtpAddress.Substring(0, pPrimarySmtpAddress.IndexOf('@')));
            paras.AddPara("EmailAddresses", "SMTP:" + pPrimarySmtpAddress);

            try
            {
                ExchangePSProvider.PSCommandSetDistributionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetDistributionGroup>{0}</SetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置通讯组审批人
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="pOU"></param>
        /// <param name="pDisplayName"></param>
        /// <param name="pPrimarySmtpAddress"></param>
        /// <param name="pStrError"></param>
        /// <returns></returns>
        public static bool SetDistributionGroup(
            string pName,
            bool ModerationEnabled,
            List<Guid> Users,
            ref string pStrError)
        {
            string cmd = string.Empty;
            if (ModerationEnabled && Users.Count > 0)
            {
                string addparam = string.Empty;
                for (int i = 0; i < Users.Count; i++)
                {
                    if (i + 1 == Users.Count)
                    {
                        addparam += "\"" + Users[i].ToString() + "\"";
                    }
                    else
                    {
                        addparam += "\"" + Users[i].ToString() + "\",";
                    }
                }

                cmd = "Set-DistributionGroup -Identity " + pName + " -ModerationEnabled  $true -ModeratedBy  @{ Add =" + addparam + "}";
            }
            else
            {
                cmd = string.Format("Set-DistributionGroup -Identity {0} -ModerationEnabled  $false -ModeratedBy $null", pName);
            }
            try
            {
                ExchangePSProvider.PSCommandSetDistributionGroup.ExecuteCmdlet(cmd);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetDistributionGroup>{0}</SetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetDistributionGroupManagedBy(
            string pName,
            List<Guid> deleteUsers,
             List<Guid> addUsers,
            ref string pStrError)
        {
            string cmd = string.Empty;
            string addparam = string.Empty;
            string deleteparam = string.Empty;
            try
            {
                if (addUsers.Count > 0)
                {
                    for (int i = 0; i < addUsers.Count; i++)
                    {
                        if (i + 1 == addUsers.Count)
                        {
                            addparam += "\"" + addUsers[i].ToString() + "\"";
                        }
                        else
                        {
                            addparam += "\"" + addUsers[i].ToString() + "\",";
                        }
                    }
                }
                if (deleteUsers.Count > 0)
                {
                    for (int i = 0; i < deleteUsers.Count; i++)
                    {
                        if (i + 1 == deleteUsers.Count)
                        {
                            deleteparam += "\"" + deleteUsers[i].ToString() + "\"";
                        }
                        else
                        {
                            deleteparam += "\"" + deleteUsers[i].ToString() + "\",";
                        }
                    }
                }

                string cmdparam = string.Empty;
                if (!string.IsNullOrEmpty(addparam))
                {
                    cmdparam = "Add =" + addparam;
                }
                if (!string.IsNullOrEmpty(deleteparam))
                {
                    if (!string.IsNullOrEmpty(cmdparam))
                    {
                        cmdparam += ";Remove=" + deleteparam;
                    }
                    else
                    {
                        cmdparam = "Remove =" + deleteparam;
                    }
                }

                if (!string.IsNullOrEmpty(cmdparam))
                {
                    cmd = "Set-DistributionGroup -Identity " + pName + "  -ManagedBy  @{" + cmdparam + "} -BypassSecurityGroupManagerCheck -ForceUpgrade";
                    ExchangePSProvider.PSCommandSetDistributionGroup.ExecuteCmdlet(cmd);
                }
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetDistributionGroup>{0}</SetDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }
        
        public static bool SetDisGroup(string pName, string customAttributeName, string customAttributeValue, ref string strError)
        {
            if (!string.IsNullOrEmpty(customAttributeName) && !string.IsNullOrEmpty(customAttributeValue))
            {
                PSParameters paras = new PSParameters();

                paras.AddPara("Identity", pName);
                paras.AddPara(customAttributeName, customAttributeValue);

                try
                {
                    ExchangePSProvider.PSCommandSetDistributionGroup.ExecuteCmdlet(paras);
                }
                catch (Exception ex)
                {
                    strError += string.Format("<SetDisGroup>{0}</SetDisGroup>", ex.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// RemoveDistributionGroup
        /// by means of "Remove-DistributionGroup"
        /// </summary>
        /// <param name="pName">Identiy of DistributionGroup, example: group@domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveDistributionGroup(
            string pName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);

            try
            {
                ExchangePSProvider.PSCommandRemoveDistribitionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveDistributionGroup>{0}</RemoveDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// AddDistributionGroupMember
        /// by means of "Add-DistributionGroupMember"
        /// </summary>
        /// <param name="pName">Identity of DistributionGroup, example: group1@domain.com</param>
        /// <param name="pMember">Member, example: user1@domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddDistributionGroupMember(
            string pName,
            string pMember,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("Member", pMember);

            try
            {
                ExchangePSProvider.PSCommandAddDistribitionGroupMember.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<AddDistributionGroupMember>{0}</AddDistributionGroupMember>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// RemoveDistributionGroupMember
        /// by means of "Remove-DistributionGroupMember"
        /// </summary>
        /// <param name="pName">Identity of DistributionGroup, example: group1@domain.com</param>
        /// <param name="pMember">Member, example: user1@domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveDistributionGroupMember(
            string pName,
            string pMember,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pName);
            paras.AddPara("Member", pMember);

            try
            {
                ExchangePSProvider.PSCommandRemoveDistribitionGroupMember.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveDistributionGroupMember>{0}</RemoveDistributionGroupMember>", ex.ToString());
                return false;
            }
            return true;
        }

        #endregion

        #region MailBox Interface

        /// <summary>
        /// AddMailBox
        /// By means of "New-Mailbox"
        /// </summary>
        /// <param name="pAccountCN">CN, example: user1@domain.com or user1 or guid</param>
        /// <param name="pDisplayName">displayname, example: john, mike</param>
        /// <param name="pOUName">OU, example: domain.com</param>
        /// <param name="pUPN">UPN, example: user1@domain.com</param>
        /// <param name="pSamAccount">SamAccount, length:20, example: user1_domain.com</param>
        /// <param name="pPrimaryAddress">smtp address, example: user1_domain.com</param>
        /// <param name="pPassword">password, example: 123456789</param>
        /// <param name="pDataBase">mailbox database name, example: Mailbox Database 0128904996, MailboxServer\Mailbox Database 0128904996</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool AddMailbox(
           ref UserInfo user,
            ref string pStrError)
        {
            Log4netHelper.Info("AddMailbox " +  user.UserAccount + " " + user.AliasName);
            System.Security.SecureString passwordStr = new System.Security.SecureString();

            foreach (char c in user.Password.ToCharArray())
            {
                passwordStr.AppendChar(c);
            }

            PSParameters paras = new PSParameters();

            paras.AddPara("LastName", user.LastName);
            paras.AddPara("FirstName", user.FirstName);
            paras.AddPara("DisplayName", user.DisplayName);
            paras.AddPara("OrganizationalUnit", user.ParentDistinguishedName);
            paras.AddPara("UserPrincipalName", user.UserAccount);
            if (!string.IsNullOrEmpty(user.SAMAccountName))
            {
                paras.AddPara("SamAccountName", user.SAMAccountName);
            }
            paras.AddPara("Name", user.DisplayName);
            paras.AddPara("Password", passwordStr);
            paras.AddPara("Database", user.UserExchange.Database);
            paras.AddPara("ResetPasswordOnNextLogon", user.NextLoginChangePassword);
            paras.AddPara("Alias", user.AliasName);
           
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandNewMailBox.ExecuteCmdlet(paras);
                foreach (PSObject o in result)
                {
                    if (o.Members["Guid"].Value != null)
                    {
                        user.UserID = Guid.Parse(Convert.ToString(o.Members["Guid"].Value));
                    }
                }
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<NewMailbox>{0}</NewMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// SetMailbox
        /// By means of "Set-Mailbox"
        /// </summary>
        /// <param name="pUPN">UPN, example: user1@domain.com</param>
        /// <param name="pOU">OU, example: domain.com</param>
        /// <param name="pDisplayName">displayname, example: john, mike</param>
        /// <param name="pEmailAddresses">emailaddresses, example: user1@domain.com</param>
        /// <param name="pIssueWarningQuota">warning quota, KB, -1 for unlimited, example: 1024</param>
        /// <param name="pLanguages">mailbox languages, -1 for unlimited, example: zh-cn</param>
        /// <param name="pMaxBlockedSenders">blocksender limits, -1 for unlimited, example: 100</param>
        /// <param name="pMaxReceiveSize">max receivemailsize, KB, -1 for unlimited, example: 1024</param>
        /// <param name="pMaxSafeSenders">safesender limits, -1 for unlimited, example: 100</param>
        /// <param name="pMaxSendSize">max sendmailsize, KB, -1 for unlimited, example: 1024</param>
        /// <param name="pOfflineAddressBook">OAB</param>
        /// <param name="pPrimarySmtpAddress">PrimarySmtpAddress, example: user1@domain.com</param>
        /// <param name="pProhibitSendQuota">ProhibitSend Quota, KB, example: 102400</param>
        /// <param name="pProhibitSendReceiveQuota">ProhibitSendReceive Quota, KB, example: 102400</param>
        /// <param name="pRecipientLimits">recipient limits, -1 for unlimited, example: 100</param>
        /// <param name="pRulesQuota">RulesQuota, KB, example: 64</param>
        /// <param name="pSamAccountName">SamAccount, length:20, example: user1_domain.com</param>
        /// <param name="pSCLDeleteEnabled">SCLDeleteEnabled, example: true</param>
        /// <param name="pSCLDeleteThreshold">SCLDeleteThreshold, between 0 and 9, example: 6</param>
        /// <param name="pSCLJunkEnabled">SCLJunkEnabled, example: true</param>
        /// <param name="pSCLJunkThreshold">SCLDeleteThreshold, between 0 and 9, example: 6</param>
        /// <param name="pSCLQuarantineEnabled">SCLQuarantineEnabled, example: true</param>
        /// <param name="pSCLQuarantineThreshold">SCLDeleteThreshold, between 0 and 9, example: 6</param>
        /// <param name="pSCLRejectEnabled">SCLRejectEnabled, example: true</param>
        /// <param name="pSCLRejectThreshold">SCLDeleteThreshold, between 0 and 9, example: 6</param>
        /// <param name="pUseDatabaseQuotaDefaults">UseDatabaseQuotaDefaults, example: true</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool SetMailbox(
            string pUPN,
            string pOU,
            string pDisplayName,
            List<string> pEmailAddresses,
            Nullable<long> pIssueWarningQuota,
            string pLanguages,
            Nullable<int> pMaxBlockedSenders,
            Nullable<long> pMaxReceiveSize,
            Nullable<int> pMaxSafeSenders,
            Nullable<long> pMaxSendSize,
            string pOfflineAddressBook,
            string pPrimarySmtpAddress,
            Nullable<long> pProhibitSendQuota,
            Nullable<long> pProhibitSendReceiveQuota,
            Nullable<int> pRecipientLimits,
            Nullable<int> pRulesQuota,
            string pSamAccountName,
            Nullable<bool> pSCLDeleteEnabled,
            Nullable<int> pSCLDeleteThreshold,
            Nullable<bool> pSCLJunkEnabled,
            Nullable<int> pSCLJunkThreshold,
            Nullable<bool> pSCLQuarantineEnabled,
            Nullable<int> pSCLQuarantineThreshold,
            Nullable<bool> pSCLRejectEnabled,
            Nullable<int> pSCLRejectThreshold,
            Nullable<bool> pUseDatabaseQuotaDefaults,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", pUPN);
            if (!string.IsNullOrEmpty(pOU))
            {
                paras.AddPara("CustomAttribute1", pOU);
            }
            if (!string.IsNullOrEmpty(pDisplayName))
            {
                paras.AddPara("DisplayName", pDisplayName);
            }

            List<string> emailAddresses = new List<string>();
            if (pEmailAddresses.Count > 0)
            {
                for (int i = 0; i < pEmailAddresses.Count; i++)
                {
                    if (pEmailAddresses[i] == pPrimarySmtpAddress)
                    {
                        emailAddresses.Add("SMTP:" + pEmailAddresses[i]);
                    }
                    else
                    {
                        emailAddresses.Add("smtp:" + pEmailAddresses[i]);
                    }
                }
                if (!emailAddresses.Contains("SMTP:" + pPrimarySmtpAddress))
                {
                    emailAddresses.Add("SMTP:" + pPrimarySmtpAddress);
                }
                paras.AddPara("EmailAddresses", emailAddresses);
                paras.AddPara("EmailAddressPolicyEnabled", false);
            }
            else
            {
                if (!string.IsNullOrEmpty(pPrimarySmtpAddress))
                {
                    paras.AddPara("PrimarySmtpAddress", pPrimarySmtpAddress);
                }
            }
            if (pIssueWarningQuota.HasValue)
            {
                if (pIssueWarningQuota.Value == -1)
                {
                    paras.AddPara("IssueWarningQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("IssueWarningQuota", pIssueWarningQuota.Value + "KB");
                }
            }
            if (!string.IsNullOrEmpty(pLanguages))
            {
                paras.AddPara("Languages", pLanguages);
            }
            if (pMaxBlockedSenders.HasValue)
            {
                paras.AddPara("MaxBlockedSenders", pMaxBlockedSenders.Value);
            }
            if (pMaxReceiveSize.HasValue)
            {
                if (pMaxReceiveSize.Value == -1)
                {
                    paras.AddPara("MaxReceiveSize", "unlimited");
                }
                else
                {
                    paras.AddPara("MaxReceiveSize", pMaxReceiveSize.Value + "KB");
                }
            }
            if (pMaxSafeSenders.HasValue)
            {
                paras.AddPara("MaxSafeSenders", pMaxSafeSenders.Value);
            }
            if (pMaxSendSize.HasValue)
            {
                if (pMaxSendSize.Value == -1)
                {
                    paras.AddPara("MaxSendSize", "unlimited");
                }
                else
                {
                    paras.AddPara("MaxSendSize", pMaxSendSize.Value + "KB");
                }
            }
            if (!string.IsNullOrEmpty(pOfflineAddressBook))
            {
                paras.AddPara("OfflineAddressBook", pOfflineAddressBook);
            }
            if (pProhibitSendQuota.HasValue)
            {
                if (pProhibitSendQuota.Value == -1)
                {
                    paras.AddPara("ProhibitSendQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("ProhibitSendQuota", pProhibitSendQuota.Value + "KB");
                }
            }
            if (pProhibitSendReceiveQuota.HasValue)
            {
                if (pProhibitSendReceiveQuota.Value == -1)
                {
                    paras.AddPara("ProhibitSendReceiveQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("ProhibitSendReceiveQuota", pProhibitSendReceiveQuota.Value + "KB");
                }
            }
            if (pRecipientLimits.HasValue)
            {
                if (pRecipientLimits.Value == -1)
                {
                    paras.AddPara("RecipientLimits", "unlimited");
                }
                else
                {
                    paras.AddPara("RecipientLimits", pRecipientLimits.Value);
                }
            }
            if (pRulesQuota.HasValue)
            {
                paras.AddPara("RulesQuota", pRulesQuota.Value + "KB");
            }
            if (!string.IsNullOrEmpty(pSamAccountName))
            {
                paras.AddPara("SamAccountName", pSamAccountName);
            }
            if (pSCLDeleteEnabled.HasValue)
            {
                paras.AddPara("SCLDeleteEnabled", pSCLDeleteEnabled.Value);
                if (pSCLDeleteThreshold.HasValue)
                {
                    paras.AddPara("SCLDeleteThreshold", pSCLDeleteThreshold.Value);
                }
            }
            if (pSCLJunkEnabled.HasValue)
            {
                paras.AddPara("SCLJunkEnabled", pSCLJunkEnabled.Value);
                if (pSCLJunkThreshold.HasValue)
                {
                    paras.AddPara("SCLJunkThreshold", pSCLJunkThreshold.Value);
                }
            }
            if (pSCLQuarantineEnabled.HasValue)
            {
                paras.AddPara("SCLQuarantineEnabled", pSCLQuarantineEnabled.Value);
                if (pSCLQuarantineThreshold.HasValue)
                {
                    paras.AddPara("SCLQuarantineThreshold", pSCLQuarantineThreshold.Value);
                }
            }
            if (pSCLRejectEnabled.HasValue)
            {
                paras.AddPara("SCLRejectEnabled", pSCLRejectEnabled.Value);
                if (pSCLRejectThreshold.HasValue)
                {
                    paras.AddPara("SCLRejectThreshold", pSCLRejectThreshold.Value);
                }
            }
            if (pUseDatabaseQuotaDefaults.HasValue)
            {
                paras.AddPara("UseDatabaseQuotaDefaults", pUseDatabaseQuotaDefaults.Value);
            }

            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetMailbox>{0}</SetMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetMailbox(
         string pDatabase,
          string pUPN,
          string pOU,
          string pDisplayName,
          List<string> pEmailAddresses,
          Nullable<long> pIssueWarningQuota,
          string pLanguages,
          Nullable<int> pMaxBlockedSenders,
          Nullable<long> pMaxReceiveSize,
          Nullable<int> pMaxSafeSenders,
          Nullable<long> pMaxSendSize,
          string pOfflineAddressBook,
          string pPrimarySmtpAddress,
          Nullable<long> pProhibitSendQuota,
          Nullable<long> pProhibitSendReceiveQuota,
          Nullable<int> pRecipientLimits,
          Nullable<int> pRulesQuota,
          string pSamAccountName,
          Nullable<bool> pSCLDeleteEnabled,
          Nullable<int> pSCLDeleteThreshold,
          Nullable<bool> pSCLJunkEnabled,
          Nullable<int> pSCLJunkThreshold,
          Nullable<bool> pSCLQuarantineEnabled,
          Nullable<int> pSCLQuarantineThreshold,
          Nullable<bool> pSCLRejectEnabled,
          Nullable<int> pSCLRejectThreshold,
          Nullable<bool> pUseDatabaseQuotaDefaults,
          ref string strError)
        {
            PSParameters paras = new PSParameters();
            PSParameters parasDatabase = new PSParameters();
            PSParameters parasRemoveDatabase = new PSParameters();

            paras.AddPara("Identity", pUPN);
            parasDatabase.AddPara("Identity", pUPN);
            parasRemoveDatabase.AddPara("Identity", pUPN);
            //if (!string.IsNullOrEmpty(pOU))
            //{
            //    paras.AddPara("CustomAttribute1", pOU);
            //}
            if (!string.IsNullOrEmpty(pDisplayName))
            {
                paras.AddPara("DisplayName", pDisplayName);
            }

            if (!string.IsNullOrEmpty(pDatabase))
            {
                parasDatabase.AddPara("TargetDatabase", pDatabase);
            }

            List<string> emailAddresses = new List<string>();
            emailAddresses.AddRange(pEmailAddresses);
            if (emailAddresses.Count > 0)
            {
                if (!string.IsNullOrEmpty(pPrimarySmtpAddress))
                {
                    if (emailAddresses.Contains(pPrimarySmtpAddress))
                    {
                        emailAddresses.Remove(pPrimarySmtpAddress);
                    }
                    emailAddresses.Add("SMTP:" + pPrimarySmtpAddress);
                }
                paras.AddPara("EmailAddresses", emailAddresses);
                paras.AddPara("EmailAddressPolicyEnabled", false);
            }
            else
            {
                if (!string.IsNullOrEmpty(pPrimarySmtpAddress))
                {
                    paras.AddPara("PrimarySmtpAddress", pPrimarySmtpAddress);
                }
            }
            if (pIssueWarningQuota.HasValue)
            {
                if (pIssueWarningQuota.Value == -1)
                {
                    paras.AddPara("IssueWarningQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("IssueWarningQuota", pIssueWarningQuota.Value + "GB");
                }
            }
            if (!string.IsNullOrEmpty(pLanguages))
            {
                paras.AddPara("Languages", pLanguages);
            }
            if (pMaxBlockedSenders.HasValue)
            {
                paras.AddPara("MaxBlockedSenders", pMaxBlockedSenders.Value);
            }
            if (pMaxReceiveSize.HasValue)
            {
                if (pMaxReceiveSize.Value == -1)
                {
                    paras.AddPara("MaxReceiveSize", "unlimited");
                }
                else
                {
                    paras.AddPara("MaxReceiveSize", pMaxReceiveSize.Value + "GB");
                }
            }
            if (pMaxSafeSenders.HasValue)
            {
                paras.AddPara("MaxSafeSenders", pMaxSafeSenders.Value);
            }
            if (pMaxSendSize.HasValue)
            {
                if (pMaxSendSize.Value == -1)
                {
                    paras.AddPara("MaxSendSize", "unlimited");
                }
                else
                {
                    paras.AddPara("MaxSendSize", pMaxSendSize.Value + "GB");
                }
            }
            if (!string.IsNullOrEmpty(pOfflineAddressBook))
            {
                paras.AddPara("OfflineAddressBook", pOfflineAddressBook);
            }
            if (pProhibitSendQuota.HasValue)
            {
                if (pProhibitSendQuota.Value == -1)
                {
                    paras.AddPara("ProhibitSendQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("ProhibitSendQuota", pProhibitSendQuota.Value + "GB");
                }
            }
            if (pProhibitSendReceiveQuota.HasValue)
            {
                if (pProhibitSendReceiveQuota.Value == -1)
                {
                    paras.AddPara("ProhibitSendReceiveQuota", "unlimited");
                }
                else
                {
                    paras.AddPara("ProhibitSendReceiveQuota", pProhibitSendReceiveQuota.Value + "GB");
                }
            }
            if (pRecipientLimits.HasValue)
            {
                if (pRecipientLimits.Value == -1)
                {
                    paras.AddPara("RecipientLimits", "unlimited");
                }
                else
                {
                    paras.AddPara("RecipientLimits", pRecipientLimits.Value);
                }
            }
            if (pRulesQuota.HasValue)
            {
                paras.AddPara("RulesQuota", pRulesQuota.Value + "GB");
            }
            if (!string.IsNullOrEmpty(pSamAccountName))
            {
                paras.AddPara("SamAccountName", pSamAccountName);
            }
            if (pSCLDeleteEnabled.HasValue)
            {
                paras.AddPara("SCLDeleteEnabled", pSCLDeleteEnabled.Value);
                if (pSCLDeleteThreshold.HasValue)
                {
                    paras.AddPara("SCLDeleteThreshold", pSCLDeleteThreshold.Value);
                }
            }
            if (pSCLJunkEnabled.HasValue)
            {
                paras.AddPara("SCLJunkEnabled", pSCLJunkEnabled.Value);
                if (pSCLJunkThreshold.HasValue)
                {
                    paras.AddPara("SCLJunkThreshold", pSCLJunkThreshold.Value);
                }
            }
            if (pSCLQuarantineEnabled.HasValue)
            {
                paras.AddPara("SCLQuarantineEnabled", pSCLQuarantineEnabled.Value);
                if (pSCLQuarantineThreshold.HasValue)
                {
                    paras.AddPara("SCLQuarantineThreshold", pSCLQuarantineThreshold.Value);
                }
            }
            if (pSCLRejectEnabled.HasValue)
            {
                paras.AddPara("SCLRejectEnabled", pSCLRejectEnabled.Value);
                if (pSCLRejectThreshold.HasValue)
                {
                    paras.AddPara("SCLRejectThreshold", pSCLRejectThreshold.Value);
                }
            }
            if (pUseDatabaseQuotaDefaults.HasValue)
            {
                paras.AddPara("UseDatabaseQuotaDefaults", pUseDatabaseQuotaDefaults.Value);
            }

            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(paras);
                try
                {
                    if (!string.IsNullOrEmpty(pDatabase))
                    {
                        ExchangePSProvider.PSCommandNewMoveRequest.ExecuteCmdlet(parasDatabase);
                    }
                }
                catch (Exception ex)
                {
                    strError = ex.ToString();
                }
                //PSCommandNRemoveMoveRequest.ExecuteCmdlet(parasRemoveDatabase);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetMailbox>{0}</SetMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetCASMailbox(
             string pUPN,
             List<string> pEmailAddresses,
             string pPrimarySmtpAddress,
             ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pUPN);
            List<string> emailAddresses = new List<string>();
            if (pEmailAddresses.Count > 0)
            {
                for (int i = 0; i < pEmailAddresses.Count; i++)
                {
                    if (pEmailAddresses[i] == pPrimarySmtpAddress)
                    {
                        emailAddresses.Add("SMTP:" + pEmailAddresses[i]);
                    }
                    else
                    {
                        emailAddresses.Add("smtp:" + pEmailAddresses[i]);
                    }
                }
                if (!emailAddresses.Contains("SMTP:" + pPrimarySmtpAddress))
                {
                    emailAddresses.Add("SMTP:" + pPrimarySmtpAddress);
                }
                paras.AddPara("EmailAddresses", emailAddresses);
                //paras.AddPara("EmailAddressPolicyEnabled", false);
            }
            else
            {
                if (!string.IsNullOrEmpty(pPrimarySmtpAddress))
                {
                    paras.AddPara("PrimarySmtpAddress", pPrimarySmtpAddress);
                }
            }
            try
            {
                ExchangePSProvider.PSCommandSetCASMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetCASMailbox>{0}</SetCASMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// SetCASMailbox
        /// By means of "Set-CASMailbox"
        /// </summary>
        /// <param name="pUPN">UPN, example: user1@domain.com</param>
        /// <param name="pActiveSyncEnabled">ActiveSync Enabled, example: true</param>
        /// <param name="pDisplayName">DisplayName, example: mike</param>
        /// <param name="pECPEnabled">ECP Enabled, example: true</param>
        /// <param name="pEmailAddresses">emailaddresses, example: user1@domain.com</param>
        /// <param name="pEmwsEnabled">Exchange webservice Enabled, example: true</param>
        /// <param name="pImapEnabled">Imap Enabled, example: true</param>
        /// <param name="pMAPIEnabled">Mapi Enabled, example: true</param>
        /// <param name="pOWAEnabled">OWA Enabled, example: true</param>
        /// <param name="pPopEnabled">Pop Enabled, example: true</param>
        /// <param name="pPrimarySmtpAddress">PrimarySmtpAddress, example: user1@domain.com</param>
        /// <param name="pSamAccountName">SamAccount, length:20, example: user1_domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool SetCASMailbox(
            string pUPN,
            Nullable<bool> pActiveSyncEnabled,
            string pDisplayName,
            List<string> pEmailAddresses,
            Nullable<bool> pImapEnabled,
            Nullable<bool> pMAPIEnabled,
            Nullable<bool> pOWAEnabled,
            Nullable<bool> pPopEnabled,
            Nullable<bool> pOWACalendarEnabled,
            Nullable<bool> pOWAContactsEnabled,
            Nullable<bool> pOWAJournalEnabled,
            Nullable<bool> pOWAJunkEmailEnabled,
            Nullable<bool> pOWANotesEnabled,
            Nullable<bool> pOWATasksEnabled,
            Nullable<bool> pOWARulesEnabled,
            Nullable<bool> pOWASMimeEnabled,
            Nullable<bool> pOWARemindersAndNotificationsEnabled,
            Nullable<bool> pOWAPremiumClientEnabled,
            Nullable<bool> pOWASpellCheckerEnabled,
            Nullable<bool> pOWASearchFoldersEnabled,
            Nullable<bool> pOWASignaturesEnabled,
            Nullable<bool> pOWAActiveSyncIntegrationEnabled,
            string pPrimarySmtpAddress,
            string pSamAccountName,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pUPN);
            if (pActiveSyncEnabled.HasValue)
            {
                paras.AddPara("ActiveSyncEnabled", pActiveSyncEnabled.Value);
            }
            if (!string.IsNullOrEmpty(pDisplayName))
            {
                paras.AddPara("DisplayName", pDisplayName);
            }

            #region SetCASMailbox(MailAddressList)
            //List<string> emailAddresses = new List<string>();
            //if (pEmailAddresses.Count > 0)
            //{
            //    for (int i = 0; i < pEmailAddresses.Count; i++)
            //    {
            //        if (pEmailAddresses[i] == pPrimarySmtpAddress)
            //        {
            //            emailAddresses.Add("SMTP:" + pEmailAddresses[i]);
            //        }
            //        else
            //        {
            //            emailAddresses.Add("smtp:" + pEmailAddresses[i]);
            //        }
            //    }
            //    if (!emailAddresses.Contains("SMTP:" + pPrimarySmtpAddress))
            //    {
            //        emailAddresses.Add("SMTP:" + pPrimarySmtpAddress);
            //    }
            //    paras.AddPara("EmailAddresses", emailAddresses);
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(pPrimarySmtpAddress))
            //    {
            //        paras.AddPara("PrimarySmtpAddress", pPrimarySmtpAddress);
            //    }
            //}
            #endregion

            if (pImapEnabled.HasValue)
            {
                paras.AddPara("ImapEnabled", pImapEnabled.Value);
            }
            if (pMAPIEnabled.HasValue)
            {
                paras.AddPara("MAPIEnabled", pMAPIEnabled.Value);
            }
            if (pOWAEnabled.HasValue)
            {
                paras.AddPara("OWAEnabled", pOWAEnabled.Value);
            }
            if (pPopEnabled.HasValue)
            {
                paras.AddPara("PopEnabled", pPopEnabled.Value);
            }
            if (!string.IsNullOrEmpty(pSamAccountName))
            {
                paras.AddPara("SamAccountName", pSamAccountName);
            }

            paras.AddPara("OWAMailboxPolicy", "Default");
            try
            {
                ExchangePSProvider.PSCommandSetCASMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetCASMailbox>{0}</SetCASMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetOWAMailboxPolicy(string pUPN, ref string pStrError)
        {
            string OWAMailboxPolicy = ConfigADProvider.GetOWAMailboxPolicy();

            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pUPN);
            paras.AddPara("OWAMailboxPolicy", OWAMailboxPolicy);

            try
            {
                ExchangePSProvider.PSCommandSetCASMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<SetCASMailbox>{0}</SetCASMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// RemoveMailbox
        /// by means of Remove-Mailbox
        /// </summary>
        /// <param name="pUPN">UPN, example: user1@domain.com</param>
        /// <param name="pPermanent">Permanent, example: user1@domain.com</param>
        /// <param name="pStrError">Error Info</param>
        /// <returns></returns>
        public static bool RemoveMailbox(
            string pUPN,
            Nullable<bool> pPermanent,
            ref string pStrError)
        {
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pUPN);
            if (pPermanent.HasValue)
            {
                paras.AddPara("Permanent", pPermanent.Value);
            }
            try
            {
                ExchangePSProvider.PSCommandRemoveMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<RemoveMailbox>{0}</RemoveMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        #region not materialize
        public static bool EnableMailbox(string userMail,  string alias, out string err)
        {
            err = string.Empty;
            try
            {
               
               // string ABP = userMail.Substring(userMail.IndexOf('@') + 1) + " ABP";
                PSParameters paras = new PSParameters();

                paras.AddPara("Identity", userMail);
                paras.AddPara("Alias", alias);
                // paras.AddPara("AddressBookPolicy", ABP);
                // paras.AddPara("PrimarySmtpAddress", userMail);
                paras.AddPara("Confirm", false);
                ExchangePSProvider.PSCommandEnableMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                err += string.Format("<EnableMailbox>{0}</EnableMailbox>", ex.ToString());
                return false;
            }
            return true;
        }
        
        public static bool EnableMailboxABP(string userMail, string orgID, out string err)
        {
            err = string.Empty;
            try
            {
                string ABP = orgID + " ABP";
                PSParameters paras = new PSParameters();

                paras.AddPara("Identity", userMail);
                paras.AddPara("Alias", userMail.Replace("@", "_"));
                paras.AddPara("AddressBookPolicy", ABP);
                paras.AddPara("PrimarySmtpAddress", userMail);
                paras.AddPara("Confirm", false);
                ExchangePSProvider.PSCommandEnableMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                err += string.Format("<EnableMailbox>{0}</EnableMailbox>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool EnableMailbox(string email, string alias, string database, out string strError)
        {
            strError = string.Empty;
            if (email.Contains("@"))
            {
                PSParameters paras = new PSParameters();

                paras.AddPara("Identity", email);

                paras.AddPara("Alias", alias);
                //paras.AddPara("PrimarySmtpAddress", email);//x400邮箱地址设置，设置后会改变属性
                paras.AddPara("Database", database);

                try
                {
                    ExchangePSProvider.PSCommandEnableMailBox.ExecuteCmdlet(paras);
                }
                catch (Exception ex)
                {
                    strError += string.Format("<EnableMailbox>{0}</EnableMailbox>", ex.ToString());
                    return false;
                }
            }
            else
            {
                strError = "邮箱格式不正确，为：" + email;
                return false;
            }
            return true;
        }

        public static bool EnableMailboxArchive(string email, out string strError)
        {
            strError = string.Empty;
            
            try
            {
                string cmd = $"Enable-Mailbox -Identity {email} -Archive";
                PSCommandBase.ExecuteCmdlet(cmd);
            }
            catch (Exception ex)
            {
                strError += string.Format("<EnableMailboxArchive>{0}</EnableMailboxArchive>", ex.ToString());
                return false;
            }

            return true;
        }

        public static bool SetMailboxRegionalConfiguration(string userMail, out string strError)
        {
            strError = string.Empty;
            //PSParameters paras = new PSParameters();

            //paras.AddPara("Identity", userMail);

            //paras.AddPara("Language", "zh-cn");
            ////paras.AddPara("PrimarySmtpAddress", email);//x400邮箱地址设置，设置后会改变属性
            //paras.AddPara("TimeZone", "China Standard Time");

            try
            {
                //ExchangePSProvider.PSCommandSetMailboxRegionalConfiguration.ExecuteCmdlet(paras);
                string cmd = $"Set-MailboxRegionalConfiguration -Identity {userMail} -Language 'zh-cn' -TimeZone 'China Standard Time' -LocalizeDefaultFolderName";
                PSCommandBase.ExecuteCmdlet(cmd);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetMailboxRegionalConfiguration>{0}</SetMailboxRegionalConfiguration>", ex.ToString());
                return false;
            }


            return true;
        }

        public static bool DisableMailbox(string email, out string strError)
        {
            strError = string.Empty;
            PSParameters paras = new PSParameters();

            paras.AddPara("Identity", email);
            paras.AddPara("Confirm", false);
            // paras.AddPara("PrimarySmtpAddress", email);

            try
            {
                ExchangePSProvider.PSCommandDisableMailBox.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<DisableMailbox>{0}</DisableMailbox>", ex.ToString());
                return false;
            }
            return true;
        }
        #endregion

        #endregion

        public static bool GetDatabaseList(out List<MailDataBaseInfo> databaseList, out string strError)
        {
            strError = string.Empty;
            databaseList = new List<MailDataBaseInfo>();

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailboxDatabase.ExecuteCmdlet();

                foreach (PSObject o in result)
                {
                    MailDataBaseInfo info = new MailDataBaseInfo();
                    info.MailboxDBID = Guid.Parse(Convert.ToString(o.Members["Guid"].Value));
                    info.MailboxDB = o.Members["Name"].Value.ToString();
                    info.MailboxServer = o.Members["Server"].Value.ToString();
                    string ProhibitSendReceiveQuota = o.Members["ProhibitSendReceiveQuota"].Value.ToString();
                    info.MailSizeName = ProhibitSendReceiveQuota.Substring(0, ProhibitSendReceiveQuota.IndexOf("(")).Trim();
                    info.MailSize = long.Parse(ProhibitSendReceiveQuota.Split('(')[1].ToString().Replace("bytes)", "").Replace(",", "").Trim());
                    databaseList.Add(info);
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetMailboxDatabase>{0}</GetMailboxDatabase>", ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取用户邮箱空间，返回bytes
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="sizename"></param>
        /// <param name="MaxValue"></param>
        /// <param name="databaseName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static bool GetEmailInfoValue(string Email, out string sizename, out long MaxValue, out string databaseName, out string strError)
        {
            strError = string.Empty;
            MaxValue = 0;
            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            databaseName = string.Empty;
            sizename = string.Empty;
            string FactSpace = string.Empty;
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                //  23.44 MB (24,576,000 bytes)
                foreach (PSObject o in result)
                {
                    databaseName = Convert.ToString(o.Members["Database"].Value);
                    FactSpace = Convert.ToString(o.Members["ProhibitSendReceiveQuota"].Value);

                    if (FactSpace.ToLower() == "unlimited")
                    {
                        sizename = "unlimited";
                        MaxValue = -1;
                        break;
                    }
                    else
                    {
                        sizename = FactSpace.Split('(')[0].ToString().Trim();
                        MaxValue = long.Parse(FactSpace.Split('(')[1].ToString().Replace("bytes)", "").Replace(",", "").Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetEmailMaxValue>{0}</GetEmailMaxValue>", ex.ToString());
                return false;
            }
            finally
            {
            }

            return true;
        }

        public static bool GetEmailInfoValue(string Email, out long MaxValue, out string databaseName, out string strError)
        {
            strError = string.Empty;
            MaxValue = 0;
            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            string FactSpace = string.Empty;
            databaseName = string.Empty;
            
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                //  23.44 MB (24,576,000 bytes)

                foreach (PSObject o in result)
                {
                    databaseName = Convert.ToString(o.Members["Database"].Value);
                    FactSpace = Convert.ToString(o.Members["ProhibitSendReceiveQuota"].Value);
                    
                    if (FactSpace.ToLower() == "unlimited")
                    {
                        MaxValue = -1;
                        break;
                    }
                    FactSpace = FactSpace.Split('(')[0].ToString();
                    FactSpace = FactSpace.Trim();

                    if (FactSpace.IndexOf("GB") > -1)
                    {
                        FactSpace = FactSpace.Replace("GB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace));
                        break;
                    }
                    else if (FactSpace.IndexOf("MB") > -1)
                    {
                        FactSpace = FactSpace.Replace("MB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) / 1024);
                        break;
                    }
                    else if (FactSpace.IndexOf("KB") > -1)
                    {
                        FactSpace = FactSpace.Replace("KB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) / (1024 * 1024));
                        break;
                    }
                    else if (FactSpace.IndexOf("B") > -1)
                    {
                        FactSpace = FactSpace.Replace("B", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) / (1024 * 1024 * 1024));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetEmailMaxValue>{0}</GetEmailMaxValue>", ex.ToString());
                return false;
            }
            finally
            {
            }

            return true;
        }

        public static bool GetUserExchange(string email, out ExchangeUserInfo userExchange, out string strError)
        {
            strError = string.Empty;
            userExchange = new ExchangeUserInfo();
            long MailSize = -1;
            string databaseName = string.Empty;

            if (!GetEmailInfoValue(email, out MailSize, out databaseName, out strError))
            {
                LoggerHelper.Error("GetEmailInfoValue", email, strError, Guid.NewGuid());
                return false;
            }
            userExchange.mailSize = Convert.ToInt32(MailSize);
            userExchange.database = databaseName;
            try
            {
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", email));
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetCASMailBox.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {
                    if (o.Members["OWAEnabled"].Value != null && (bool)o.Members["OWAEnabled"].Value == true)
                    {
                        userExchange.OWAEnabled = true;
                    }
                    if (o.Members["PopEnabled"].Value != null && (bool)o.Members["PopEnabled"].Value == true)
                    {
                        userExchange.PopEnabled = true;
                    }
                    if (o.Members["ImapEnabled"].Value != null && (bool)o.Members["ImapEnabled"].Value == true)
                    {
                        userExchange.ImapEnabled = true;
                    }
                    if (o.Members["MAPIEnabled"].Value != null && (bool)o.Members["MAPIEnabled"].Value == true)
                    {
                        userExchange.MAPIEnabled = true;
                    }
                    if (o.Members["ActiveSyncEnabled"].Value != null && (bool)o.Members["ActiveSyncEnabled"].Value == true)
                    {
                        userExchange.ActiveSyncEnabled = true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }
        }

        public static bool UpdateUserAlias(string userAccount, string alias, out string strError)
        {
            strError = string.Empty;
            UserInfo user = new UserInfo();

            if (GetUser(userAccount, out user, out strError))
            {
                PSParameters paras = new PSParameters();
                paras.AddPara("Identity", userAccount);
                paras.AddPara("Alias", alias);

                try
                {
                    ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(paras);
                }
                catch (Exception ex)
                {
                    strError += string.Format("<SetMailbox>{0}</SetMailbox>", ex.ToString());
                    return false;
                }
            }

            return true;
        }

        public static bool GetUser(string userID, out UserInfo user, out string strError)
        {
            user = new UserInfo();
            strError = string.Empty;

            try
            {
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", userID));
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {

                    user.UserID = Guid.Parse(o.Members["Guid"].Value.ToString());
                    user.DisplayName = o.Members["DisplayName"].Value.ToString();
                    user.AliasName = o.Members["Alias"].Value.ToString();
                    user.UserAccount = o.Members["PrimarySmtpAddress"].Value.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }
        }

        public static bool UpdateExchangeUser(string userEmail,
            string displayName,
            string alias,
           ExchangeInfo userExchange,
            out string strError)
        {
            strError = string.Empty;
            
            //启用邮箱邮箱
            if (userExchange.ExchangeStatus)
            {
                //用户不存在
                ExchangeUserInfo exchange = new ExchangeUserInfo();
                if (!GetUserExchange(userEmail, out  exchange, out strError))
                {
                    if (string.IsNullOrEmpty(userExchange.Database))
                    {
                        //启用用户邮箱
                        if (!EnableMailbox(userEmail, alias, out strError))
                        {
                            Log4netHelper.Error("EnableMailbox" + strError);
                            return false;
                        }

                        if (!EnableMailboxArchive(userEmail, out strError))
                        {
                            Log4netHelper.Error("EnableMailboxArchive" + strError);
                            return false;
                        }
                        if (!SetMailboxRegionalConfiguration(userEmail, out strError))
                        {
                            Log4netHelper.Error("SetMailboxRegionalConfiguration" + strError);
                            return false;
                        }
                    }
                    else
                    {
                        //启用用户邮箱
                        if (!EnableMailbox(userEmail, alias, userExchange.Database, out strError))
                        {
                            Log4netHelper.Error("EnableMailbox" + strError);
                            return false;
                        }

                        if (!EnableMailboxArchive(userEmail, out strError))
                        {
                            Log4netHelper.Error("EnableMailboxArchive" + strError);
                            return false;
                        }
                        if (!SetMailboxRegionalConfiguration(userEmail, out strError))
                        {
                            Log4netHelper.Error("SetMailboxRegionalConfiguration" + strError);
                            return false;
                        }
                    }

                    //int sleepsec = Convert.ToInt32(ConfigADProvider.GetThreadSleep());
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(sleepsec));

                    ExchangeUserInfo exchangeUserInfo = new ExchangeUserInfo();
                    //修改用户邮箱属性
                    exchangeUserInfo.mailSize = userExchange.MailSize;
                    exchangeUserInfo.MAPIEnabled = userExchange.Mapi;
                    exchangeUserInfo.ActiveSyncEnabled = userExchange.Activesync;
                    exchangeUserInfo.PopEnabled = userExchange.POP3;
                    exchangeUserInfo.ImapEnabled = userExchange.Imap4;
                    exchangeUserInfo.OWAEnabled = userExchange.OWA;
                    exchangeUserInfo.database = string.Empty;//mailDatabase;

                    if (!UpdateUserExchange(userEmail, displayName, exchangeUserInfo, out strError))
                    {
                        Log4netHelper.Error("UpdateUserExchange ：" + strError);
                        return false;
                    }
                }
                //用户邮箱存在
                else
                {
                    //修改用户邮箱属性
                    ExchangeUserInfo exchangeUserInfo = new ExchangeUserInfo();
                    //修改用户邮箱属性
                    exchangeUserInfo.mailSize = userExchange.MailSize;
                    exchangeUserInfo.MAPIEnabled = userExchange.Mapi;
                    exchangeUserInfo.ActiveSyncEnabled = userExchange.Activesync;
                    exchangeUserInfo.PopEnabled = userExchange.POP3;
                    exchangeUserInfo.ImapEnabled = userExchange.Imap4;
                    exchangeUserInfo.OWAEnabled = userExchange.OWA;
                    exchangeUserInfo.database = string.Empty;//mailDatabase;

                    if (!UpdateUserExchange(userEmail, displayName, exchangeUserInfo, out strError))
                    {
                        Log4netHelper.Error("UpdateUserExchange ：" + strError);
                        return false;
                    }
                }
            }
            //禁用用户邮箱
            else
            {
                ExchangeUserInfo exchange = new ExchangeUserInfo();
                if (!GetUserExchange(userEmail, out exchange, out strError))
                {
                    return true;
                }
                //用户邮箱存在
                else
                {
                    //if (!DisableMailbox(userEmail, out strError))
                    //{
                    //    Log4netHelper.Error("DisableMailbox ：" + strError);
                    //    return false;
                    //}
                }
            }
            return true;
        }
        
        public static bool UpdateUserExchange(string email, string displayName, ExchangeUserInfo userExchange, out string strError)
        {
            strError = string.Empty;
            bool result = false;

            if (string.IsNullOrEmpty(userExchange.database))
            {
                strError = "Exchange database can't be null.";
            }

            #region 邮箱空间参数
            //邮箱空间单位转换为 GB
            long mailSize = userExchange.mailSize;

            int nOut = 0;
            //Info：WarnMailSize：90;MaxSendMailSize:10;ReceipentNum:200;MailSize:1000',@Result=N'True',@ID=@p5 output


            //用户警告空间计算 单位：GB
            long warningSize = (long)(mailSize * 0.9);
            if (userExchange.WarnMailSize > 0)
            {
                warningSize = (long)(userExchange.WarnMailSize * 0.01 * userExchange.mailSize);
            }

            //计算发送和接收邮件大小，用附件参数控制, 单位：GB
            long sendReceiveSize = (long)(mailSize * 0.9);
            if (userExchange.MaxSendMailSize > 0)
            {
                sendReceiveSize = userExchange.MaxSendMailSize;
            }

            long maxSendSize = 0;
            if (userExchange.MaxSendMailSize > 0)
            {
                maxSendSize = userExchange.MaxSendMailSize;
            }
            long maxReceiveSize = 0;
            if (userExchange.MaxSendMailSize > 0)
            {
                maxReceiveSize = userExchange.MaxSendMailSize;
            }
            #endregion

            string username = email.Substring(0, email.IndexOf('@'));

            string domain = email.Substring(email.IndexOf('@') + 1);
            result = ChangeUserExchange(domain,
                username
                , userExchange.database
                , displayName, userExchange.mailSize, email, userExchange.ActiveSyncEnabled
                , userExchange.OWAEnabled, userExchange.PopEnabled, userExchange.ImapEnabled, userExchange.MAPIEnabled
                , warningSize, null, null, mailSize, mailSize
                , userExchange.OWACalendarEnabled, userExchange.OWAContactsEnabled, userExchange.OWAJournalEnabled, userExchange.OWAJunkEmailEnabled
                , userExchange.OWANotesEnabled, userExchange.OWATasksEnabled, userExchange.OWARulesEnabled, userExchange.OWASMimeEnabled
                , true, true, true, true, true, true
                , out strError);


            return result;
        }

        public static bool ChangeUserExchange(
            string pDomainName,
            string pAccount,
            string pDatabase,
            string pDisName,
            Nullable<long> pMailSize,
            string pMailAddress,
            //string pPassword,
            Nullable<bool> ActiveSyncEnabled,
            Nullable<bool> OWAEnabled,
            Nullable<bool> PopEnabled,
            Nullable<bool> ImapEnabled,
            Nullable<bool> MAPIEnabled,
            Nullable<long> pIssueWarningQuota,
            Nullable<long> pMaxReceiveSize,
            Nullable<long> pMaxSendSize,
            Nullable<long> pProhibitSendQuota,
            Nullable<long> pProhibitSendReceiveQuota,
            Nullable<bool> pOWACalendarEnabled,
            Nullable<bool> pOWAContactsEnabled,
            Nullable<bool> pOWAJournalEnabled,
            Nullable<bool> pOWAJunkEmailEnabled,
            Nullable<bool> pOWANotesEnabled,
            Nullable<bool> pOWATasksEnabled,
            Nullable<bool> pOWARulesEnabled,
            Nullable<bool> pOWASMimeEnabled,
            Nullable<bool> pOWARemindersAndNotificationsEnabled,
            Nullable<bool> pOWAPremiumClientEnabled,
            Nullable<bool> pOWASpellCheckerEnabled,
            Nullable<bool> pOWASearchFoldersEnabled,
            Nullable<bool> pOWASignaturesEnabled,
            Nullable<bool> pOWAActiveSyncIntegrationEnabled,
            out string strError)
        {
            strError = string.Empty;
            bool result = true;
            //string pMailAddress = string.Format("{0}@{1}", pAccount, pDomainName);
            List<string> addresses = new List<string>();
            //addresses.Add(pMailAddress);

            //设置mailbox邮箱空间大小
            if (pMailSize != -1)
            {
                if (result)
                {
                    result = SetMailbox(pDatabase, pMailAddress, pDomainName, pDisName, addresses, pIssueWarningQuota, "zh-cn", null, pMaxReceiveSize,
                    null, pMaxSendSize, null, pMailAddress, pProhibitSendQuota, pProhibitSendReceiveQuota,
                    null, null, null, null, null, null, null, null, null, null, null,
                    false, ref strError);
                }
            }
            else
            {
                if (result)
                {
                    result = SetMailbox(pDatabase, pMailAddress, pDomainName, pDisName, addresses, pMailSize, "zh-cn", null, pMailSize,
                    null, pMailSize, null, pMailAddress, pMailSize, pMailSize,
                    null, null, null, null, null, null, null, null, null, null, null,
                    true, ref strError);
                }
            }
            if (result)
            {
                result = SetCASMailbox(
                    pMailAddress,
                    ActiveSyncEnabled,
                    pDisName,
                    addresses,
                    ImapEnabled,
                    MAPIEnabled,
                    OWAEnabled,
                    PopEnabled,
                    pOWACalendarEnabled, pOWAContactsEnabled, pOWAJournalEnabled, pOWAJunkEmailEnabled,
                    pOWANotesEnabled, pOWATasksEnabled, pOWARulesEnabled, pOWASMimeEnabled,
                    pOWARemindersAndNotificationsEnabled, pOWAPremiumClientEnabled, pOWASpellCheckerEnabled,
                    pOWASearchFoldersEnabled, pOWASignaturesEnabled, pOWAActiveSyncIntegrationEnabled,
                    "",
                    null,
                    ref strError);
            }

            return result;
        }

        public static bool GetUsedMailBoxSize(string Email, out long UsedMailBoxSize, out string strError)
        {
            UsedMailBoxSize = 0;
            strError = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailboxStatistics.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {
                    string uSize = o.Members["TotalItemSize"].Value.ToString().Split('(')[1].ToString().Replace("bytes)", "").Replace(",", "").Trim();
                    UsedMailBoxSize = long.Parse(uSize);
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetUsedMailBoxSize>{0}</GetUsedMailBoxSize>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetUsedMailBoxSize(string Email,out string UsedSizeName, out long UsedMailBoxSize,out string DatabaseProhibitSendQuotaName,out long DatabaseProhibitSendQuotaSize, out string strError)
        {
            UsedSizeName = string.Empty;
            UsedMailBoxSize = 0;
            DatabaseProhibitSendQuotaName = string.Empty;
            DatabaseProhibitSendQuotaSize = 0;
            strError = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailboxStatistics.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {
                    string TotalItemSize = o.Members["TotalItemSize"].Value.ToString();
                    UsedSizeName = TotalItemSize.Substring(0, TotalItemSize.IndexOf("(")).Trim();
                    string uSize = TotalItemSize.Split('(')[1].ToString().Replace("bytes)", "").Replace(",", "").Trim();
                    UsedMailBoxSize = long.Parse(uSize);
                    string DatabaseProhibitSendQuota = o.Members["DatabaseProhibitSendQuota"].Value.ToString();
                    DatabaseProhibitSendQuotaName = DatabaseProhibitSendQuota.Substring(0, DatabaseProhibitSendQuota.IndexOf("(")).Trim();
                    DatabaseProhibitSendQuotaSize = long.Parse(DatabaseProhibitSendQuota.Split('(')[1].ToString().Replace("bytes)", "").Replace(",", "").Trim());
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetUsedMailBoxSize>{0}</GetUsedMailBoxSize>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetPersonalJunkMail(string Email, bool IsAutoSelect, int intSelect, bool IsDelete, int intDelete, out string strError)
        {
            strError = string.Empty;
            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            para.Add(new PSParameter("SCLJunkEnabled", IsAutoSelect));
            para.Add(new PSParameter("SCLJunkThreshold", intSelect));
            para.Add(new PSParameter("SCLDeleteEnabled", IsDelete));
            para.Add(new PSParameter("SCLDeleteThreshold", intDelete));
            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetPersonalJunkMail>{0}</SetPersonalJunkMail>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetEmailAlarm(string Email, out long AlarmValue, out string strError)
        {
            strError = string.Empty;
            AlarmValue = 0;
            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            string FactSpace = string.Empty;

            try
            {

                ICollection<PSObject> res = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                //  1.717 GB (1,843,200,000 bytes)


                foreach (PSObject o in res)
                {
                    FactSpace = Convert.ToString(o.Members["IssueWarningQuota"].Value);

                    FactSpace = FactSpace.Split('(')[0].ToString();
                    FactSpace = FactSpace.Trim();

                    if (FactSpace.IndexOf("GB") > -1)
                    {
                        FactSpace = FactSpace.Replace("GB", "").Trim();
                        AlarmValue = (long)(double.Parse(FactSpace) * 1000);
                        break;
                    }
                    else if (FactSpace.IndexOf("MB") > -1)
                    {
                        FactSpace = FactSpace.Replace("MB", "").Trim();
                        AlarmValue = (long)(double.Parse(FactSpace));
                        break;
                    }
                    else if (FactSpace.IndexOf("KB") > -1)
                    {
                        FactSpace = FactSpace.Replace("KB", "").Trim();
                        AlarmValue = (long)(double.Parse(FactSpace) / 1000);
                        break;
                    }
                    else if (FactSpace.IndexOf("B") > -1)
                    {
                        FactSpace = FactSpace.Replace("B", "").Trim();
                        AlarmValue = (long)(double.Parse(FactSpace) / (1000 * 1000));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetEmailAlarm>{0}</GetEmailAlarm>", ex.ToString());
                return false;
            }
            finally
            {
            }

            return true;
        }

        public static bool SetEmailAlarm(string Email, double AlarmRate, out string strError)
        {
            strError = string.Empty;

            long MaxValue = 0;
            if (!GetEmailMaxValue(Email, out MaxValue, out strError))
            {
                return false;
            }

            long AlarmValue = (long)(MaxValue * AlarmRate);

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            para.Add(new PSParameter("IssueWarningQuota", AlarmValue.ToString() + "MB"));
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetEmailAlarm>{0}</SetEmailAlarm>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetEmailMaxValue(string Email, out long MaxValue, out string strError)
        {
            strError = string.Empty;
            MaxValue = 0;
            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));
            string FactSpace = string.Empty;
            try
            {

                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                //  23.44 MB (24,576,000 bytes)

                foreach (PSObject o in result)
                {
                    FactSpace = Convert.ToString(o.Members["ProhibitSendReceiveQuota"].Value);
                    FactSpace = FactSpace.Split('(')[0].ToString();
                    FactSpace = FactSpace.Trim();

                    if (FactSpace.IndexOf("GB") > -1)
                    {
                        FactSpace = FactSpace.Replace("GB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) * 1000);
                        break;
                    }
                    else if (FactSpace.IndexOf("MB") > -1)
                    {
                        FactSpace = FactSpace.Replace("MB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace));
                        break;
                    }
                    else if (FactSpace.IndexOf("KB") > -1)
                    {
                        FactSpace = FactSpace.Replace("KB", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) / 1000);
                        break;
                    }
                    else if (FactSpace.IndexOf("B") > -1)
                    {
                        FactSpace = FactSpace.Replace("B", "").Trim();
                        MaxValue = (long)(double.Parse(FactSpace) / (1000 * 1000));
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetEmailMaxValue>{0}</GetEmailMaxValue>", ex.ToString());
                return false;
            }
            finally
            {
            }

            return true;
        }

        public static bool GetEmailJunkSet(string Email, out bool IsAutoSelect, out int intSelect, out bool IsDelete, out int intDelete, out string pStrError)
        {
            IsAutoSelect = false;
            intSelect = 0;
            IsDelete = false;
            intDelete = 0;
            pStrError = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", Email));

            try
            {

                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetMailBox.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {
                    if (o.Members["SCLJunkEnabled"].Value != null && (bool)o.Members["SCLJunkEnabled"].Value == true)
                    {
                        IsAutoSelect = true;
                        intSelect = (int)(o.Members["SCLJunkThreshold"].Value);
                    }
                    else
                    {
                        IsAutoSelect = false;
                    }

                    if (o.Members["SCLDeleteEnabled"].Value != null && (bool)o.Members["SCLDeleteEnabled"].Value == true)
                    {
                        IsDelete = true;
                        intDelete = (int)(o.Members["SCLDeleteThreshold"].Value);
                    }
                    else
                    {
                        IsDelete = false;
                        intDelete = 9;
                    }
                }
            }
            catch (Exception ex)
            {
                pStrError += string.Format("<GetEmailJunkSet>{0}</GetEmailJunkSet>", ex.ToString());
                return false;
            }
            return true;
        }

        public static enDomainALStatus GetALStatus(string strDomain, out string alPath, out string galPath)
        {
            bool al = true;
            bool gal = true;
            alPath = string.Empty;
            galPath = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", strDomain + " AL"));
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetAL.ExecuteCmdlet(para);

                foreach (PSObject o in result)
                {

                    alPath = Convert.ToString(o.Members["DistinguishedName"].Value);
                }

            }
            catch (Exception ex)
            {
                al = false;
            }
            finally
            {
            }

            para.Clear();
            para.Add(new PSParameter("Identity", strDomain + " GAL"));
            try
            {
                ICollection<PSObject> res = ExchangePSProvider.PSCommandGetGAL.ExecuteCmdlet(para);
                foreach (PSObject o in res)
                {
                    galPath = Convert.ToString(o.Members["DistinguishedName"].Value);
                }
            }
            catch (Exception ex)
            {
                gal = false;
            }
            finally
            {
            }

            if (al && gal)
            {
                return enDomainALStatus.all;
            }
            if (al && !gal)
            {
                return enDomainALStatus.al;
            }
            if (!al && gal)
            {
                return enDomainALStatus.gal;
            }
            if (!al && !gal)
            {
                return enDomainALStatus.none;
            }
            return enDomainALStatus.none;
        }

        public static bool RemoveContact(string contactId, out string strError)
        {
            strError = string.Empty;

            PSParameters para = new PSParameters();

            para.Add(new PSParameter("Identity", contactId));
            para.Add(new PSParameter("Confirm", false));

            try
            {
                ICollection<PSObject> res = ExchangePSProvider.PSCommandRemoveContact.ExecuteCmdlet(para);

            }
            catch (Exception ex)
            {
                strError += string.Format("<RemoveContact>{0}</RemoveContact>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool RemoveDevice(string Email, out string strError)
        {
            strError = string.Empty;
            string CmdText = string.Format("Get-ActiveSyncDeviceStatistics -mailbox {0}| Clear-ActiveSyncDevice -confirm:$false", Email);

            try
            {
                // ICollection<PSObject> res = ExchangePSProvider.PSCommandRemoveDevice.ExecuteCmdlet(CmdText);

            }
            catch (Exception ex)
            {
                strError += string.Format("<RemoveContact>{0}</RemoveContact>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetMailBoxInfoPowerShell(string strName, int MailSize, int MaxSendSize, int WaringSize, int RecipientLimits, out string strError)
        {
            strError = string.Empty;

            long nNewSize = MailSize;
            nNewSize = nNewSize * 1000 * 1024;


            long nWarningSize = (long)(nNewSize * (WaringSize * 0.01));

            long MaxSendMailSize = MaxSendSize * 1000 * 1024;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", strName));
            //para.Add(new PSParameter("MaxReceiveSize", Microsoft.Exchange.Data.Unlimited<long>.UnlimitedString));
            para.Add(new PSParameter("MaxSendSize", MaxSendMailSize));
            para.Add(new PSParameter("IssueWarningQuota", nWarningSize));
            para.Add(new PSParameter("ProhibitSendQuota", nNewSize));
            para.Add(new PSParameter("ProhibitSendReceiveQuota", nNewSize));
            para.Add(new PSParameter("EmailAddressPolicyEnabled", false));
            para.Add(new PSParameter("UseDatabaseQuotaDefaults", false));
            para.Add(new PSParameter("RecipientLimits", RecipientLimits));

            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetUserMaxSendSizeValue>{0}</SetUserMaxSendSizeValue>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetUserMailSizeValue(string strName, long MailSize, out string strError)
        {
            strError = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", strName));
            //para.Add(new PSParameter("MaxReceiveSize", Microsoft.Exchange.Data.Unlimited<long>.UnlimitedString));
            //para.Add(new PSParameter("MaxSendSize", MaxSendMailSize));
            para.Add(new PSParameter("IssueWarningQuota", MailSize));
            para.Add(new PSParameter("ProhibitSendQuota", MailSize));
            para.Add(new PSParameter("ProhibitSendReceiveQuota", MailSize));
            //para.Add(new PSParameter("EmailAddressPolicyEnabled", false));
            //para.Add(new PSParameter("UseDatabaseQuotaDefaults", false));
            //para.Add(new PSParameter("RecipientLimits", RecipientLimits));


            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetUserMailSizeValue>{0}</SetUserMailSizeValue>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetUserMailSizeValue(string strName, long MailSize, long MaxSize, double warningSize, out string strError)
        {
            strError = string.Empty;

            PSParameters para = new PSParameters();
            para.Add(new PSParameter("Identity", strName));
            para.Add(new PSParameter("MaxReceiveSize", MaxSize + "MB"));
            para.Add(new PSParameter("MaxSendSize", MaxSize + "MB"));
            if (MailSize >= 0)
            {
                para.Add(new PSParameter("IssueWarningQuota", MailSize * warningSize + "MB"));
                para.Add(new PSParameter("ProhibitSendQuota", MailSize + "MB"));
                para.Add(new PSParameter("ProhibitSendReceiveQuota", MailSize + "MB"));
            }
            else
            {
                para.Add(new PSParameter("IssueWarningQuota", "unlimited"));
                para.Add(new PSParameter("ProhibitSendQuota", "unlimited"));
                para.Add(new PSParameter("ProhibitSendReceiveQuota", "unlimited"));
            }
            //para.Add(new PSParameter("EmailAddressPolicyEnabled", false));
            para.Add(new PSParameter("UseDatabaseQuotaDefaults", false));
            //para.Add(new PSParameter("RecipientLimits", RecipientLimits));


            try
            {
                ExchangePSProvider.PSCommandSetMailBox.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetUserMailSizeValue>{0}</SetUserMailSizeValue>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool SetADForest(
            string pName,
            string pType,
            ref string strError)
        {
            try
            {
                ExchangePSProvider.PSCommandSetADForest.ExecuteCmdlet(pName, pType);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetADForest>{0}</SetADForest>", ex.ToString());
                return false;
            }
            finally
            {
            }
            return true;
        }

        public static bool NewDisGroup(ref GroupInfo group, out string strError)
        {
            strError = string.Empty;

            PSParameters paras = new PSParameters();
            paras.AddPara("Name", group.DisplayName);
            paras.AddPara("Alias", group.Account.Substring(0, group.Account.IndexOf('@')));
            paras.AddPara("PrimarySmtpAddress", group.Account);
            paras.AddPara("OrganizationalUnit", group.ParentDistinguishedName);

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandNewDistribitionGroup.ExecuteCmdlet(paras);
                foreach (PSObject o in result)
                {
                    if (o.Members["Guid"].Value != null)
                    {
                        group.GroupID = Guid.Parse(Convert.ToString(o.Members["Guid"].Value));
                    }
                }
            }
            catch (Exception ex)
            {
                strError += string.Format("<NewDistribitionGroup>{0}</NewDistribitionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool EnableDisGroup(string pName, out string strError)
        {
            strError = string.Empty;
            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);

            try
            {
                ExchangePSProvider.PSCommandEnableDistributionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<EnableDistributionGroup>{0}</EnableDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool EnableDisGroup(string pName, string pEmailAddress, out string strError)
        {
            strError = string.Empty;

            PSParameters paras = new PSParameters();
            paras.AddPara("Identity", pName);
            paras.AddPara("Alias", pEmailAddress.Substring(0, pEmailAddress.IndexOf('@')));
            paras.AddPara("PrimarySmtpAddress", pEmailAddress);

            try
            {
                ExchangePSProvider.PSCommandEnableDistributionGroup.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<EnableDistributionGroup>{0}</EnableDistributionGroup>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool SetUserPhoto(string strMail, byte[] photoStream, out string strError)
        {
            PSParameters paras = new PSParameters();
            strError = string.Empty;

            paras.AddPara("Identity", strMail);
            paras.AddPara("PictureData", photoStream);
            paras.AddPara("Confirm", false);

            try
            {
                ExchangePSProvider.PSCommandSetUserPhoto.ExecuteCmdlet(paras);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetUserPhoto>{0}</SetUserPhoto>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool RemoveSensitiveMail(string strscope, string strSearchQuery, DateTime startTime, DateTime endTime, out string resultMessage, out string strError)
        {
            resultMessage = string.Empty;
            strError = string.Empty;
            try
            {
                ICollection<PSObject> result = ExchangePSProvider.PSCommandRemoveSensitiveMail.ExecuteCmdlet(strscope, strSearchQuery, startTime.AddHours(-8), endTime.AddHours(-8));

                foreach (PSObject o in result)
                {
                    string user = o.Members["Identity"].Value.ToString();
                    string result1 = o.Members["Success"].Value.ToString();
                    string ResultItemsCount = o.Members["ResultItemsCount"].Value.ToString();
                    resultMessage += $"用户：{user}，查询结果：{result1}，查询对象数量：{ResultItemsCount}";
                    Log4netHelper.Info($"User:{user}，result：{result1}，ResultItemsCount：{ResultItemsCount}");
                }
                resultMessage = string.IsNullOrEmpty(resultMessage) ? "无查询结果" : resultMessage.Remove(resultMessage.LastIndexOf('，'), 1);
                Log4netHelper.Info("PSCommandRemoveSensitiveMail result :" + result.Count);
            }
            catch (Exception ex)
            {
                strError = string.Format("<RemoveSensitiveMail>{0}</RemoveSensitiveMail>", ex.ToString());
                resultMessage += "删除敏感邮件操作异常：" + ex.Message;
                return false;
            }
            return true;
        }

        public static bool NewComplianceSearch(string name, string exchangeLocation, string strSearchQuery, DateTime startTime, DateTime endTime, out string strError)
        {
            strError = string.Empty;
            try
            {
                string contentMatchQuery = $"{strSearchQuery} AND (sent>=\"{startTime}\") AND (Sent<=\"{endTime}\")";

                //New - ComplianceSearch - Name "test 1111" - ExchangeLocation "kaisagroup@bnc2.cn" - ContentMatchQuery '(subject:"test") AND (sent>="2019/7/7 16:55:18") AND (Sent<="2019/7/8 15:00:18")'
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Name", name));
                para.Add(new PSParameter("ExchangeLocation", exchangeLocation));
                para.Add(new PSParameter("ContentMatchQuery", contentMatchQuery));

                ExchangePSProvider.PSCommandNewComplianceSearch.ExecuteCmdlet(para);

               // ICollection<PSObject> result = ExchangePSProvider.PSCommandNewComplianceSearch.ExecuteCmdlet(name, exchangeLocation, strSearchQuery, startTime, endTime);
                //Log4netHelper.Info("NewComplianceSearch result :" + result.Count);
            }
            catch (Exception ex)
            {
                strError += string.Format("<NewComplianceSearch>{0}</NewComplianceSearch>", ex.ToString());
                Log4netHelper.Error(strError);
                return false;
            }
            return true;
        }

        public static bool SetComplianceSearch(string name, string exchangeLocation, string strSearchQuery, DateTime startTime, DateTime endTime, out string strError)
        {
            strError = string.Empty;
            try
            {
                string contentMatchQuery = $"{strSearchQuery} AND (sent>=\"{startTime}\") AND (Sent<=\"{endTime}\")";
                
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", name));
                para.Add(new PSParameter("ExchangeLocation", exchangeLocation));
                para.Add(new PSParameter("ContentMatchQuery", contentMatchQuery));

                ExchangePSProvider.PSCommandSetComplianceSearch.ExecuteCmdlet(para);

                ICollection<PSObject> result = ExchangePSProvider.PSCommandSetComplianceSearch.ExecuteCmdlet(para);
                Log4netHelper.Info("SetComplianceSearch result :" + result.Count);
            }
            catch (Exception ex)
            {
                strError += string.Format("<SetComplianceSearch>{0}</SetComplianceSearch>", ex.ToString());
                return false;
            }
            return true;
        }
        
        public static bool StartComplianceSearch(string name, out string strError)
        {
            strError = string.Empty;
            try
            {
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", name));
                ICollection<PSObject> result = ExchangePSProvider.PSCommandStartComplianceSearch.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<StartComplianceSearch>{0}</StartComplianceSearch>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetComplianceSearch(string name, out string strError)
        {
            strError = string.Empty;
            try
            {
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", name));
                ICollection<PSObject> result = ExchangePSProvider.PSCommandGetComplianceSearch.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<GetComplianceSearch>{0}</GetComplianceSearch>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool RemoveComplianceSearch(string name, out string strError)
        {
            strError = string.Empty;
            try
            {
                PSParameters para = new PSParameters();
                para.Add(new PSParameter("Identity", name));
                para.Add(new PSParameter("Confirm", false)); 
                ICollection<PSObject> result = ExchangePSProvider.PSCommandRemoveComplianceSearch.ExecuteCmdlet(para);
            }
            catch (Exception ex)
            {
                strError += string.Format("<RemoveComplianceSearch>{0}</RemoveComplianceSearch>", ex.ToString());
                return false;
            }
            return true;
        }

        public static bool NewComplianceSearchAction(string name, out string strError)
        {
            strError = string.Empty;
            try
            {
                //PSParameters para = new PSParameters();
                //para.Add(new PSParameter("SearchName", name));
                //para.Add(new PSParameter("Purge"));
                //para.Add(new PSParameter("PurgeType", "SoftDelete"));
                //para.Add(new PSParameter("Confirm", false));
                string cmd = $"New-ComplianceSearchAction -SearchName \"{name}\" -Purge -PurgeType SoftDelete Confirm:$false";

                ExchangePSProvider.PSCommandNewComplianceSearchAction.ExecuteCmdlet(cmd, false);
            }
            catch (Exception ex)
            {
                strError += string.Format("<NewComplianceSearchAction>{0}</NewComplianceSearchAction>", ex.ToString());
                Log4netHelper.Error(strError);
                return false;
            }
            return true;
        }

        public static bool GetServerMailCount(DateTime starTime, DateTime endTime, out int sendCount, out int receiveCount, out string strError)
        {
            strError = string.Empty;
            sendCount = 0;
            receiveCount = 0;

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.GetServerreceiveMailCount.ExecuteCmdlet(starTime, endTime);

                foreach (PSObject o in result)
                {
                    if (o == null)
                    {
                        receiveCount = 0;
                    }
                    else
                    {
                        receiveCount = Convert.ToInt32(o.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.GetServerSendMailCount.ExecuteCmdlet(starTime, endTime);

                foreach (PSObject o in result)
                {
                    if (o == null)
                    {
                        sendCount = 0;
                    }
                    else
                    {
                        sendCount = Convert.ToInt32(o.ToString());
                    }
                }

                ICollection<PSObject> result1 = ExchangePSProvider.GetSMTPSendMailCount.ExecuteCmdlet(starTime, endTime);
                foreach (PSObject o in result1)
                {
                    if (o == null)
                    {
                        sendCount += 0;
                    }
                    else
                    {
                        sendCount += Convert.ToInt32(o.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }

            return true;
        }

        public static bool GetUserMailCount(DateTime starTime, DateTime endTime, string mail, out int sendCount, out int receiveCount, out string strError)
        {
            strError = string.Empty;
            sendCount = 0;
            receiveCount = 0;

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.GetUserreceiveMailCount.ExecuteCmdlet(starTime, endTime, mail);

                foreach (PSObject o in result)
                {
                    if (o == null)
                    {
                        receiveCount = 0;
                    }
                    else
                    {
                        receiveCount = Convert.ToInt32(o.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }

            try
            {
                ICollection<PSObject> result = ExchangePSProvider.GetUserSendServerMailCount.ExecuteCmdlet(starTime, endTime, mail);

                foreach (PSObject o in result)
                {
                    if (o == null)
                    {
                        sendCount = 0;
                    }
                    else
                    {
                        sendCount = Convert.ToInt32(o.ToString());
                        //DBUtility.Logger.Info(mail + "账户GetUserSendServerMailCount不为空发送邮件数量：" + sendCount.ToString() + "新增加数量为：" + Convert.ToInt32(o.ToString()));
                    }
                }

                ICollection<PSObject> result1 = ExchangePSProvider.GetUserSendSMTPMailCount.ExecuteCmdlet(starTime, endTime, mail);
                foreach (PSObject o in result1)
                {
                    if (o == null)
                    {
                        sendCount += 0;
                        //DBUtility.Logger.Info(mail + "账户GetUserSendSMTPMailCount发送邮件数量：" + sendCount.ToString());
                    }
                    else
                    {
                        sendCount = sendCount + Convert.ToInt32(o.ToString());
                        //DBUtility.Logger.Info(mail + "账户GetUserSendSMTPMailCount不为空发送邮件数量：" + sendCount.ToString() + "新增加数量为：" + Convert.ToInt32(o.ToString()));
                    }
                }
                ICollection<PSObject> result2 = ExchangePSProvider.GetUserSendExSMTPMailCount.ExecuteCmdlet(starTime, endTime, mail);
                foreach (PSObject o in result2)
                {
                    if (o == null)
                    {
                        sendCount += 0;
                        //DBUtility.Logger.Info(mail + "账户GetUserSendExSMTPMailCount发送邮件数量：" + sendCount.ToString());
                    }
                    else
                    {
                        sendCount = sendCount + Convert.ToInt32(o.ToString());
                        //DBUtility.Logger.Info(mail + "账户GetUserSendSMTPMailCount不为空发送邮件数量：" + sendCount.ToString() + "新增加数量为：" + Convert.ToInt32(o.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return false;
            }

            return true;
        }

        public static bool TestCommand(string cmd,out string strError)
        {
            strError = string.Empty;
            try
            {
               // ICollection<PSObject> result = ExchangePSProvider.PSCommandTest.ExecuteCmdlet(cmd);
               // Log4netHelper.Info("TestCommand result :" + result.Count);
            }
            catch (Exception ex)
            {
                strError += string.Format("<TestCommand>{0}</TestCommand>", ex.ToString());
                return false;
            }
            return true;
        }
    }

    public enum enDomainALStatus
    {
        all,
        al,
        gal,
        none
    }
}
