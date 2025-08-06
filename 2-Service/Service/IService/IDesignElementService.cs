using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _2_Service.Service.IService
{
    public interface IDesignElementService
    {
        Task<IEnumerable<DesignElementDTO>> GetAllDesignElements();
        Task<DesignElementDTO?> GetDesignElementById(int id);
        Task AddDesignElement(DesignElement designElement);
        Task UpdateDesignElement(DesignElement designElement);
        Task DeleteDesignElement(int id);

    }
}
