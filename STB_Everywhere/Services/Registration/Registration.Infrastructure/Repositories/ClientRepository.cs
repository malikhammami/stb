using Registration.Application.Contracts.Persistence;
using Registration.Domain.Entities;
using Registration.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Infrastructure.Repositories
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(RegistrationContext dbContext) : base(dbContext)
        {
        }

        
    }
}
