using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    [Serializable]
    public class WaterMakingInfo
    {
        private Guid _ID = Guid.Empty;
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public DateTime CreateTime
        { get; set; } = DateTime.Now;

        public string CreateTimeName
        { get { return CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        public WaterMakingContentInfo WaterMakingContent
        { get; set; } = new WaterMakingContentInfo();

        public string Description
        { get; set; } = string.Empty;

        public Guid CreateUserID
        { get; set; } = Guid.Empty;

        public Guid RoleID
        { get; set; } = Guid.Empty;
    }

    [Serializable]
    //PDF条件
    public class PDFConditionInfo
    {
        public string From
        { get; set; } = string.Empty;

        public string Subject
        { get; set; } = string.Empty;

        public string Recipients
        { get; set; } = string.Empty;

        public List<string> RecipientLists
        {
            get
            {
                List<string> _recipientLists = new List<string>();
                if (Recipients.Contains(";"))
                {
                    string[] arr = Recipients.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    _recipientLists = arr.ToList();
                }
                return _recipientLists;
            }
        }

        public string PDFName
        { get; set; } = string.Empty;

        public string PDFCondition
        {
            get
            {
                string condition = string.Empty;
                if (!string.IsNullOrEmpty(From))
                {
                    condition = "发件人：" + From + "，";
                }
                if (!string.IsNullOrEmpty(Recipients))
                {
                    condition += "收件人：" + Recipients + "，";
                }
                if (!string.IsNullOrEmpty(Subject))
                {
                    condition += "邮件主题：" + Subject + "，";
                }
                if (!string.IsNullOrEmpty(PDFName))
                {
                    condition += "PDF文件名：" + PDFName + "，";
                }
                return condition;
            }
        }
    }

    [Serializable]
    //正文条件
    public class BodyConditionInfo
    {
        public string From
        { get; set; } = string.Empty;

        public string Subject
        { get; set; } = string.Empty;

        public string Recipients
        { get; set; } = string.Empty;

        public List<string> RecipientLists
        {
            get
            {
                List<string> _recipientLists = new List<string>();
                if (Recipients.Contains(";"))
                {
                    string[] arr = Recipients.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    _recipientLists = arr.ToList();
                }
                return _recipientLists;
            }
        }

        public bool IsContainsAttachment
        { get; set; } = false;

        public string AttachmentName
        { get; set; } = string.Empty;

        public string BodyCondition
        {
            get
            {
                string condition = string.Empty;
                if (!string.IsNullOrEmpty(From))
                {
                    condition = "发件人：" + From + "，";
                }
                if (!string.IsNullOrEmpty(Recipients))
                {
                    condition += "收件人：" + Recipients + "，";
                }
                if (!string.IsNullOrEmpty(Subject))
                {
                    condition += "邮件主题：" + Subject + "，";
                }
                if (IsContainsAttachment)
                {
                    condition += "包含附件，";

                    if (!string.IsNullOrEmpty(AttachmentName))
                    {
                        condition += "附件名称：" + AttachmentName + "，";
                    }
                }
                return condition;
            }
        }
    }

    [Serializable]
    public class WaterMakingContentInfo
    {
        public bool IsAllRecipients
        { get; set; } = false;
        public string Content
        { get; set; } = string.Empty;

        public string WaterMakingContent
        {
            get
            {
                string _content = string.Empty;
                if (IsAllRecipients)
                {
                    _content = "全部收件人";
                }
                else
                {
                    _content = Content;
                }
                return _content;
            }
        }
    }

    [Serializable]
    public class PDFWaterMakingInfo : WaterMakingInfo
    {
        public PDFConditionInfo PDFCondition
        { get; set; } = new PDFConditionInfo();

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (string.IsNullOrEmpty(Name))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(PDFCondition.From) && string.IsNullOrEmpty(PDFCondition.Subject)
                && string.IsNullOrEmpty(PDFCondition.Recipients) && string.IsNullOrEmpty(PDFCondition.PDFName))
            {
                error.Code = ErrorCode.WaterMarkingConditionEmpty;
            }

            return error;
        }
    }

    [Serializable]
    public class BodyWaterMakingInfo : WaterMakingInfo
    {
        public BodyConditionInfo BodyCondition
        { get; set; } = new BodyConditionInfo();

        public ErrorCodeInfo AddCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (string.IsNullOrEmpty(Name))
            {
                error.Code = ErrorCode.NameEmpty;
            }
            else if (string.IsNullOrEmpty(BodyCondition.From) && string.IsNullOrEmpty(BodyCondition.Subject)
                && string.IsNullOrEmpty(BodyCondition.Recipients) && string.IsNullOrEmpty(BodyCondition.AttachmentName))
            {
                error.Code = ErrorCode.WaterMarkingConditionEmpty;
            }

            return error;
        }
    }
}
