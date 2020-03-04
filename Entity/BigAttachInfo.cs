using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    class BigAttachInfo
    {
    }

    public class AttachResultInfo
    {
        public string error { get; set; }
        public Object data { get; set; }
    }

    public class AttachSettingItem
    {
        public double MaxFileSize { get; set; } = 0.00;
        public int MaxUploads { get; set; } = 0;
        public string FileUploadBlackList { get; set; } = string.Empty;
        public int ChunkSize { get; set; } = 0;
        public bool AllowDrop { get; set; } = true;
        public string DefaultExtension { get; set; } = string.Empty;
    }


    public class GetUploadParItem
    {
        public string StorageID { get; set; } = string.Empty;
        public string StorageRelativePath { get; set; } = string.Empty;
        public string StorageUri { get; set; } = string.Empty;
        public double UserQuota { get; set; } = 0.00;
        public double UserUsedQuota { get; set; } = 0.00;
        public string OutlookFolderID { get; set; } = string.Empty;
    }


    public class BigFileListInfo
    {
        public List<BigFileItemInfo> files { get; set; }
        public int pageCount { get; set; } = 0;
        public int recordCount { get; set; } = 0;
    }

    public class BigFileItemInfo
    {
        public string ID { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ExtensionName { get; set; } = string.Empty;
        public double FileSize { get; set; } = 0.00;
        public string UserID { get; set; } = string.Empty;
        public string FolderID { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; } = DateTime.MinValue;
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public string DisplayFileSize { get; set; } = string.Empty;
    }



}
