using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    internal class EmailService : IEmailService
    {
        public async Task<bool> SendEmail(Email email)
        {
            return true;
        }
    }
}
