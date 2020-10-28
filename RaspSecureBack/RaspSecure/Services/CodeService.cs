using Microsoft.EntityFrameworkCore;
using RaspSecure.Context;
using RaspSecure.Models;
using RaspSecure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Services
{
    public class CodeService
    {
        private readonly RaspSecureDbContext _context;
        private readonly LogsDbContext _logs;
        public CodeService (RaspSecureDbContext context, LogsDbContext logs)
        {
            _context = context;
            _logs = logs;
        }

        public async Task<SecurityCode> AddCodeAsync(SecurityCodeCreateDTO newCode)
        {
            if (await _context.SecurityCodes.FirstOrDefaultAsync(c => c.Code == newCode.Code) is object)
                throw new ArgumentException($"Code {newCode.Code} already exist");
            var code = new SecurityCode()
            {
                Code = newCode.Code,
                Description = newCode.Description,
                IsActive = newCode.IsActive,
                CreatedAt = DateTimeOffset.Now,
                ExpiredAt = newCode.ExpiredAt,
            };
            await _context.SecurityCodes.AddAsync(code);
            await _context.SaveChangesAsync();
            return code;
        }
        public async Task<SecurityCode> GetCodeAsync(int codeId)
        {
            var result = await _context.SecurityCodes.FirstOrDefaultAsync(c => c.SecurityCodeId == codeId);
            if (result is null)
                throw new ArgumentException($"Code with id \"{codeId}\" not found");
            return result;
        }
        public async Task<IEnumerable<SecurityCode>> GetAllAsync()
        {
            return await _context.SecurityCodes.ToListAsync();
        }

        public async Task<bool> CheckCodeAsync(string code)
        {
            var result = await _context.SecurityCodes.FirstOrDefaultAsync(c => c.Code == code);
            var isValid = false;
            var recognizedId = -1;
            if (result is object && result.ExpiredAt > DateTime.Now)
            {
                isValid = result.IsActive;
                recognizedId = result.SecurityCodeId;
            }
            var log = new LogEntity()
            {
                Code = code,
                AccessTime = DateTime.Now,
                IsSucceed = isValid,
                SecurityCodeId = recognizedId,
            };
            await _logs.LogEntitys.AddAsync(log);
            await _logs.SaveChangesAsync();
            return isValid;
        }
    }
}
