using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Models.Enums;

namespace TitanBot2.Models
{
    public class HelperSkill
    {
        public int SkillId { get; }
        public int HelperId { get; }
        public string Name { get; }
        public BonusType BonusType { get; }
        public double Magnitude { get; }
        public int RequiredLevel { get; }
        public string FileVersion { get; }

        public HelperSkill(int skillId, int helperId, string name, BonusType bonusType, double magnitude, int requiredLevel, string fileVersion)
        {
            SkillId = skillId;
            HelperId = helperId;
            Name = name;
            BonusType = bonusType;
            Magnitude = magnitude;
            RequiredLevel = requiredLevel;
            FileVersion = fileVersion;
        }
    }
}
