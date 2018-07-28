/// <copyright file="QueueItem.cs" company="Ron Kuslak">Copyright (c) Ron Kuslak. All rights reserved.</copyright>
/// <author>Ron Kuslak</author>

namespace SqlSpeaker.Database
{
    public class QueueItem
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public int Response { get; set; }

        public bool Processed { get; set; }
    }
}