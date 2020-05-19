using BaseComManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdpComManager
{
    public class UdpInitInfo : BaseInitInfo, IBaseInitInfo
    {
        public int Port         { get; set; }
        public string IpAddress { get; set; }
    }
}
