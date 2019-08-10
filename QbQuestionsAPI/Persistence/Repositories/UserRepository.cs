using System.Linq;
using System.Security.Cryptography;  
using System.Text;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Persistence.Contexts;

namespace QbQuestionsAPI.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool IsValidUser(User user)
        {
            return true;
            // User dbUser = _context.Users.Where(u => u.Username == user.Username).FirstOrDefault();
            // string passwordHash;

            // using (SHA256 sha256Hash = SHA256.Create())  
            // {  
            //     byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Password));  
            //     StringBuilder sb = new StringBuilder();  
            //     for (int i = 0; i < bytes.Length; i++)  
            //     {  
            //         sb.Append(bytes[i].ToString("x2"));  
            //     }  
            //     passwordHash = sb.ToString();
            // }

            // return passwordHash == dbUser.Password;
        }
    }
}