using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orbis
{
    public class UserT : DataTransferObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }
}