﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Group.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupFlagsChanged)]
    public class ServerGroupFlagsChanged : IWritable
    {
        public ulong GroupId { get; set; }
        public GroupFlags Flags { get; set; }
        public ulong Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Flags);
            writer.Write(Unknown1);
        }
    }
}