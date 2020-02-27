using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class MeetingInfo
    {
        private string _Sender = string.Empty;
        public string Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }

        private List<string> _Attendees = new List<string>();
        public List<string> Attendees
        {
            get { return _Attendees; }
            set { _Attendees = value; }
        }

        private string _Subject = string.Empty;
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }

        private string _Body = string.Empty;
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        private DateTime _Start = DateTime.Now;
        public DateTime Start
        {
            get { return _Start; }
            set { _Start = value; }
        }

        private DateTime _End = DateTime.Now;
        public DateTime End
        {
            get { return _End; }
            set { _End = value; }
        }

        private string _Location = string.Empty;
        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public ErrorCodeInfo SendCheckProp()
        {
            ErrorCodeInfo error = new ErrorCodeInfo();
            if (String.IsNullOrEmpty(_Sender))
            {
                error.Code = ErrorCode.SenderEmpty;
            }
            if (string.IsNullOrEmpty(_Subject))
            {
                error.Code = ErrorCode.SubjectEmpty;
            }
            if (string.IsNullOrEmpty(_Location))
            {
                error.Code = ErrorCode.LocationEmpty;
            }
            if (_Attendees.Count <= 0)
            {
                error.Code = ErrorCode.AttendeesEmpty;
            }
            if (_End < _Start)
            {
                error.Code = ErrorCode.EndTimeLessStart;
            }
            return error;
        }
    }
}
