using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;
using Repository;

namespace _3_Repository.IRepository
{
    public interface IDesignAreaRepository : IGenericRepository<DesignArea>
    {
        Task<IEnumerable<DesignArea>> GetAllDesignAreaAsync();
        Task<DesignArea> GetDesignAreaByIdAsync(int id);
        Task AddDesignAreaAsync(DesignArea designArea);
        Task UpdateDesignAreaAsync(DesignArea designArea);
        Task DeleteDesignAreaAsync(int id);
    }

}
