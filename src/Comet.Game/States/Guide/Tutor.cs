// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Mentor.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States.Syndicates;

namespace Comet.Game.States.Guide
{
    public sealed class Tutor
    {
        private DbTutor m_tutor;
        private DbTutorContributions m_access;

        private Tutor() { }
        
        public static async Task<Tutor> CreateAsync(DbTutor tutor)
        {
            var guide = new Tutor
            {
                m_tutor = tutor,
                m_access = await DbTutorContributions.GetGuideAsync(tutor.StudentId)
            };
            guide.m_access ??= new DbTutorContributions
            {
                TutorIdentity = tutor.GuideId,
                StudentIdentity = tutor.StudentId
            };

            DbCharacter dbMentor = await CharactersRepository.FindByIdentityAsync(tutor.GuideId);
            if (dbMentor == null)
                return null;
            guide.GuideName = dbMentor.Name;

            dbMentor = await CharactersRepository.FindByIdentityAsync(tutor.StudentId);
            if (dbMentor == null)
                return null;
            guide.StudentName = dbMentor.Name;
            return guide;
        }

        public uint GuideIdentity => m_tutor.GuideId;
        public string GuideName { get; private set; }

        public uint StudentIdentity => m_tutor.StudentId;
        public string StudentName { get; private set; }

        public Character Guide => Kernel.RoleManager.GetUser(m_tutor.GuideId);
        public Character Student => Kernel.RoleManager.GetUser(m_tutor.StudentId);

        public async Task<bool> AwardTutorExperienceAsync(uint addExpTime)
        {
            m_access.Experience += addExpTime;

            Character user = Kernel.RoleManager.GetUser(m_access.TutorIdentity);
            if (user != null)
            {
                user.MentorExpTime += addExpTime;
            }
            else
            {
                DbTutorAccess tutorAccess = await DbTutorAccess.GetAsync(m_access.TutorIdentity);
                tutorAccess ??= new DbTutorAccess
                {
                    GuideIdentity = GuideIdentity
                };
                tutorAccess.Experience += addExpTime;
                await BaseRepository.SaveAsync(tutorAccess);
            }
            return await SaveAsync();
        }

        public async Task<bool> AwardTutorGodTimeAsync(ushort addGodTime)
        {
            m_access.GodTime += addGodTime;

            Character user = Kernel.RoleManager.GetUser(m_access.TutorIdentity);
            if (user != null)
            {
                user.MentorGodTime += addGodTime;
            }
            else
            {
                DbTutorAccess tutorAccess = await DbTutorAccess.GetAsync(m_access.TutorIdentity);
                tutorAccess ??= new DbTutorAccess
                {
                    GuideIdentity = GuideIdentity
                };
                tutorAccess.Blessing += addGodTime;
                await BaseRepository.SaveAsync(tutorAccess);
            }
            return await SaveAsync();
        }

        public async Task<bool> AwardOpportunityAsync(ushort addTime)
        {
            m_access.PlusStone += addTime;

            Character user = Kernel.RoleManager.GetUser(m_access.TutorIdentity);
            if (user != null)
            {
                user.MentorAddLevexp += addTime;
            }
            else
            {
                DbTutorAccess tutorAccess = await DbTutorAccess.GetAsync(m_access.TutorIdentity);
                tutorAccess ??= new DbTutorAccess
                {
                    GuideIdentity = GuideIdentity
                };
                tutorAccess.Composition += addTime;
                await BaseRepository.SaveAsync(tutorAccess);
            }
            return await SaveAsync();
        }

        public int SharedBattlePower
        {
            get
            {
                Character mentor = Guide;
                Character student = Student;
                if (mentor == null || student == null)
                    return 0;
                if (mentor.PureBattlePower < student.PureBattlePower)
                    return 0;

                var limit = Kernel.RoleManager.GetTutorBattleLimitType(student.PureBattlePower);
                if (limit == null)
                    return 0;

                var type = Kernel.RoleManager.GetTutorType(mentor.Level);
                if (type == null)
                    return 0;
                
                return (int) Math.Min(limit.BattleLevelLimit, (mentor.PureBattlePower - student.PureBattlePower) * (type.BattleLevelShare / 100f));
            }
        }

        public Task BetrayAsync()
        {
            m_tutor.BetrayalFlag = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            return SaveAsync();
        }

        public Task SendAsync(MsgGuideInfo.RequestMode mode)
        {
            Character tutor = mode == MsgGuideInfo.RequestMode.Mentor ? Guide : Student;
            Character target = mode == MsgGuideInfo.RequestMode.Mentor ? Student : Guide;
            return target.SendAsync(new MsgGuideInfo
            {
                Identity = target.Identity,
                Level = tutor?.Level ?? 0,
                Blessing = m_access.GodTime,
                Composition = (ushort) m_access.PlusStone,
                Experience = m_access.Experience,
                IsOnline = tutor != null,
                Mesh = tutor?.Mesh ?? 0,
                Mode = mode,
                Syndicate = tutor?.SyndicateIdentity ?? 0,
                SyndicatePosition = tutor?.SyndicateRank ?? SyndicateMember.SyndicateRank.None,
                Names = new List<string>
                {
                    mode == MsgGuideInfo.RequestMode.Mentor ? GuideName : StudentName,
                    target.Name,
                    tutor?.MateName ?? Language.StrNone
                },
                EnroleDate = uint.Parse(m_tutor.Date?.ToString("yyyyMMdd") ?? "0"),
                PkPoints = tutor?.PkPoints ?? 0,
                Profession = tutor?.Profession ?? 0,
                SharedBattlePower = (uint) (mode == MsgGuideInfo.RequestMode.Mentor ? SharedBattlePower : 0),
                SenderIdentity = m_tutor.GuideId,
                Unknown24 = 999999
            });
        }

        public async Task<bool> SaveAsync()
        {
            return await BaseRepository.SaveAsync(m_tutor) && await BaseRepository.SaveAsync(m_access);
        }

        public async Task<bool> DeleteAsync()
        {
            await BaseRepository.DeleteAsync(m_tutor);
            await BaseRepository.DeleteAsync(m_access);
            return true;
        }
    }
}