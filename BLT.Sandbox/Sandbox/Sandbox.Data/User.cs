﻿using System;
using System.Collections.Generic;

namespace Sandbox.Data
{
    public class User// : EntityBase
    {
        // primary key
        public Guid Id { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }

        public User()
        {
            Clients = new List<Client>();
            Campaigns = new List<Campaign>();
        }
    }
}
