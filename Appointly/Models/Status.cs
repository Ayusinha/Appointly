﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Models
{
    public enum Status
    {
        Pending = 1,
        Accept,
        Decline,
        Completed,
        Cancelled
    }
}
