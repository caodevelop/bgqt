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
    public class AttatchUserInfo
    {
        public string currentEmailAddress { get; set; }
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


    public class UploadParItemInfo
    {
        public string StorageID { get; set; } = string.Empty;
        public string StorageRelativePath { get; set; } = string.Empty;
        public string StorageUri { get; set; } = string.Empty;
        public double UserQuota { get; set; } = 0.00;
        public double UserUsedQuota { get; set; } = 0.00;
        public string OutlookFolderID { get; set; } = string.Empty;
    }

    public class ShareSettingsInfo
    {
        public bool MustAuth { get; set; } = false;
        public bool AllowChangeAtuth { get; set; } = false;
        public bool AllowChangeExpireTime { get; set; } = false;
        public DateTime ExpireTime { get; set; } = DateTime.MinValue;
        public int FileShareLimit { get; set; } = 100;
        public string ShareNotificationTemplate { get; set; } = string.Empty;
        public bool AllowChangeValidateSetting { get; set; } = false;
        public bool ValidateCheckBoxChecked { get; set; } = false;
        public object DefaultValidateCode { get; set; } = "x3b6g9";
    }


    public class BigFileListInfo
    {
        public List<BigFileItemInfo> files { get; set; } = new List<BigFileItemInfo>();
        public int pageCount { get; set; } = 0;
        public int recordCount { get; set; } = 0;
    }

    public class BigFileItemInfo
    {
        public Guid ID { get; set; } = Guid.Empty;

        public Guid FileID { get; set; } = Guid.Empty;

        public Guid TempID { get; set; } = Guid.Empty;
        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileFullName { get; set; } = string.Empty;
        public string HashCode { get; set; } = string.Empty;
     
        
        public string ExtensionName { get; set; } = string.Empty;
        public double FileSize { get; set; } = 0.00;
        public string DisplayName { get; set; } = string.Empty;
        public Guid UserID { get; set; } = Guid.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int FileStatus { get; set; } = 1;
        public string FolderID { get; set; } = string.Empty;
        public string StoreID { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; } = DateTime.MinValue;
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public string DisplayFileSize
        {
            get
            {
                string str = string.Empty;
                int sizeKB = Convert.ToInt32(FileSize / 1024);
                if (sizeKB == 0)
                {
                    str = "1 KB";
                }
                else if (sizeKB < 1024)
                {
                    str = $"{sizeKB} KB";
                }
                else if (sizeKB >= 1024)
                {
                    int sizeMB = Convert.ToInt32(sizeKB / 1024);
                    if (sizeMB < 1024)
                    {
                        str = $"{sizeMB} MB";
                    }
                    else if (sizeMB >= 1024)
                    {
                        int sizeGB = Convert.ToInt32(sizeMB / 1024);
                        str = $"{sizeGB} GB";
                    }
                }

                return str;
            }
        }

        public bool Succeed { get; set; } = true;
    }

    public class UploadFileItemInfo : BigFileItemInfo
    {
        public long TotalChunks { get; set; } = 0;
        public long ChunkIndex { get; set; } = 0;
        public long FileSizeInt { get; set; } = 0;
        public long Position { get; set; } = 0;
        public byte[] Data { get; set; } 
    }

    public class ShareInfo
    {        
        public Guid ShareID { get; set; } = Guid.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime ExpireTime { get; set; } = DateTime.Now;
        public string ValCode { get; set; } = string.Empty;
    }

   

}
