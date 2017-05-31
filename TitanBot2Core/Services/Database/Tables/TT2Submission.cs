using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Tables
{
    public class TT2Submission
    {
        [BsonId]
        public int Id { get; set; }
        public ulong Submitter { get; set; }
        public ulong? Answerer { get; set; }
        public SubmissionType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Response { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri Reddit { get; set; }
        public ulong? Message { get; set; }
        public DateTime SubmissionTime { get; set; }
        public DateTime? ReplyTime { get; set; }

        public enum SubmissionType
        {
            Bug = 0,
            Suggestion = 1,
            Question = 2
        }
    }
}
