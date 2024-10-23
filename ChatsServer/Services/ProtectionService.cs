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
using System.Collections.Concurrent;

namespace ChatsServer.Services
{
    public class ProtectionService
    {
        private readonly DataContext dataContext;
        private static ConcurrentDictionary<int, string> tokens;

        public ProtectionService(DataContext dataContext)
        {
            this.dataContext = dataContext;
            if (tokens is null) tokens = new ConcurrentDictionary<int, string>();
        }

        public async Task<bool> CheckToken(int id, string token) => tokens.TryGetValue(id, out var tokenData) && tokenData == token;
        public async Task<string> GetToken(int id) 
        {
            if(tokens.TryGetValue(id, out var token))
            {
                return token;
            }

            return string.Empty;
         }

        public async Task<bool> CheckAuthCredentials(string username, string password)
        {
            var hash = await dataContext.Hashes.FirstOrDefaultAsync(x =>  x.Operator.UserName == username);
            if(hash is null) return false;
            var credentialPasswordHash = GenPasswordHash(hash.Salt, password);
            return hash.Password == credentialPasswordHash;
        }


        public async Task<string> UpdateToken(string username)
        {
            if(string.IsNullOrEmpty(username)) return string.Empty;
            var user = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
            return await UpdateToken(user?.Id ?? 0);
        }

        public async Task<string> UpdateToken(int operatorId)
        {
            var hash = await dataContext.Hashes.FirstOrDefaultAsync(x => x.OperatorId == operatorId);
            if (hash?.Token is null) return string.Empty;
            var tokenString = GenTokenString(hash.Salt);
            hash.Token = StringHelper.ComputeHash(tokenString);
            dataContext.Hashes.Update(hash);
            await dataContext.SaveChangesAsync();
            tokens[operatorId] = hash.Token;
            return hash.Token;
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
