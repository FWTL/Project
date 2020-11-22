﻿using System;

namespace FWTL.Aggregate
{
    public class AccountJob
    {
        public Guid JobId { get; set; }

        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }

        public virtual Job Job { get; set; }
    }
}