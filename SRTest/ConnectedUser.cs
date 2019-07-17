using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRTest
{
    public class ConnectedUser
    {
        public string name { get; set; }
        public string connectionid { get; set; }
        public string groupname { get; set; }

        public ConnectedUser()
        {

        }

        public ConnectedUser(string username, string cid, string groupName)
        {
            connectionid = cid;
            name = username;
            groupname = groupName;
        }
    }
}
