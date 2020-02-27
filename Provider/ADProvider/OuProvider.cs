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
    public class OuProvider
    {
        public bool AddOu(Guid transactionid, AdminInfo admin, ref OuInfo ou, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||ParentId:{ou.parentid}";

            DirectoryEntry OuParentEntry = new DirectoryEntry();
            DirectoryEntry OuEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.parentid, out OuParentEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }
                    OuEntry = OuParentEntry.Children.Add(string.Format("OU = {0}", ou.name), "organizationalUnit");
                    OuEntry.Properties["name"].Value = ou.name;

                    OuEntry.CommitChanges();
                    if (string.IsNullOrEmpty(ou.description.Trim()))
                    {
                        OuEntry.Properties["description"].Clear();
                    }
                    else
                    {
                        OuEntry.Properties["description"].Value = ou.description.Trim();
                    }

                    OuEntry.Properties["st"].Value = ou.IsProfessionalGroups.ToString();

                    OuParentEntry.CommitChanges();
                    OuParentEntry.Close();
                    OuEntry.CommitChanges();
                    ou.id = OuEntry.Guid;
                    ou.distinguishedName = Convert.ToString(OuEntry.Properties["distinguishedName"].Value);
                    OuEntry.Close();
                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("OuProvider调用AddOu异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuParentEntry != null)
                {
                    OuParentEntry.Close();
                }

                if (OuEntry != null)
                {
                    OuEntry.Close();
                }
            }
            return bResult;
        }

        public bool ModifyOu(Guid transactionid, AdminInfo admin, ref OuInfo ou, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Name:{ou.name}";
            paramstr += $"||Description:{ou.description}";
            paramstr += $"||Id:{ou.id}";

            DirectoryEntry OuEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out OuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    OuEntry.Rename(string.Format("OU = {0}", ou.name));
                    if (string.IsNullOrEmpty(ou.description.Trim()))
                    {
                        OuEntry.Properties["description"].Clear();
                    }
                    else
                    {
                        OuEntry.Properties["description"].Value = ou.description.Trim();
                    }

                    OuEntry.Properties["st"].Value = ou.IsProfessionalGroups.ToString();
                    OuEntry.CommitChanges();

                    ou.distinguishedName = Convert.ToString(OuEntry.Properties["distinguishedName"].Value);

                    OuEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("OuProvider调用ModifyOu异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }
            }
            return bResult;
        }

        public bool MoveOU()
        {
            bool result = true;
            return result;
        }

        public bool DeleteOu(Guid transactionid, AdminInfo admin, OuInfo ou, out ErrorCodeInfo error)
        {
            bool bResult = true;
            error = new ErrorCodeInfo();
            string strError = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Id:{ou.id}";

            DirectoryEntry OuEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.id, out OuEntry, out strError))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        bResult = false;
                        break;
                    }

                    // OuEntry.Parent.Children.Remove(OuEntry);
                    OuEntry.DeleteTree();
                    OuEntry.CommitChanges();
                    OuEntry.Close();

                } while (false);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("OuProvider调用DeleteOU异常", paramstr, ex.ToString(), transactionid);
                error.Code = ErrorCode.Exception;
                bResult = false;
            }
            finally
            {
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }
            }
            return bResult;
        }

        public bool AddRecycleOu(Guid transactionid, AdminInfo admin, OuInfo ou, out ErrorCodeInfo error)
        {
            bool result = false;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||Id:{ou.id}";
            paramstr += $"||distinguishedName:{ou.distinguishedName}";
            DirectoryEntry ouParentEntry = new DirectoryEntry();
            DirectoryEntry ouRecycleEntry = new DirectoryEntry();
            DirectoryEntry OuEntry = new DirectoryEntry();
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByGuid(ou.parentid, out ouParentEntry, out message))
                    {
                        error.Code = ErrorCode.SearchADDataError;
                        LoggerHelper.Error("OuManager调用AddRecycleOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    string recycleoupath = ConfigADProvider.GetADRecycleOuLdapByLdap(Convert.ToString(ouParentEntry.Properties["distinguishedName"].Value));
                    if (!commonProvider.GetADEntryByPath(recycleoupath, out ouRecycleEntry, out message))
                    {
                        result = true;
                        break;
                    }

                    DirectoryEntry newOuEntry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleOuEntry(ouRecycleEntry.Path, ou.name, out newOuEntry, out message))
                    {
                        result = true;
                        break;
                    }

                    OuEntry = ouRecycleEntry.Children.Add(string.Format("OU = {0}", ou.name), "organizationalUnit");
                    OuEntry.Properties["name"].Value = ou.name;
                    ouRecycleEntry.CommitChanges();
                    ouRecycleEntry.Close();
                    OuEntry.CommitChanges();
                    OuEntry.Close();
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("OuManager调用AddRecycleOu异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            finally
            {
                if (ouParentEntry != null)
                {
                    ouParentEntry.Close();
                }
                if (ouRecycleEntry != null)
                {
                    ouRecycleEntry.Close();
                }
                if (OuEntry != null)
                {
                    OuEntry.Close();
                }
            }
            return result;
        }

        public bool ModifyRecycleOu(Guid transactionid, AdminInfo admin, string recycleOuPath, string newOuName, out ErrorCodeInfo error)
        {
            bool result = false;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||recycleOuPath:{recycleOuPath}";
            paramstr += $"||newOuName:{newOuName}";
            DirectoryEntry ouRecycleEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByPath(recycleOuPath, out ouRecycleEntry, out message))
                    {
                        break;
                    }

                    DirectoryEntry newOuEntry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleOuEntry(ouRecycleEntry.Parent.Path, newOuName, out newOuEntry, out message))
                    {
                        break;
                    }

                    ouRecycleEntry.Rename(string.Format("OU = {0}", newOuName));
                    ouRecycleEntry.Close();
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("OuProvider调用ModifyRecycleOu异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            finally
            {
                if (ouRecycleEntry != null)
                {
                    ouRecycleEntry.Close();
                }
            }
            return result;
        }

        public bool MoveRecycleOu(Guid transactionid, AdminInfo admin, string recycleOuPath, string targetRecycleOuPath, out ErrorCodeInfo error)
        {
            bool result = false;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||recycleOuPath:{recycleOuPath}";
            paramstr += $"||targetRecycleOuPath:{targetRecycleOuPath}";

            DirectoryEntry ouRecycleEntry = new DirectoryEntry();
            DirectoryEntry targetOuRecycleEntry = new DirectoryEntry();

            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByPath(recycleOuPath, out ouRecycleEntry, out message))
                    {
                        LoggerHelper.Error("MoveRecycleOu调用GetADEntryByPath异常", paramstr, message, transactionid);
                        break;
                    }

                    if (!commonProvider.GetADEntryByPath(targetRecycleOuPath, out targetOuRecycleEntry, out message))
                    {
                        LoggerHelper.Error("MoveRecycleOu调用GetADEntryByPath异常", paramstr, message, transactionid);
                        break;
                    }


                    DirectoryEntry newOuEntry = new DirectoryEntry();
                    if (commonProvider.GetOneLevelSigleOuEntry(targetOuRecycleEntry.Path, Convert.ToString(ouRecycleEntry.Properties["name"].Value), out newOuEntry, out message))
                    {
                        LoggerHelper.Error("MoveRecycleOu调用GetOneLevelSigleOuEntry异常", paramstr, message, transactionid);
                        break;
                    }

                    ouRecycleEntry.MoveTo(targetOuRecycleEntry);
                    ouRecycleEntry.CommitChanges();
                    ouRecycleEntry.Close();
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("MoveRecycleOu异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            finally
            {
                if (ouRecycleEntry != null)
                {
                    ouRecycleEntry.Close();
                }
            }
            return result;
        }

        public bool DeleteRecycleOu(Guid transactionid, AdminInfo admin, string recycleOuPath, out ErrorCodeInfo error)
        {
            bool result = false;
            error = new ErrorCodeInfo();
            string message = string.Empty;
            string paramstr = string.Empty;
            paramstr += $"userID:{admin.UserID}";
            paramstr += $"||UserAccount:{admin.UserAccount}";
            paramstr += $"||recycleOuPath:{recycleOuPath}";

            DirectoryEntry ouRecycleEntry = new DirectoryEntry();
            
            try
            {
                do
                {
                    CommonProvider commonProvider = new CommonProvider();
                    if (!commonProvider.GetADEntryByPath(recycleOuPath, out ouRecycleEntry, out message))
                    {
                        LoggerHelper.Error("OuProvider调用DeleteRecycleOu异常", paramstr, message, transactionid);
                        break;
                    }

                    List<DirectoryEntry> EntryList = new List<DirectoryEntry>();
                    if (!commonProvider.SearchEntryData(ouRecycleEntry.Path, SearchType.All, out EntryList, out message))
                    {
                        LoggerHelper.Error("OuProvider调用DeleteRecycleOu异常", paramstr, message, transactionid);
                        result = false;
                        break;
                    }

                    if (EntryList.Count > 0)
                    {
                        break;
                    }
                    else
                    {
                        ouRecycleEntry.DeleteTree();
                        ouRecycleEntry.CommitChanges();
                        ouRecycleEntry.Close();
                    }
                    
                    result = true;

                } while (false);
            }
            catch (Exception ex)
            {
                error.Code = ErrorCode.Exception;
                LoggerHelper.Error("OuProvider调用DeleteRecycleOu异常", paramstr, ex.ToString(), transactionid);
                result = false;
            }
            finally
            {
                if (ouRecycleEntry != null)
                {
                    ouRecycleEntry.Close();
                }
            }
            return result;
        }
    }
}
