using Microsoft.EntityFrameworkCore;
using RaspSecure.Context;
using RaspSecure.Helpers;
using RaspSecure.Models;
using RaspSecure.Models.Auth;
using RaspSecure.Models.DTO;
using RaspSecure.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Services
{
    public sealed class UserService
    {
        private readonly RaspSecureDbContext _context;
        public UserService(RaspSecureDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CheckAdminUser()
        {
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Role == RolesEnum.Admin);
            return !(admin is null);
        }

        public async Task<ICollection<UserDTO>> GetUsers()
        {
           return await _context.Users.Select(u =>
                new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    Role = u.Role
                }).ToListAsync();
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                throw new NotFoundException(nameof(UserEntity), id);

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role
            };
        }

        public async Task<UserDTO> CreateUser(UserRegisterDTO userDTO, bool saveRole = false)
        {
            var role = RolesEnum.Deactivated;
            if (!IsEmailUniqu(userDTO.Email))
                throw new ArgumentException("Email already exist");
            if (!await CheckAdminUser())
                role = RolesEnum.Admin;
            if (saveRole)
                role = userDTO.Role;

            var userEntity = new UserEntity
            {
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now,
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                Role = role
            };
            var salt = SecurityHelper.GetRandomBytes();

            userEntity.Salt = Convert.ToBase64String(salt);
            userEntity.Password = SecurityHelper.HashPassword(userDTO.Password, salt);

            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = userEntity.Id,
                Email = userEntity.Email,
                UserName = userEntity.UserName,
                Role = userEntity.Role
            };
        }

        public async Task UpdateUser(UserDTO userDTO, int userId)
        {
            if (userId != userDTO.Id)
                throw new InvalidUsernameOrPasswordException();
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDTO.Id);
            if (userEntity is null)
                throw new NotFoundException(nameof(UserEntity), userDTO.Id);

            var timeNow = DateTimeOffset.Now;

            userEntity.UserName = userDTO.UserName;
            userEntity.UpdatedAt = timeNow;

            _context.Users.Update(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int userId)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userEntity is null)
                throw new NotFoundException(nameof(UserEntity), userId);

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();
        }

        public bool IsEmailUniqu(string email)
        {
            var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
            return user is null;
        }
    }
}
