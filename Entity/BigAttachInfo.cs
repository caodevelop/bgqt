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


    public class UploadParItemInfo
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
        public Guid ID { get; set; } = Guid.Empty;
        public string FileName { get; set; } = string.Empty;
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
    }



    public class Rootobject
    {
        public object error { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string FileFullName { get; set; }
        public string HashCode { get; set; }
        public string ExtensionName { get; set; }
        public float FileSize { get; set; }
        public string DisplayName { get; set; }
        public string UserID { get; set; }
        public string AuthorName { get; set; }
        public int FileStatus { get; set; }
        public string FolderID { get; set; }
        public string StoreID { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

}
