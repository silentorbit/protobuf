using System;
using ProtoBuf;
using System.Collections.Generic;
using Personal;
using System.ComponentModel;

namespace Test
{
    [ProtoContract]
    class NetPerson
    {
        [ProtoMember(1)]
        public string
            Name;

        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public string Email { get; set; }

        [ProtoMember(4)]
        public List<NetPhoneNumber> Phone { get; set; }
        
        [ProtoContract]
        public class NetPhoneNumber
        {
            [ProtoMember(1)]
            public string Number { get; set; }

            [ProtoMember(2)]
            [DefaultValue(Person.PhoneType.HOME)]
            public Person.PhoneType Type { get; set; }
        }
    }
}

