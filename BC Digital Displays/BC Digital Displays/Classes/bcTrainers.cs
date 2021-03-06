﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BC_Digital_Displays.Classes
{
    [DataContract]
    class bcTrainers
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public DateTimeOffset createdAt { get; set; }

        [DataMember]
        public DateTimeOffset updatedAt { get; set; }

        [DataMember]
        public bool deleted { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string degree { get; set; }

        [DataMember]
        public double years { get; set; }

        [DataMember]
        public double yearsBC { get; set; }

        [DataMember]
        public string expertise { get; set; }

        [DataMember]
        public string reward { get; set; }

        [DataMember]
        public string expectation { get; set; }

        [DataMember]
        public string accomplishment { get; set; }

        [DataMember]
        public string photo { get; set; }

        [DataMember]
        public string reflections { get; set; }
    }
}
