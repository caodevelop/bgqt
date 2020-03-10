using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class ErrorCodeInfo
    {
        private ErrorCode _Code = ErrorCode.None;
        private String codeInfo = String.Empty;
        public ErrorCode Code
        {
            get
            {
                return _Code;
            }
            set
            {
                codeInfo = string.Empty;
                _Code = value;
            }
        }
        public String Info
        {
            get
            {
                if (codeInfo == string.Empty)
                {
                    switch (_Code)
                    {
                        case ErrorCode.None:
                            codeInfo = "执行成功";
                            break;
                        case ErrorCode.PageSizeIllegal:
                            codeInfo = "每页项数不合法!";
                            break;
                        case ErrorCode.PageSizeEmpty:
                            codeInfo = "每页项数为空!";
                            break;
                        case ErrorCode.CurPageEmpty:
                            codeInfo = "当前页数为空!";
                            break;
                        case ErrorCode.CurPageIllegal:
                            codeInfo = "当前页数不合法!";
                            break;
                        case ErrorCode.SearchNameEmpty:
                            codeInfo = "查询信息为空!";
                            break;
                        case ErrorCode.IPPeriodForbidden:
                            codeInfo = "IP不允许";
                            break;
                        case ErrorCode.JsonRequestEmpty:
                            codeInfo = "Json参数为空";
                            break;
                        case ErrorCode.TokenEmpty:
                            codeInfo = "Token为空";
                            break;
                        case ErrorCode.TokenIllegal:
                            codeInfo = "Token不合法";
                            break;
                        case ErrorCode.TokenExpire:
                            codeInfo = "Token过期";
                            break;
                        case ErrorCode.JsonRequestIllegal:
                            codeInfo = "Json参数不合法";
                            break;
                        case ErrorCode.Exception:
                            codeInfo = "系统异常，请联系超级管理员";
                            break;
                        case ErrorCode.SQLException:
                            codeInfo = "数据库异常，请联系超级管理员";
                            break;
                        case ErrorCode.WebApiException:
                            codeInfo = "WebApi异常";
                            break;
                        case ErrorCode.AppAccessTokenEmpty:
                            codeInfo = "AppToken为空";
                            break;
                        case ErrorCode.AppAccessTokenIllegal:
                            codeInfo = "AppToken不合法";
                            break;
                        case ErrorCode.AppOpEmpty:
                            codeInfo = "App操作为空";
                            break;
                        case ErrorCode.AppOpIllegal:
                            codeInfo = "App操作不合法";
                            break;
                        case ErrorCode.AppIDEmpty:
                            codeInfo = "AppID为空";
                            break;
                        case ErrorCode.SecretEmpty:
                            codeInfo = "Secret密钥为空";
                            break;
                        case ErrorCode.AppIDIllegal:
                            codeInfo = "AppID不合法";
                            break;
                        case ErrorCode.AppTokenEmpty:
                            codeInfo = "AppToken为空";
                            break;
                        case ErrorCode.AppTokenIllegal:
                            codeInfo = "AppToken不合法";
                            break;
                        case ErrorCode.AppReqDataEmpty:
                            codeInfo = "App返回值为空";
                            break;
                        case ErrorCode.AppReqDataIllegal:
                            codeInfo = "App返回值不合法";
                            break;
                        case ErrorCode.AppReqEnDataIllegal:
                            codeInfo = "App返回值不合法";
                            break;
                        case ErrorCode.AppIDNotExist:
                            codeInfo = "AppID不存在";
                            break;
                        case ErrorCode.AppException:
                            codeInfo = "AppID异常";
                            break;
                        case ErrorCode.ValKeyEmpty:
                            codeInfo = "AppID异常";
                            break;
                        case ErrorCode.UserNotExist:
                            codeInfo = "用户不存在";
                            break;
                        case ErrorCode.ParentIdEmpty:
                            codeInfo = "父级节点为空";
                            break;
                        case ErrorCode.NameEmpty:
                            codeInfo = "名称为空";
                            break;
                        case ErrorCode.NameIllegal:
                            codeInfo = "OU 名称不合法，应以英文+数字为前缀";
                            break;
                        case ErrorCode.NamePrefixEmpty:
                            codeInfo = "OU 名称不合法，应以英文+数字为前缀";
                            break;
                        case ErrorCode.ADSecurityError:
                            codeInfo = "AD 身份验证错误";
                            break;
                        case ErrorCode.SearchADDataError:
                            codeInfo = "查询对象不存在";
                            break;
                        case ErrorCode.IdEmpty:
                            codeInfo = "ID 为空";
                            break;
                        case ErrorCode.HaveSameName:
                            codeInfo = "存在相同名称";
                            break;
                        case ErrorCode.HaveMembers:
                            codeInfo = "该 OU 下存在成员，不可删除。";
                            break;
                        case ErrorCode.AccountEmpty:
                            codeInfo = "账户为空";
                            break;
                        case ErrorCode.IdNotExist:
                            codeInfo = "ID 不存在";
                            break;
                        case ErrorCode.MailboxDBEmpty:
                            codeInfo = "MailboxDB 为空";
                            break;
                        case ErrorCode.HaveSameOu:
                            codeInfo = "已存在同名 OU";
                            break;
                        case ErrorCode.ObjectIDEmpty:
                            codeInfo = "对象 ID 为空";
                            break;
                        case ErrorCode.KeywordsEmpty:
                            codeInfo = "敏感关键词为空";
                            break;
                        case ErrorCode.HaveSameGroup:
                            codeInfo = "存在相同通讯组";
                            break;
                        case ErrorCode.HaveSameRule:
                            codeInfo = "存在相同规则";
                            break;
                        case ErrorCode.RoleNotExist:
                            codeInfo = "角色不存在";
                            break;
                        case ErrorCode.UserHaveRole:
                            codeInfo = "用户已具有角色";
                            break;
                        case ErrorCode.UserNotRole:
                            codeInfo = "用户未具有角色";
                            break;
                        case ErrorCode.IsDefaultRole:
                            codeInfo = "默认角色不能删除";
                            break;
                        case ErrorCode.MustHaveMember:
                            codeInfo = "必须存在成员";
                            break;
                        case ErrorCode.LoginUserError:
                            codeInfo = "账号或密码错误";
                            break;
                        case ErrorCode.MustHaveSameLevelOuPath:
                            codeInfo = "允许根 OU 同级 OU 不能为空。";
                            break;
                        case ErrorCode.UserNotLoginRole:
                            codeInfo = "用户没有登录权限。";
                            break;
                        case ErrorCode.UserNotEnoughRole:
                            codeInfo = "管理员没有足够的操作权限。";
                            break;
                        case ErrorCode.SourceOuContainsTargetOu:
                            codeInfo = "目标 OU 为源 OU 的子 OU，无法移动。";
                            break;
                        case ErrorCode.HaveSameObject:
                            codeInfo = "存在相同对象。";
                            break;
                        case ErrorCode.ControlOUPathNotEmpty:
                            codeInfo = "必须指定管理范围。";
                            break;
                        case ErrorCode.SameOuPath:
                            codeInfo = "源路径与目标路径相同。";
                            break;
                        case ErrorCode.ObjectsEmpty:
                            codeInfo = "必须指定对象。";
                            break;
                        case ErrorCode.ComplianceSearchOnlyScope:
                            codeInfo = "查询大于等于10000用户数据时，请选择单一范围。";
                            break;
                        case ErrorCode.OuHaveMailBoxDB:
                            codeInfo = "该 OU 已指定邮箱数据库。";
                            break;
                        case ErrorCode.MaxAccountLength:
                            codeInfo = "用户账号长度不能超过 20 个字符。";
                            break;
                        case ErrorCode.HaveSameDisplayName:
                            codeInfo = "当前节点下已存在相同显示名称。";
                            break;
                        case ErrorCode.AccountIllegal:
                            codeInfo = "用户账号不合法";
                            break;
                        case ErrorCode.RootOuNotOperate:
                            codeInfo = " OU 根节点不能被修改或删除。";
                            break;
                        case ErrorCode.ParentDnEmpty:
                            codeInfo = "父级节点为空。";
                            break;
                        case ErrorCode.ParentNotExist:
                            codeInfo = "父级节点不存在。";
                            break;
                        case ErrorCode.PasswordIllegal:
                            codeInfo = "密码支持 8~20 个字符，包含字母、数字、标点符号；不支持空格，' & '，' < '";
                            break;
                        case ErrorCode.SensitiveMailIsExecuting:
                            codeInfo = "删除敏感邮件命令已在执行中，请在执行完成后进行编辑或删除。";
                            break;
                        case ErrorCode.SensitiveMailExecute:
                            codeInfo = "删除敏感邮件命令已在队列或执行中，不能重复执行。";
                            break;
                        case ErrorCode.NotHaveData:
                            codeInfo = "暂无导出数据。命令尚未执行或数据已被清理。";
                            break;
                        case ErrorCode.PasswordNotStrong:
                            codeInfo = "密码不满足密码策略的要求。检查最小密码长度、密码复杂性和密码历史的要求。 ";
                            break;
                        case ErrorCode.SenderEmpty:
                            codeInfo = "发件人不能为空。";
                            break;
                        case ErrorCode.SubjectEmpty:
                            codeInfo = "主题不能为空。";
                            break;
                        case ErrorCode.LocationEmpty:
                            codeInfo = "地点不能为空。";
                            break;
                        case ErrorCode.AttendeesEmpty:
                            codeInfo = "参与人不能为空。";
                            break;
                        case ErrorCode.EndTimeLessStart:
                            codeInfo = "开始时间不能大于结束时间。";
                            break;
                        case ErrorCode.SNKeyExpire:
                            codeInfo = "系统异常，请联系超级管理员。";
                            break;
                        case ErrorCode.FileNotExist:
                            codeInfo = "文件不存在。";
                            break;

                            
                        default:
                            break;
                    }
                }
                return codeInfo;
            }
        }

        public void SetInfo(params string[] infos)
        {
            switch (_Code)
            {
                case ErrorCode.HaveParentOu:
                    codeInfo = $"已选择 {infos[0]} 的父级 OU";
                    break;
                case ErrorCode.UserNotExchange:
                    codeInfo = $"用户 {infos[0]} 未开通邮箱。";
                    break;
                case ErrorCode.MoveNodeNotExist:
                    codeInfo = $"移动对象 {infos[0]} 不存在。";
                    break;
                case ErrorCode.MoveNodehaveSameAccount:
                    codeInfo = $"目标节点下已存在相同显示名称的用户 {infos[0]} ";
                    break;
                case ErrorCode.HaveSameNamePrefixAccount:
                    codeInfo = $"{infos[0]} 中已存在相同前缀的账号。";
                    break;
                case ErrorCode.HaveSameNamePrefixOu:
                    codeInfo = $"{infos[0]} 中已存在相同前缀的 OU 。";
                    break;
                case ErrorCode.HaveSameAccount:
                    codeInfo = $"{ infos[0]} 中已存在相同账号。";
                    break;
                case ErrorCode.RootHaveSameDisplayName:
                    codeInfo = $"{ infos[0]} 中已存在相同显示名称。";
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    public enum ErrorCode
    {
        None = 0,
        PageSizeIllegal = 1001,
        PageSizeEmpty = 1002,
        CurPageEmpty = 1003,
        CurPageIllegal = 1004,

        // bigattach
        FileNotExist = 2001,

        SearchNameEmpty = 1005,
        IPPeriodForbidden = 7001,
        JsonRequestEmpty = 8001,
        TokenEmpty = 8002,
        TokenIllegal = 8003,
        TokenExpire = 8004,
        JsonRequestIllegal = 8005,
        SNKeyExpire = 8006,
        Exception = 9999,
        SQLException = 9998,
        WebApiException = 9997,
        AppAccessTokenEmpty = 20001,
        AppAccessTokenIllegal = 20002,
        AppOpEmpty = 20003,
        AppOpIllegal = 20004,
        AppIDEmpty = 20005,
        AppIDIllegal = 20006,
        AppTokenEmpty = 20007,
        AppTokenIllegal = 20008,
        AppReqDataEmpty = 20009,
        AppReqDataIllegal = 20010,
        AppReqEnDataIllegal = 20011,
        AppIDNotExist = 20012,
        AppException = 99999,
        SecretEmpty = 20013,

        ValKeyEmpty = 10000,
        UserNotExist = 10001,
        ParentIdEmpty = 10002,
        NameEmpty = 10003,
        NameIllegal = 10004,
        NamePrefixEmpty = 10005,
        ADSecurityError = 10006,
        SearchADDataError = 10007,
        IdEmpty = 10008,
        HaveSameName = 10009,
        HaveMembers = 10010,
        AccountEmpty = 10011,
        IdNotExist = 10012,
        MailboxDBEmpty = 10013,
        HaveSameOu = 10014,
        ObjectIDEmpty = 10015,
        KeywordsEmpty = 10016,
        HaveSameGroup = 10017,
        HaveSameRule = 10018,
        RoleNotExist = 10019,
        UserHaveRole = 10020,
        UserNotRole = 10021,
        IsDefaultRole = 10022,
        MustHaveMember = 10023,
        LoginUserError = 10024,
        MustHaveSameLevelOuPath = 10025,
        UserNotLoginRole = 10026,
        UserNotEnoughRole = 10027,
        SourceOuContainsTargetOu = 10028,
        HaveSameObject = 10029,
        ControlOUPathNotEmpty = 10030,
        HaveParentOu = 10031,
        UserNotExchange = 10032,
        SameOuPath = 10033,
        ObjectsEmpty = 10034,
        ComplianceSearchOnlyScope = 10035,
        OuHaveMailBoxDB = 10036,
        MaxAccountLength = 10037,

        HaveSameDisplayName = 10038,
        HaveSameAccount = 10039,
        AccountIllegal = 10040,
        GroupNotExchange = 10041,
        HaveSameNamePrefixAccount = 10042,
        HaveSameNamePrefixOu = 10043,
        RootOuNotOperate = 10044,
        RootHaveSameDisplayName = 10045,

        ParentDnEmpty = 10046,
        ParentNotExist = 10047,
        MoveNodeNotExist = 10048,
        PasswordIllegal = 10049,
        SensitiveMailIsExecuting = 10050,
        SensitiveMailExecute = 10051,
        NotHaveData = 10052,
        PasswordNotStrong = 10053,
        MoveNodehaveSameAccount = 10054,

        SenderEmpty =10055,
        SubjectEmpty = 10056,
        LocationEmpty = 10057,
        AttendeesEmpty = 10058,
        EndTimeLessStart = 10059,
    }
}
