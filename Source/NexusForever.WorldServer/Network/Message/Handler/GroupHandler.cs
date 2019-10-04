﻿using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Group;
using NexusForever.WorldServer.Game.Group.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GroupHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientGroupInvite)]
        public static void HandleGroupInvite(WorldSession session, ClientGroupInvite request)
        {
            session.EnqueueEvent(new TaskGenericEvent<Character>(CharacterDatabase.GetCharacterByName(request.PlayerName),
                character =>
            {
                if (character == null)
                {
                    session.EnqueueMessageEncrypted(new ServerGroupInviteResult
                    {
                        GroupId = 0,
                        PlayerName = request.PlayerName,
                        Result = InviteResult.PlayerNotFound
                    });
                    return;
                }

                var group = GroupManager.CreateNewGroup();
                group.PartyLeaderCharacterId = session.Player.CharacterId;
                group.CreateNewInvite(character.Id);
                var member = group.CreateNewMember(session.Player.CharacterId);

                session.EnqueueMessageEncrypted(new ServerGroupInviteResult
                {
                    GroupId = group.GroupId,
                    PlayerName = request.PlayerName,
                    Result = InviteResult.Sent
                });

                WorldSession targetSession = NetworkManager<WorldSession>.GetSession(s => s.Player?.CharacterId == character.Id);
                if (targetSession != null)
                {
                    targetSession.EnqueueMessageEncrypted(new ServerGroupInviteReceived
                    {
                        GroupId = group.GroupId,
                        Unknown0 = 0,
                        Unknown1 = 0,
                        GroupMembers = new List<GroupMember>
                        {
                            new GroupMember
                            {
                                Name = session.Player.Name,
                                Faction = session.Player.Faction1,
                                Race = session.Player.Race,
                                Class = session.Player.Class,
                                Path = session.Player.Path,
                                Level = (byte)session.Player.Level,
                                GroupMemberId = (ushort)member.Id
                            }
                        }
                    });
                }
            }));
        }

        [MessageHandler(GameMessageOpcode.ClientGroupInviteResponse)]
        public static void HandleGroupInviteResponse(WorldSession session, ClientGroupInviteResponse clientGroupInviteResponse)
        {
            log.Info($"{clientGroupInviteResponse.GroupId}, {clientGroupInviteResponse.Response}, {clientGroupInviteResponse.Unknown0}");

            var group = GroupManager.GetGroupById(clientGroupInviteResponse.GroupId);
            var invite = group.FindInvite(session.Player.CharacterId);

            if (group == null || invite == null)
                return;

            WorldSession targetSession = NetworkManager<WorldSession>.GetSession(s => s.Player?.CharacterId == group.PartyLeaderCharacterId);
            
            // Declined
            if (clientGroupInviteResponse.Response == InviteResponseResult.Declined)
            {
                targetSession.EnqueueMessageEncrypted(new ServerGroupInviteResult
                {
                    GroupId = clientGroupInviteResponse.GroupId,
                    PlayerName = session.Player.Name,
                    Result = InviteResult.Declined
                });

                group.DismissInvite(invite);
                if (group.IsEmpty)
                    GroupManager.DismissGroup(group);

                return;
            }

            // Accepted
            var member = group.AcceptInvite(invite);
            ServerGroupJoin join = new ServerGroupJoin
            {
                PlayerJoined = new TargetPlayerIdentity
                {
                    RealmId = WorldServer.RealmId,
                    CharacterId = session.Player.Guid
                },
                GroupId = group.GroupId,
                Unknown0 = 257,
                MaxSize = 5,
                LootRuleNormal = 1,
                LootRuleThreshold = 2,
                LootThreshold = 3,
                LootRuleHarvest = 0,
                GroupMembers = new List<ServerGroupJoin.GroupMemberInfo>
                {
                    new ServerGroupJoin.GroupMemberInfo
                    {
                        MemberIdentity = new TargetPlayerIdentity
                        {
                            RealmId = WorldServer.RealmId,
                            CharacterId = session.Player.Guid
                        },
                        Flags = 8198,
                        GroupMember = new GroupMember
                        {
                            Name = session.Player.Name,
                            Faction = session.Player.Faction1,
                            Race = session.Player.Race,
                            Class = session.Player.Class,
                            Path = session.Player.Path,
                            Level = (byte)session.Player.Level,
                            GroupMemberId = (ushort)member.Id,
                            Realm = WorldServer.RealmId,
                            WorldZoneId = (ushort)targetSession.Player.Zone.Id,
                            Unknown25 = 2725,
                            Unknown26 = 1,
                            Unknown27 = true
                        },
                        GroupIndex = 1
                    },
                    new ServerGroupJoin.GroupMemberInfo
                    {
                        MemberIdentity = new TargetPlayerIdentity
                        {
                            RealmId = WorldServer.RealmId,
                            CharacterId = targetSession.Player.Guid
                        },
                        Flags = 8192,
                        GroupMember = new GroupMember
                        {
                            Name = targetSession.Player.Name,
                            Faction = targetSession.Player.Faction1,
                            Race = targetSession.Player.Race,
                            Class = targetSession.Player.Class,
                            Path = targetSession.Player.Path,
                            Level = (byte)targetSession.Player.Level,
                            GroupMemberId = (ushort)group.PartyLeader.Id,
                            Realm = WorldServer.RealmId,
                            WorldZoneId = (ushort)session.Player.Zone.Id,
                            Unknown25 = 2725,
                            Unknown26 = 1,
                            Unknown27 = true
                        },
                        GroupIndex = 2
                    },
                },
                LeaderIdentity = new TargetPlayerIdentity
                {
                    RealmId = WorldServer.RealmId,
                    CharacterId = session.Player.Guid
                },
                Realm = WorldServer.RealmId
            };

            session.EnqueueMessageEncrypted(join);

            targetSession.EnqueueMessageEncrypted(new ServerGroupInviteResult
            {
                GroupId = group.GroupId,
                PlayerName = session.Player.Name,
                Result = InviteResult.Accepted
            });

            targetSession.EnqueueMessageEncrypted(join);
        }
    }
}
