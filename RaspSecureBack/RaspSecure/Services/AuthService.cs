using Microsoft.EntityFrameworkCore;
using RaspSecure.Context;
using RaspSecure.Helpers;
using RaspSecure.Models;
using RaspSecure.Models.Auth;
using RaspSecure.Models.DTO;
using RaspSecure.Models.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Services
{
    public sealed class AuthService
    {
        private readonly JwtFactory _jwtFactory;
        private readonly MailService _mailService;
        private readonly RaspSecureDbContext _context;

        public AuthService(RaspSecureDbContext context, JwtFactory jwtFactory, MailService mailService)
        {
            _context = context;
            _jwtFactory = jwtFactory;
            _mailService = mailService;
        }

        public async Task<AuthUserDTO> Authorize(UserLoginDTO userDTO)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);

            if (userEntity is null)
                throw new InvalidUsernameOrPasswordException();

            if (!SecurityHelper.ValidatePassword(userDTO.Password, userEntity.Password, userEntity.Salt))
                throw new InvalidUsernameOrPasswordException();

            var token = await GenerateAccessToken(userEntity.Id, userEntity.UserName, userEntity.Email, userEntity.Role);
            var user = new UserDTO
            {
                Id = userEntity.Id,
                Email = userEntity.Email,
                UserName = userEntity.UserName,
                Role = userEntity.Role,
                CreatedAt = userEntity.CreatedAt
            };

            return new AuthUserDTO
            {
                User = user,
                Token = token
            };
        }

        public async Task<AccessTokenDTO> GenerateAccessToken(int userId, string userName, string email, RolesEnum role)
        {
            var refreshToken = _jwtFactory.GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            });

            await _context.SaveChangesAsync();

            var accessToken = await _jwtFactory.GenerateAccessToken(userId, userName, email, role);

            return new AccessTokenDTO(accessToken, refreshToken);
        }

        public async Task<AccessTokenDTO> RefreshToken(RefreshTokenDTO tokenDTO)
        {
            var userId = _jwtFactory.GetUserIdFromToken(tokenDTO.AccessToken);
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userEntity is null)
                throw new NotFoundException(nameof(UserEntity), userId);

            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenDTO.RefreshToken && t.UserId == userId);

            if (refreshToken is null)
                throw new InvalidTokenException("refresh");

            if (!refreshToken.IsActive)
                throw new ExpiredRefreshTokenException();

            var jwtToken = await _jwtFactory.GenerateAccessToken(userEntity.Id, userEntity.UserName, userEntity.Email, userEntity.Role);
            var newRefreshToken = _jwtFactory.GenerateRefreshToken();

            _context.RefreshTokens.Remove(refreshToken);
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = userEntity.Id,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            });

            await _context.SaveChangesAsync();

            return new AccessTokenDTO(jwtToken, newRefreshToken);
        }

        public async Task RevokeRefreshToken(string refreshToken, int userId)
        {
            var token = _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken && t.UserId == userId);

            if (token is null)
                throw new InvalidTokenException("refresh");

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        public async Task Reset(string address, string url)
        {
            var userEntity = await _context.Users.Where(u => u.Email == address).FirstOrDefaultAsync();
            if (userEntity is object)
            {
                var refreshToken = _jwtFactory.GenerateRefreshToken();
                _context.ResetToken.Add(new ResetToken
                {
                    Token = refreshToken,
                    UserId = userEntity.Id,
                    CreatedAt = DateTimeOffset.Now,
                    UpdatedAt = DateTimeOffset.Now
                });
                await _context.SaveChangesAsync();
                await _mailService.Send(address, url, refreshToken);
            }
        }
        public async Task Update(string password, string token)
        {
            var resetToken = await _context.ResetToken.FirstOrDefaultAsync(t => t.Token == token);
            if (resetToken is null)
                throw new InvalidTokenException("InvalidToken");

            if (!resetToken.IsActive)
            {
                _context.ResetToken.Remove(resetToken);
                await _context.SaveChangesAsync();
                throw new ExpiredRefreshTokenException();
            }
            var userEntity = await _context.Users.FindAsync(resetToken.UserId);
            var salt = SecurityHelper.GetRandomBytes();
            userEntity.Salt = Convert.ToBase64String(salt);
            userEntity.Password = SecurityHelper.HashPassword(password, salt);
            _context.Users.Update(userEntity);
            _context.ResetToken.RemoveRange(_context.ResetToken.Where(t => t.UserId == userEntity.Id));
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(t => t.UserId == userEntity.Id));
            await _context.SaveChangesAsync();
        }

        public async Task Update(string password, int userId)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity is null)
                throw new NotFoundException(nameof(UserEntity), userId);
            var salt = SecurityHelper.GetRandomBytes();
            userEntity.Salt = Convert.ToBase64String(salt);
            userEntity.Password = SecurityHelper.HashPassword(password, salt);
            _context.Users.Update(userEntity);
            _context.ResetToken.RemoveRange(_context.ResetToken.Where(t => t.UserId == userEntity.Id));
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(t => t.UserId == userEntity.Id));
            await _context.SaveChangesAsync();
        }
    }
}
