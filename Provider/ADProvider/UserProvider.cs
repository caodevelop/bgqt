using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.ADProvider
{
    public class UserProvider
    {
        public bool AddUser(Guid transactionid, AdminInfo admin, ref UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||SAMAccountName:{user.SAMAccountName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";
            paramstr += $"||UserPassword:{user.Password}";
            paramstr += $"||ParentOuId:{user.ParentOuId}";

            DirectoryEntry OuEntry = new DirectoryEntry();
            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.ParentOuId, out OuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    user.ParentOu = Convert.ToString(OuEntry.Properties["distinguishedName"].Value);
                    UserEntry = OuEntry.Children.Add(string.Format("CN = {0}", user.DisplayName), "user");
                    UserEntry.Properties["userPrincipalName"].Value = user.UserAccount;
                    UserEntry.Properties["displayname"].Value = user.DisplayName;
                    UserEntry.Properties["givenname"].Value = user.FirstName;
                    UserEntry.Properties["sn"].Value = user.LastName;
                    UserEntry.Properties["sAMAccountName"].Value = user.SAMAccountName;
                    UserEntry.CommitChanges();

                    UserEntry.Properties["userAccountControl"].Value = Convert.ToInt32(UserEntry.Properties["userAccountControl"].Value) & ~2;
                    UserEntry.CommitChanges();

                    string pwd = user.Password;
                    UserEntry.Invoke("SetPassword", new object[] { pwd });
                    UserEntry.CommitChanges();

                    UserEntry.InvokeSet("pwdLastSet", (user.NextLoginChangePassword == true ? 0 : -1));
                    UserEntry.CommitChanges();

                    user.UserID = UserEntry.Guid;
                    UserEntry.Close();
                    OuEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用AddUser异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }

                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }

        public bool ChangeUser(Guid transactionid, AdminInfo admin, UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserDisplayName:{user.DisplayName}";
            paramstr += $"||UserFirstName:{user.FirstName}";
            paramstr += $"||UserLastName:{user.LastName}";

            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out UserEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    if (Convert.ToString(UserEntry.Properties["CN"].Value) != user.DisplayName)
                    {
                        DirectoryEntry entry = new DirectoryEntry();
                        if (commonProvider.GetOneLevelSigleEntryByCN(UserEntry.Parent.Path, user.DisplayName, out entry, out strError))
                        {
                            error.Code = ErrorCode.HaveSameDisplayName;
                            bResult = false;
                            break;
                        }

                        UserEntry.Rename(string.Format("CN = {0}", user.DisplayName));
                        UserEntry.Properties["displayName"].Value = user.DisplayName;
                    }

                    //选填项
                    UserEntry.Properties["sn"].Value = string.IsNullOrEmpty(user.LastName.Trim()) ? null : user.LastName.Trim();
                    UserEntry.Properties["givenName"].Value = string.IsNullOrEmpty(user.FirstName.Trim()) ? null : user.FirstName.Trim();
                    UserEntry.Properties["telephonenumber"].Value = string.IsNullOrEmpty(user.Phone.Trim()) ? null : user.Phone.Trim();
                    UserEntry.Properties["physicaldeliveryofficename"].Value = string.IsNullOrEmpty(user.Office.Trim()) ? null : user.Office.Trim();
                    UserEntry.Properties["description"].Value = string.IsNullOrEmpty(user.Description.Trim()) ? null : user.Description.Trim();
                    UserEntry.Properties["mobile"].Value = string.IsNullOrEmpty(user.Mobile.Trim()) ? null : user.Mobile.Trim();
                    UserEntry.Properties["mailNickname"].Value = string.IsNullOrEmpty(user.AliasName.Trim()) ? null : user.AliasName.Trim();
                    UserEntry.Properties["company"].Value = string.IsNullOrEmpty(user.Company.Trim()) ? null : user.Company.Trim();
                    UserEntry.Properties["department"].Value = string.IsNullOrEmpty(user.Department.Trim()) ? null : user.Department.Trim();
                    UserEntry.Properties["title"].Value = string.IsNullOrEmpty(user.Post.Trim()) ? null : user.Post.Trim();

                    UserEntry.CommitChanges();
                    UserEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用ChangeUser异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }
        
        public bool GetLoginUser(Guid transactionid, string userAccount, string password, out AdminInfo info, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            info = new AdminInfo();

            string paramstr = string.Empty;
            paramstr += "useraccount:" + userAccount;
            paramstr += "||password:" + password;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(ConfigADProvider.GetRootADPath(), userAccount, password);
                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = string.Format("(&(objectClass=user)(|(sAMAccountName={0})(userPrincipalName={0})))", userAccount);
                //  search.Filter = "(&(objectClass=user)(sAMAccountName=" + userAccount + "))";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    DirectoryEntry user = result.GetDirectoryEntry();
                    info.UserID = user.Guid;
                    info.DisplayName = Convert.ToString(user.Properties["displayName"].Value);
                    info.UserAccount = Convert.ToString(user.Properties["userPrincipalName"].Value);
                    if (!UserProvider.IsAccountActive(Convert.ToInt32(user.Properties["userAccountControl"][0])))
                    {
                        error.Code = ErrorCode.LoginUserError;
                        bResult = false;
                    }
                }
                else
                {
                    error.Code = ErrorCode.UserNotExist;
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用GetLoginUser异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.LoginUserError;
                bResult = false;
            }
            return bResult;
        }

        public bool GetUserByAccountPassword(Guid transactionid, string userAccount, string oldPassword, out UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            user = new UserInfo();
            string paramstr = string.Empty;
            paramstr += "useraccount:" + userAccount;
            paramstr += "||oldPassword:" + oldPassword;
           
            try
            {
                DirectoryEntry entry = new DirectoryEntry(ConfigADProvider.GetRootADPath(), userAccount, oldPassword);
                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = string.Format("(&(objectClass=user)(|(sAMAccountName={0})(userPrincipalName={0})))", userAccount);
                //  search.Filter = "(&(objectClass=user)(sAMAccountName=" + userAccount + "))";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    user.UserID = userEntry.Guid;
                    user.UserAccount = userEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(userEntry.Properties["userPrincipalName"].Value);
                    user.LastName = userEntry.Properties["sn"].Value == null ? "" : Convert.ToString(userEntry.Properties["sn"].Value);
                    user.FirstName = userEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(userEntry.Properties["givenName"].Value);
                    user.Phone = userEntry.Properties["telephoneNumber"].Value == null ? "" : Convert.ToString(userEntry.Properties["telephoneNumber"].Value);
                    user.Office = userEntry.Properties["physicalDeliveryOfficeName"].Value == null ? "" : Convert.ToString(userEntry.Properties["physicalDeliveryOfficeName"].Value);
                    user.Description = userEntry.Properties["description"].Value == null ? "" : Convert.ToString(userEntry.Properties["description"].Value);
                    user.Mobile = userEntry.Properties["mobile"].Value == null ? "" : Convert.ToString(userEntry.Properties["mobile"].Value);
                    user.Company = userEntry.Properties["company"].Value == null ? "" : Convert.ToString(userEntry.Properties["company"].Value);
                    user.Department = userEntry.Properties["department"].Value == null ? "" : Convert.ToString(userEntry.Properties["department"].Value);
                    user.Post = userEntry.Properties["title"].Value == null ? "" : Convert.ToString(userEntry.Properties["title"].Value);
                    user.DisplayName = userEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(userEntry.Properties["displayName"].Value);
                    user.ParentOu = userEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(userEntry.Parent.Properties["name"].Value);

                    int userAccountControl = Convert.ToInt32(userEntry.Properties["userAccountControl"][0]);
                    user.UserStatus = UserProvider.IsAccountActive(userAccountControl);
                }
                else
                {
                    error.Code = ErrorCode.UserNotExist;
                    bResult = false;
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用GetUserByAccountPassword异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.LoginUserError;
                bResult = false;
            }
            return bResult;
        }

        public bool ChangeUserPassword(Guid transactionid, UserInfo user, string oldPassword, string newPassword, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;

            string paramstr = string.Empty;
            paramstr += "userID:" + user.UserID;
            paramstr += "||oldPassword:" + oldPassword;
            paramstr += "||newPassword:" + newPassword;
            try
            {

                DirectoryEntry entry = new DirectoryEntry();
                CommonProvider commonProvider = new CommonProvider();
                if (!commonProvider.GetADEntryByGuid(user.UserID, out entry, out message))
                {
                    error.Code = ErrorCode.UserNotExist;
                    LoggerHelper.Error("UserProvider调用ChangeUserPassword异常", paramstr, message, transactionid);
                    bResult = false;
                }

                entry.Invoke("SetPassword", new object[] { newPassword });
                entry.CommitChanges();
                entry.Close();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message.Contains("密码不满足密码策略的要求"))
                {
                    error.Code = ErrorCode.PasswordNotStrong;
                }
                else
                {
                    error.Code = ErrorCode.Exception;
                }
                LoggerHelper.Error("UserProvider调用ChangeUserPassword异常", paramstr, ex.ToString(), transactionid);
                bResult = false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("密码不满足密码策略的要求"))
                {
                    error.Code = ErrorCode.PasswordNotStrong;
                }
                else
                {
                    error.Code = ErrorCode.Exception;
                }
                LoggerHelper.Error("UserProvider调用ChangeUserPassword异常", paramstr, ex.ToString(), transactionid);
                bResult = false;
            }
            return bResult;
        }

        public bool ResetUserPassword(Guid transactionid, AdminInfo admin, ref UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||Password:{user.Password}";
            paramstr += $"||NextLoginChangePassword=:{user.NextLoginChangePassword}";
            paramstr += $"||PasswordNeverExpire:{user.PasswordNeverExpire}";

            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out UserEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    user.UserAccount = Convert.ToString(UserEntry.Properties["userPrincipalName"].Value);
                    user.DisplayName = Convert.ToString(UserEntry.Properties["name"].Value);

                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        UserEntry.Invoke("SetPassword", new object[] { user.Password });
                        UserEntry.CommitChanges();
                    }

                    if (user.NextLoginChangePassword)
                    {
                        //下次登录需要更改密码
                        UserEntry.Properties["pwdLastSet"][0] = 0;
                    }
                    else
                    {
                        UserEntry.Properties["pwdLastSet"][0] = -1;
                    }

                    //密码永不过期
                    if (user.PasswordNeverExpire)
                    {
                        UserEntry.Properties["userAccountControl"].Value = Convert.ToInt32(UserEntry.Properties["userAccountControl"].Value) | 65536;
                    }
                    else
                    {
                        UserEntry.Properties["userAccountControl"].Value = Convert.ToInt32(UserEntry.Properties["userAccountControl"].Value) & ~65536;
                    }

                    UserEntry.CommitChanges();
                    UserEntry.Close();

                } while (false);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                error.Code = ErrorCode.PasswordNotStrong;
                LoggerHelper.Error("UserProvider调用ResetUserPassword异常", paramstr, ex.ToString(), transactionid);
                bResult = false;
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.PasswordNotStrong;
                LoggerHelper.Error("UserProvider调用ResetUserPassword异常", paramstr, ex.ToString(), transactionid);
                bResult = false;
            }
            finally
            {
                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }

        public bool ChangeUserStatus(Guid transactionid, AdminInfo admin, UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"AdminID:{admin.UserID}";
            paramstr += $"||AdminAccount:{admin.UserAccount}";
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||UserAccount:{user.UserAccount}";
            paramstr += $"||UserStatus:{user.UserStatus}";
           
            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out UserEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    int Temp = Convert.ToInt32(UserEntry.Properties["userAccountControl"][0]);
                    //启用/禁用
                    if (user.UserStatus)
                    {
                        //启用
                        UserEntry.Properties["userAccountControl"][0] = Temp & ~2;
                    }
                    else
                    {
                        //禁用
                        UserEntry.Properties["userAccountControl"][0] = Temp | 2;
                    }

                    UserEntry.CommitChanges();
                    UserEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用ChangeUserStatus异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }

        public bool ClearMemberof(Guid transactionid, UserInfo user, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();

                    if (!commonProvider.GetADEntryByGuid(user.UserID, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("ClearMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //从该对象原隶属组成员中剔除该对象
                    bool bTempResult = true;
                    DirectoryEntry GroupEntryTemp = new DirectoryEntry();
                    for (int i = 0; i < entry.Properties["memberof"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(entry.Properties["memberof"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out GroupEntryTemp, out message))
                        {
                            LoggerHelper.Error("ClearMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            bTempResult = false;
                            break;
                        }

                        GroupEntryTemp.Properties["member"].Remove(Convert.ToString(entry.Properties["distinguishedName"].Value));
                        GroupEntryTemp.CommitChanges();
                        GroupEntryTemp.Close();

                        #region 不能删除hab组
                        //string strIsHabGroup = GroupEntryTemp.Properties["msOrg-IsOrganizational"].Value == null ? "" : Convert.ToString(GroupEntryTemp.Properties["msOrg-IsOrganizational"].Value).ToUpper();
                        //if (strIsHabGroup.Trim() != "TRUE")
                        //{
                        //    GroupEntryTemp.Properties["member"].Remove(Convert.ToString(entry.Properties["distinguishedName"].Value));
                        //    GroupEntryTemp.CommitChanges();
                        //    GroupEntryTemp.Close();
                        //}
                        #endregion
                    }

                    if (!bTempResult)
                    {
                        result = false;
                        break;
                    }

                    entry.CommitChanges();
                    entry.Close();
                } while (false);
            }
            catch (Exception ex)
            {
                result = false;
                LoggerHelper.Error("ClearMemberof异常", paramstr, ex.ToString(), transactionid);
            }

            return result;
        }

        public bool ClearAllMemberof(Guid transactionid, UserInfo user, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();

                    if (!commonProvider.GetADEntryByGuid(user.UserID, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("ClearMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    //从该对象原隶属组成员中剔除该对象
                    DirectoryEntry GroupEntryTemp = new DirectoryEntry();
                    for (int i = 0; i < entry.Properties["memberof"].Count; i++)
                    {
                        string MemberDnName = Convert.ToString(entry.Properties["memberof"][i]);
                        if (!commonProvider.GetADEntryByPath(MemberDnName, out GroupEntryTemp, out message))
                        {
                            LoggerHelper.Error("ClearMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                            continue;
                        }

                        GroupEntryTemp.Properties["member"].Remove(Convert.ToString(entry.Properties["distinguishedName"].Value));
                        GroupEntryTemp.CommitChanges();
                        GroupEntryTemp.Close();
                    }

                    entry.CommitChanges();
                    entry.Close();
                } while (false);
            }
            catch (Exception ex)
            {
                result = false;
                LoggerHelper.Error("ClearMemberof异常", paramstr, ex.ToString(), transactionid);
            }

            return result;
        }

        public bool AddMemberof(Guid transactionid, UserInfo user, GroupInfo group,out string groupname, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            groupname = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";
            paramstr += $"||GroupID:{group.GroupID}";

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("AddMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    DirectoryEntry groupEntry = new DirectoryEntry();
                    if (!commonProvider.GetADEntryByGuid(group.GroupID, out groupEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("AddMemberof调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    groupname = groupEntry.Properties["name"].Value == null ? "" : Convert.ToString(groupEntry.Properties["name"].Value);

                    groupEntry.Properties["member"].Add(Convert.ToString(entry.Properties["distinguishedName"].Value));
                    groupEntry.CommitChanges();
                    groupEntry.Close();

                    entry.CommitChanges();
                    entry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                result = false;
                LoggerHelper.Error("AddMemberof异常", paramstr, ex.ToString(), transactionid);
            }

            return result;
        }

        public bool MoveAndDisableUser(Guid transactionid, UserInfo user, Guid targetOuId, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||targetOuId:{targetOuId}";
        
            DirectoryEntry userEntry = new DirectoryEntry();
            DirectoryEntry ouEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out userEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    user.DisplayName = Convert.ToString(userEntry.Properties["displayName"].Value);

                    if (!commonProvider.GetADEntryByGuid(targetOuId, out ouEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    //如果重名则改名
                    DirectoryEntry newUserEntry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(ouEntry.Path, user.DisplayName, out newUserEntry, out strError))
                    {
                        userEntry.Rename(string.Format("CN = {0}", user.DisplayName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                        userEntry.Properties["displayName"].Value = user.DisplayName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        userEntry.CommitChanges();
                    }

                    userEntry.MoveTo(ouEntry);
                    int Temp = Convert.ToInt32(userEntry.Properties["userAccountControl"][0]);
                    userEntry.Properties["userAccountControl"][0] = Temp | 2;
                    userEntry.CommitChanges();
                    userEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用MoveAndDisableUser异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (userEntry != null)
                {
                    userEntry.Close();
                }
                if (ouEntry != null)
                {
                    ouEntry.Close();
                }
            }
            return bResult;
        }

        public bool MoveAndEnableUser(Guid transactionid, UserInfo user, Guid targetOuId, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"||UserID:{user.UserID}";
            paramstr += $"||targetOuId:{targetOuId}";

            DirectoryEntry userEntry = new DirectoryEntry();
            DirectoryEntry targetOuEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(targetOuId, out targetOuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("UserManager调用MoveAndEnableUser异常", paramstr, strError, transactionid);
                        bResult = false;
                        break;
                    }

                    if (!commonProvider.GetADEntryByGuid(user.UserID, out userEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("UserManager调用MoveAndEnableUser异常", paramstr, strError, transactionid);
                        bResult = false;
                        break;
                    }

                    DirectoryEntry newUserEntry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleEntryByCN(targetOuEntry.Path, Convert.ToString(userEntry.Properties["name"].Value), out newUserEntry, out strError))
                    {
                        error.Code = ErrorCode.HaveSameDisplayName;
                        LoggerHelper.Error("UserManager调用MoveAndEnableUser异常", paramstr, strError, transactionid);
                        bResult = false;
                        break;
                    }

                    userEntry.MoveTo(targetOuEntry);
                    int Temp = Convert.ToInt32(userEntry.Properties["userAccountControl"][0]);
                    userEntry.Properties["userAccountControl"][0] = Temp & ~2;
                    userEntry.CommitChanges();
                    userEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用MoveAndEnableUser异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (userEntry != null)
                {
                    userEntry.Close();
                }
                if (targetOuEntry != null)
                {
                    targetOuEntry.Close();
                }
            }
            return bResult;
        }

        public bool GetUserInfo(Guid transactionid, ref UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";
           
            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    DirectoryEntry userEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(user.UserID, out userEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }
                   
                    user.UserAccount = userEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(userEntry.Properties["userPrincipalName"].Value);
                    user.LastName = userEntry.Properties["sn"].Value == null ? "" : Convert.ToString(userEntry.Properties["sn"].Value);
                    user.FirstName = userEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(userEntry.Properties["givenName"].Value);
                    user.Phone = userEntry.Properties["telephoneNumber"].Value == null ? "" : Convert.ToString(userEntry.Properties["telephoneNumber"].Value);
                    user.Office = userEntry.Properties["physicalDeliveryOfficeName"].Value == null ? "" : Convert.ToString(userEntry.Properties["physicalDeliveryOfficeName"].Value);
                    user.Description = userEntry.Properties["description"].Value == null ? "" : Convert.ToString(userEntry.Properties["description"].Value);
                    user.Mobile = userEntry.Properties["mobile"].Value == null ? "" : Convert.ToString(userEntry.Properties["mobile"].Value);
                    user.Company = userEntry.Properties["company"].Value == null ? "" : Convert.ToString(userEntry.Properties["company"].Value);
                    user.Department = userEntry.Properties["department"].Value == null ? "" : Convert.ToString(userEntry.Properties["department"].Value);
                    user.Post = userEntry.Properties["title"].Value == null ? "" : Convert.ToString(userEntry.Properties["title"].Value);
                    user.DisplayName = userEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(userEntry.Properties["displayName"].Value);
                    user.ParentOuId = userEntry.Parent.Guid;
                    user.ParentOu = userEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(userEntry.Parent.Properties["name"].Value);
                    user.ParentDistinguishedName = userEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(userEntry.Parent.Properties["distinguishedName"].Value);
                    user.DistinguishedName = userEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(userEntry.Properties["distinguishedName"].Value);
                    user.SAMAccountName = userEntry.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(userEntry.Properties["sAMAccountName"].Value);
                    user.AliasName = userEntry.Properties["mailNickname"].Value == null ? "" : Convert.ToString(userEntry.Properties["mailNickname"].Value);

                    int userAccountControl = Convert.ToInt32(userEntry.Properties["userAccountControl"][0]);
                    user.UserStatus = UserProvider.IsAccountActive(userAccountControl);

                    DirectoryEntry groupEntry = new DirectoryEntry();
                    for (int i = 0; i < userEntry.Properties["memberof"].Count; i++)
                    {
                        string MemberofDnName = Convert.ToString(userEntry.Properties["memberof"][i]);

                        if (commonProvider.GetADEntryByPath(MemberofDnName, out groupEntry, out message))
                        {
                            GroupInfo group = new GroupInfo();
                            group.GroupID = groupEntry.Guid;
                            group.DisplayName = groupEntry.Properties["name"].Value == null ? "" : Convert.ToString(groupEntry.Properties["name"].Value);
                            group.Account = groupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(groupEntry.Properties["mail"].Value);
                            group.IsOrganizational = groupEntry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(groupEntry.Properties["msOrg-IsOrganizational"].Value));
                            user.BelongGroups.Add(group);
                        }
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用GetUserInfo异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }

        public bool DeleteUser(Guid transactionid, UserInfo user, out ErrorCodeInfo error)
        {
            bool result = true;
            string message = string.Empty;
            error = new ErrorCodeInfo();
            string paramstr = string.Empty;
            paramstr += $"UserID:{user.UserID}";

            try
            {
                do
                {
                    DirectoryEntry entry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();

                    if (!commonProvider.GetADEntryByGuid(user.UserID, out entry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("DeleteUser调用GetADEntryByGuid异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }
                    entry.Parent.Children.Remove(entry);
                    entry.CommitChanges();
                    entry.Close();
                } while (false);
            }
            catch (Exception ex)
            {
                result = false;
                LoggerHelper.Error("DeleteUser异常", paramstr, ex.ToString(), transactionid);
            }

            return result;
        }

        public bool GetMailUsersByOu(Guid transactionid, Guid ouID, out List<UserInfo> users, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strError = string.Empty;
            users = new List<UserInfo>();
            string paramstr = string.Empty;
            paramstr += $"ouID:{ouID}";

            DirectoryEntry ouEntry = new DirectoryEntry();
            DirectoryEntry item = new DirectoryEntry();

            try
            {
                do
                {
                    DirectoryEntry userEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ouID, out ouEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    DirectoryEntry de = null;
                    de = new DirectoryEntry(ouEntry.Path);
                    DirectorySearcher deSearch = new DirectorySearcher(de);
                    deSearch.SearchRoot = de;
                    string strFilter = commonProvider.GetSearchType(SearchType.MailUser, string.Empty);
                    deSearch.Filter = strFilter;
                    deSearch.SearchScope = SearchScope.Subtree;
                    deSearch.SizeLimit = 20000;
                    deSearch.ServerTimeLimit = TimeSpan.FromSeconds(600);
                    deSearch.ClientTimeout = TimeSpan.FromSeconds(600);
                    SearchResultCollection results = deSearch.FindAll();

                    if (results != null && results.Count > 0)
                    {
                        foreach (SearchResult Result in results)
                        {
                            item = Result.GetDirectoryEntry();
                            UserInfo user = new UserInfo();
                            user.UserID = item.Guid;
                            user.UserAccount = item.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(item.Properties["userPrincipalName"].Value);
                            user.SAMAccountName = item.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(item.Properties["sAMAccountName"].Value);
                            users.Add(user);
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("GetMailUsersByOu异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (ouEntry != null)
                {
                    ouEntry.Close();
                }
                if (item != null)
                {
                    item.Close();
                }
            }
            return bResult;
        }

        public bool GetUserInfoByEMPLID(Guid transactionid, string EMPLID, out UserInfo user, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"EMPLID:{EMPLID}";
            user = new UserInfo();

            DirectoryEntry UserEntry = new DirectoryEntry();

            try
            {
                do
                {
                    DirectoryEntry userEntry = new DirectoryEntry();
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetEntryDataBysAMAccount(ConfigADProvider.GetCompanyADRootPath(), EMPLID, out userEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    user.UserID = userEntry.Guid;
                    user.UserAccount = userEntry.Properties["userPrincipalName"].Value == null ? "" : Convert.ToString(userEntry.Properties["userPrincipalName"].Value);
                    user.LastName = userEntry.Properties["sn"].Value == null ? "" : Convert.ToString(userEntry.Properties["sn"].Value);
                    user.FirstName = userEntry.Properties["givenName"].Value == null ? "" : Convert.ToString(userEntry.Properties["givenName"].Value);
                    user.Phone = userEntry.Properties["telephoneNumber"].Value == null ? "" : Convert.ToString(userEntry.Properties["telephoneNumber"].Value);
                    user.Office = userEntry.Properties["physicalDeliveryOfficeName"].Value == null ? "" : Convert.ToString(userEntry.Properties["physicalDeliveryOfficeName"].Value);
                    user.Description = userEntry.Properties["description"].Value == null ? "" : Convert.ToString(userEntry.Properties["description"].Value);
                    user.Mobile = userEntry.Properties["mobile"].Value == null ? "" : Convert.ToString(userEntry.Properties["mobile"].Value);
                    user.Company = userEntry.Properties["company"].Value == null ? "" : Convert.ToString(userEntry.Properties["company"].Value);
                    user.Department = userEntry.Properties["department"].Value == null ? "" : Convert.ToString(userEntry.Properties["department"].Value);
                    user.Post = userEntry.Properties["title"].Value == null ? "" : Convert.ToString(userEntry.Properties["title"].Value);
                    user.DisplayName = userEntry.Properties["displayName"].Value == null ? "" : Convert.ToString(userEntry.Properties["displayName"].Value);
                    user.ParentOuId = userEntry.Parent.Guid;
                    user.ParentOu = userEntry.Parent.Properties["name"].Value == null ? "" : Convert.ToString(userEntry.Parent.Properties["name"].Value);
                    user.ParentDistinguishedName = userEntry.Parent.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(userEntry.Parent.Properties["distinguishedName"].Value);
                    user.DistinguishedName = userEntry.Properties["distinguishedName"].Value == null ? "" : Convert.ToString(userEntry.Properties["distinguishedName"].Value);
                    user.SAMAccountName = userEntry.Properties["sAMAccountName"].Value == null ? "" : Convert.ToString(userEntry.Properties["sAMAccountName"].Value);
                    user.AliasName = userEntry.Properties["mailNickname"].Value == null ? "" : Convert.ToString(userEntry.Properties["mailNickname"].Value);

                    int userAccountControl = Convert.ToInt32(userEntry.Properties["userAccountControl"][0]);
                    user.UserStatus = UserProvider.IsAccountActive(userAccountControl);
                    if (!user.UserStatus)
                    {
                        LoggerHelper.Error("UserProvider调用GetUserInfoByEMPLID异常", paramstr, "该账户已被停用", transactionid);
                        error.Code = ErrorCode.UserIsDisable;
                        bResult = false;
                        break;
                    }

                    DirectoryEntry groupEntry = new DirectoryEntry();
                    for (int i = 0; i < userEntry.Properties["memberof"].Count; i++)
                    {
                        string MemberofDnName = Convert.ToString(userEntry.Properties["memberof"][i]);

                        if (commonProvider.GetADEntryByPath(MemberofDnName, out groupEntry, out message))
                        {
                            GroupInfo group = new GroupInfo();
                            group.GroupID = groupEntry.Guid;
                            group.DisplayName = groupEntry.Properties["name"].Value == null ? "" : Convert.ToString(groupEntry.Properties["name"].Value);
                            group.Account = groupEntry.Properties["mail"].Value == null ? "" : Convert.ToString(groupEntry.Properties["mail"].Value);
                            group.IsOrganizational = groupEntry.Properties["msOrg-IsOrganizational"].Value == null ? false : Convert.ToBoolean(Convert.ToInt32(groupEntry.Properties["msOrg-IsOrganizational"].Value));
                            user.BelongGroups.Add(group);
                        }
                    }

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UserProvider调用GetUserInfoByEMPLID异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (UserEntry != null)
                {
                    UserEntry.Close();
                }
            }
            return bResult;
        }

        /// <summary>
        ///判断用户帐号是否激活
        ///用户帐号属性控制器
        ///如果用户帐号已经激活，返回true；否则返回false
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public static bool IsAccountActive(int userAccountControl)
        {
            int userAccountControl_Disabled = Convert.ToInt32(ADS_USER_FLAG_ENUM.ADS_UF_ACCOUNTDISABLE);
            int flagExists = userAccountControl & userAccountControl_Disabled;

            if (flagExists > 0)
                return false;
            else
                return true;
        }
    }
}
 