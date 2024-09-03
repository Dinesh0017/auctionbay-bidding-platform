﻿using Microsoft.AspNetCore.Identity;

namespace Api.Entities
{
    public class AppUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
