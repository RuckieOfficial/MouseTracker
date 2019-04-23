using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseTracker
{
    class Profile {
        public string Name { get; set; }
        public UInt32 MouseSensitivity { get; set; }
        public UInt32 ScrollSensitivity { get; set; }
        public UInt32 DoubleClickSensitivity { get; set; }

        public Profile(string Name, UInt32 MouseSensitivity, UInt32 ScrollSensitivity, UInt32 DoubleClickSensitivity)
        {
            this.Name = Name;
            this.MouseSensitivity = MouseSensitivity;
            this.ScrollSensitivity = ScrollSensitivity;
            this.DoubleClickSensitivity = DoubleClickSensitivity;
        }
    }
}
