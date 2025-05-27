using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;
using Repository;

namespace _3_Repository.IRepository
{
    public interface IDesignElementRepository : IGenericRepository<DesignElement>
    {
        Task<IEnumerable<DesignElement>> GetAllAsync();
        Task<DesignElement> GetByIdAsync(int id);
        Task AddAsync(DesignElement designElement);
        Task UpdateAsync(DesignElement designElement);
        Task DeleteAsync(int id);
    }

}
