using System;

namespace SilentOrbit.ProtocolBuffers
{
    static class Search
    {
        /// <summary>
        /// Search for message in hierarchy
        /// </summary>
        public static ProtoType GetProtoType(ProtoMessage msg, string path)
        {
            //Search for message or enum
            ProtoType pt;

            //Search from one level up until a match is found
            while (msg is ProtoCollection == false)
            {
                //Search sub messages
                pt = SearchSubMessages(msg, msg.Package + "." + msg.ProtoName + "." + path);
                if (pt != null)
                    return pt;

                //Search siblings
                pt = SearchSubMessages(msg.Parent, msg.Package + "." + path);
                if (pt != null)
                    return pt;

                msg = msg.Parent;
            }

            //Finally search for global namespace
            return SearchSubMessages(msg, path);
        }

        static ProtoType SearchSubMessages(ProtoMessage msg, string fullPath)
        {
            foreach (ProtoMessage sub in msg.Messages.Values)
            {
                if (fullPath == sub.FullProtoName)
                    return sub;

                if (fullPath.StartsWith(sub.FullProtoName + "."))
                {
                    ProtoType pt = SearchSubMessages(sub, fullPath);
                    if (pt != null)
                        return pt;
                }
            }

            foreach (ProtoEnum subEnum in msg.Enums.Values)
            {
                if (fullPath == subEnum.FullProtoName)
                    return subEnum;
            }

            return null;
        }
    }
}

