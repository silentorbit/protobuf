using SilentOrbit.Code;

namespace SilentOrbit.ProtocolBuffers
{
    static class ServiceWriter
    {
        internal static void GenerateRpcNames(ProtoService s, CodeWriter cw)
        {
            cw.Bracket($"public static class {s.CsType}RpcNames");

            foreach (var m in s.Methods.Values)
            {
                cw.WriteLine($"public const string {m.ProtoName} = \"{s.CsType}_{m.ProtoName}\";");
            }

            cw.EndBracket();
        }

        internal static void GenerateInterface(ProtoService s, CodeWriter cw)
        {
            cw.Summary(s.Comments);
            cw.Bracket($"public interface I{s.CsType}Supporter");

            foreach (var m in s.Methods.Values)
            {
                cw.Summary(m.Comments);
                cw.WriteLine($"{m.ResponseProtoType.FullProtoName} {m.ProtoName}({m.RequestProtoType.FullProtoName} request);");
            }

            cw.EndBracket();
        }
    }
}
