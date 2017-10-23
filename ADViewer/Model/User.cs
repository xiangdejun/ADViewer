using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADViewer
{
    public class User
    {
        private List<ADModel> _properties;
        public List<ADModel> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new List<ADModel>();

                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        private List<string> _groups;
        public List<string> Groups
        {
            get
            {
                if (_groups == null)
                    _groups = new List<string>();

                return _groups;
            }
            set
            {
                _groups = value;
            }
        }
    }
}
