using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EntityToolsTests.Web
{
    public class Person
    {
        [DataMember]
        [Key]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
