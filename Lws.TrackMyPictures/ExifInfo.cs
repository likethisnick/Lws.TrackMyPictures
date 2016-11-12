using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lws.TrackMyPictures
{
    public class ExifInfo
    {
        public string FilePath;
        public DateTime photoTime;
        public string photoPlace;
        public string Extension;
        public string FileSize;
        public int width;
        public int heigh;
        public double Lat;
        public double Long;
        public string CameraInfo;
        public string CameraInfoAdd;
        public string Comment;

        public ExifInfo()
        {
            FilePath = "";
        }

    }

    
}
