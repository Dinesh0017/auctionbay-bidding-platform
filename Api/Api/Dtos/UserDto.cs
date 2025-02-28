﻿namespace Api.Dtos
{
    public record class UserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        public string? Id { get; set; }
    }
}
