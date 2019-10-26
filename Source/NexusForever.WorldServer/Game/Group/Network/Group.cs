﻿using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Group.Extensions;
using NexusForever.WorldServer.Game.Group.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

#nullable enable

namespace NexusForever.WorldServer.Game.Group
{
    public partial class Group
    {
        public delegate IWritable? BroadcastCallback(GroupMember member);

        /// <summary>
        /// Broadcast generated message to every member in the group
        /// </summary>
        /// <param name="callback">callback to generate message per every member</param>
        public void Broadcast(BroadcastCallback callback)
        {
            GetMembers().ForEach(member =>
            {
                var value = callback(member);
                if (value != null)
                    member.Send(value);
            });
        }

        /// <summary>
        /// Broadcast given message to the whole group
        /// </summary>
        /// <param name="message">message to broadcast</param>
        /// <param name="excluded">do not send message to excluded member</param>
        public void Broadcast(IWritable message, GroupMember? excluded = null)
        {
            membersLock.EnterReadLock();
            try
            {
                members.ForEach(m => {
                    if (m.Id == excluded?.Id)
                        return;
                    m.Send(message);
                });
            }
            finally
            {
                membersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Build Group Join packet for the given member
        /// </summary>
        /// <param name="member">new member who joined</param>
        public ServerGroupJoin BuildServerGroupJoin(GroupMember member)
        {
            uint groupIndex = 1;
            var groupMembers = new List<ServerGroupJoin.GroupMemberInfo>();
            membersLock.EnterReadLock();
            try
            {
                foreach (var groupMember in members)
                {
                    groupMembers.Add(groupMember.BuildGroupMemberInfo(groupIndex++));
                }
            }
            finally
            {
                membersLock.ExitReadLock();
            }

            return new ServerGroupJoin
            {
                JoinedPlayer = member.Player.BuildTargetPlayerIdentity(),
                GroupId = Id,
                GroupFlags = Flags,
                MaxSize = MaxSize,
                LootRuleNormal = LootRule.NeedBeforeGreed,          // Under LootThreshold rarity (For Raid)
                LootRuleThreshold = LootRule.RoundRobin,            // This is the selection for Loot Rules in the UI / Over LootTreshold rarity (For Raid)
                LootThreshold = LootThreshold.Excellent,
                LootRuleHarvest = LootRuleHarvest.FirstTagger,      // IDK were it shows this setting in the UI
                GroupMembers = groupMembers,
                LeaderIdentity = PartyLeader.Player.BuildTargetPlayerIdentity(),
                Realm = WorldServer.RealmId
            };
        }

        /// <summary>
        /// Build group join request packet
        /// </summary>
        public ServerGroupRequestJoin BuildServerGroupRequestJoin(GroupInvite invite)
        {
            return new ServerGroupRequestJoin
            {
                GroupId = Id,
                MemberInfo = invite.BuildGroupMemberInfo()
            };
        }

        /// <summary>
        /// Build Member Add packet for the given member
        /// </summary>
        public ServerGroupMemberAdd BuildServerGroupMemberAdd(GroupMember member)
        {
            membersLock.EnterReadLock();
            try
            {
                var groupIndex = (uint)members.IndexOf(member) + 1;
                var memberInfo = member.BuildGroupMemberInfo(groupIndex);
                return new ServerGroupMemberAdd
                {
                    GroupId = Id,
                    AddMemberInfo = memberInfo
                };
            }
            finally
            {
                membersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Build Group Leave packet with the given reason
        /// </summary>
        public ServerGroupLeave BuildServerGroupLeave(RemoveReason reason)
        {
            return new ServerGroupLeave
            {
                GroupId = Id,
                Reason = reason
            };
        }

        /// <summary>
        /// Build invite result
        /// </summary>
        public ServerGroupInviteResult BuildServerGroupInviteResult(string playerName, InviteResult result)
        {
            return new ServerGroupInviteResult
            {
                GroupId = Id,
                PlayerName = playerName,
                Result = result
            };
        }

        /// <summary>
        /// Build group reuqest to join result
        /// </summary>
        public ServerGroupRequestJoinResult BuildServerGroupRequestJoinResult(string playerName, InviteResult result)
        {
            return new ServerGroupRequestJoinResult
            {
                GroupId = Id,
                PlayerName = playerName,
                Result = result,
                IsJoin = true
            };
        }

        /// <summary>
        /// Build group invite response that is sent to the invitee
        /// </summary>
        public ServerGroupInviteReceived BuildServerGroupInviteReceived()
        {
            var groupMembers = new List<Member>();
            membersLock.EnterReadLock();
            try
            {
                members.ForEach(m => groupMembers.Add(m.BuildGroupMember()));
            }
            finally
            {
                membersLock.ExitReadLock();
            }
            return new ServerGroupInviteReceived
            {
                GroupId = Id,
                GroupMembers = groupMembers
            };
        }

        /// <summary>
        /// Build ready check packet
        /// </summary>
        public ServerGroupSendReadyCheck BuildServerGroupSendReadyCheck(GroupMember member, string message)
        {
            return new ServerGroupSendReadyCheck
            {
                GroupId = Id,
                SenderIdentity = member.Player.BuildTargetPlayerIdentity(),
                Message = message
            };
        }

        /// <summary>
        /// Build group flags changed packet
        /// </summary>
        public ServerGroupFlagsChanged BuildServerGroupFlagsChanged()
        {
            return new ServerGroupFlagsChanged
            {
                GroupId = Id,
                Flags = Flags
            };
        }

        /// <summary>
        /// Build group size change packet
        /// </summary>
        public ServerGroupMaxSizeChange BuildServerGroupMaxSizeChange()
        {
            return new ServerGroupMaxSizeChange
            {
                GroupId = Id,
                MaxSize = MaxSize,
                Flags = Flags
            };
        }
    }
}
