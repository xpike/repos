using ProtoBuf;
using System;
using System.Runtime.Serialization;
using XPike.Contracts;

namespace Example.Library.Models
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class User
        : IModel
    {
        [DataMember]
        [ProtoMember(1)]
        public int UserId { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string Username { get; set; }
    }
}