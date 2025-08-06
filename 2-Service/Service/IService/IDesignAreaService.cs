using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Service.IService
{
    public interface IDesignAreaService
    {
        Task<IEnumerable<DesignArea>> GetAllDesignAreas();
        Task<DesignArea> GetDesignAreaById(int id);
        Task AddDesignArea(DesignArea designArea);
        Task UpdateDesignArea(DesignArea designArea);
        Task DeleteDesignArea(int id);
    }
}
