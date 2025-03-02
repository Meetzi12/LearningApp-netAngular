using System;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; } //Entity Framework Convention => Id named property will be used as primary key in the DB; if it is of "int: datatype EF will auto increment it
    public required string UserName { get; set; }
    public required byte[] PasswordHash { get; set; } 
    public required byte[] PasswordSalt { get; set; } //Salt will be used to hash the password
    
}
