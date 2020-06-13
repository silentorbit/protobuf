using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    class ProtoService : ProtoType, IComment
    {
        public override Wire WireType => Wire.Varint;

        public string Comments { get; set; }
        public Dictionary<string, RpcMethod> Methods
            = new Dictionary<string, RpcMethod>();

        public ProtoService(ProtoMessage parent, string package)
            : base(parent, package)
        {
        }

        public override string ToString()
            => "service " + this.FullProtoName;
    }

    class RpcMethod : IComment
    {
        public string Comments { get; set; }

        /// <summary>
        /// Method name as read from the .proto file
        /// </summary>
        public string ProtoName { get; set; }

        /// <summary>
        /// Request type as read from the .proto file
        /// </summary>
        public string RequestTypeName { get; set; }

        /// <summary>
        /// Response name read from the .proto file
        /// </summary>
        public string ResponseTypeName { get; set; }

        public ProtoType RequestProtoType { get; set; }
        public ProtoType ResponseProtoType { get; set; }
        public string CsName { get; internal set; }
        public readonly SourcePath Source;

        public RpcMethod(TokenReader tr)
        {
            Source = new SourcePath(tr);
        }
    }
}
