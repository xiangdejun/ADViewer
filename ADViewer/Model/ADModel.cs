using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADViewer
{
    public class ADModel : IComparable
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public int CompareTo(object obj)
        {
            return this.Property.CompareTo(((ADModel)obj).Property);
        }
    }

}
