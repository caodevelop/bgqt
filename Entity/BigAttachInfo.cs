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
        public AttachSettingItem data { get; set; }
    }

    public class AttachSettingItem
    {
        public double MaxFileSize { get; set; }
        public int MaxUploads { get; set; }
        public string FileUploadBlackList { get; set; }
        public int ChunkSize { get; set; }
        public bool AllowDrop { get; set; }
        public string DefaultExtension { get; set; }
    }

}
