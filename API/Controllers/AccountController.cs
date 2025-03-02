using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] //account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
    {
        if(await UserExists(registerDTO.Username)) return BadRequest("Username is taken");
        
        using var hmac = new HMACSHA512(); //using statement to dispose of the HMACSHA512 object after it is used
        var user = new AppUser{
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
                                              
        await context.SaveChangesAsync();
        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
        if(user == null) return Unauthorized("Invalid username");

        var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); //request wala
        for(int i = 0; i < computedHash.Length; i++){
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }
        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
