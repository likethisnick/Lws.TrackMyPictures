using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace Lws.TrackMyPictures
{
    class DateTimeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            ListViewItemsData a = (ListViewItemsData)x;
            ListViewItemsData b = (ListViewItemsData)y;
            DateTime dt1 = Convert.ToDateTime(a.GridViewDate);
            DateTime dt2 = Convert.ToDateTime(b.GridViewDate);
            int result = DateTime.Compare(dt1, dt2);
      
            if (result < 0)
                return 1;
            else if (result == 0)
                return 0;
            else
                return -1;
        }
    }
}
