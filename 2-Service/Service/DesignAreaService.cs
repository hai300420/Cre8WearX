using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using BusinessObject.Model;

namespace _2_Service.Service
{
    public interface IDesignAreaService
    {
        Task<IEnumerable<DesignArea>> GetAllDesignAreas();
        Task<DesignArea> GetDesignAreaById(int id);
        Task AddDesignArea(DesignArea designArea);
        Task UpdateDesignArea(DesignArea designArea);
        Task DeleteDesignArea(int id);
    }
    public class DesignAreaService : IDesignAreaService
    {
        private readonly IDesignAreaRepository _designAreaRepository;
        public DesignAreaService(IDesignAreaRepository designAreaRepository)
        {
            _designAreaRepository = designAreaRepository;
        }
        public async Task AddDesignArea(DesignArea designArea)
        {
            await _designAreaRepository.AddDesignAreaAsync(designArea);
        }

        public async Task DeleteDesignArea(int id)
        {
            await _designAreaRepository.DeleteDesignAreaAsync(id);
        }

        public async Task<IEnumerable<DesignArea>> GetAllDesignAreas()
        {
            return await _designAreaRepository.GetAllDesignAreaAsync();
        }

        public Task<DesignArea> GetDesignAreaById(int id)
        {
            return _designAreaRepository.GetDesignAreaByIdAsync(id);
        }

        public async Task UpdateDesignArea(DesignArea designArea)
        {
            await _designAreaRepository.UpdateDesignAreaAsync(designArea);
        }
    }

}
