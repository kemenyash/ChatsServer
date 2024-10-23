using DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Extensions;

namespace ChatsServer.Services
{
    public class ProtectionService
    {
        private readonly DataContext dataContext;

        public ProtectionService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> CheckToken(string token) => await dataContext.Hashes.AnyAsync(x => x.Token == token);

        public async Task<bool> CheckAuthCredentials(string username, string password)
        {
            var hash = await dataContext.Hashes.FirstOrDefaultAsync(x =>  x.Operator.UserName == username);
            if(hash is null) return false;
            var credentialPasswordHash = GenPasswordHash(hash.Salt, password);
            return hash.Password == credentialPasswordHash;
        }


        private async Task<bool> UpdateToken(int operatorId)
        {
            var hash = await dataContext.Hashes.FirstOrDefaultAsync(x => x.OperatorId == operatorId);
            if (string.IsNullOrEmpty(hash?.Token)) return false;
            var tokenString = GenTokenString(hash.Salt);
            hash.Token = StringHelper.ComputeHash(tokenString);
            dataContext.Hashes.Update(hash);
            await dataContext.SaveChangesAsync();
            return true;
        }

        private static string GenPasswordHash(string salt, string password)
        {
            var passwordString = $"{salt}{password}{salt}";
            return StringHelper.ComputeHash(passwordString);
        }

        private static string GenTokenString(string salt)
        {
            var randomString = StringHelper.GetRandomString();
            return $"{salt}{randomString}{salt}";
        }


    }
}
