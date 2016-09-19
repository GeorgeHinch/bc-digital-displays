﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BC_Digital_Displays.Classes
{
    [DataContract]
    class bcEquipment
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public DateTime createdAt { get; set; }

        [DataMember]
        public DateTime updatedAt { get; set; }

        [DataMember]
        public bool deleted { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public int studio { get; set; }
    }
}
