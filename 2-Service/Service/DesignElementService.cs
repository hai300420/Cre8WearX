using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _2_Service.Service
{
    public interface IDesignElementService
    {
        Task<IEnumerable<DesignElementDTO>> GetAllDesignElements();
        Task<DesignElementDTO?> GetDesignElementById(int id);
        Task AddDesignElement(DesignElement designElement);
        Task UpdateDesignElement(DesignElement designElement);
        Task DeleteDesignElement(int id);

    }
    public class DesignElementService : IDesignElementService
    {
        private readonly IDesignElementRepository _designElementRepository;
        public DesignElementService(IDesignElementRepository designElementRepository)
        {
            _designElementRepository = designElementRepository;
        }
        public async Task AddDesignElement(DesignElement designElement)
        {
            await _designElementRepository.AddAsync(designElement);
        }

        public async Task DeleteDesignElement(int id)
        {
            await _designElementRepository.DeleteAsync(id);
        }
            
       
        public async Task<IEnumerable<DesignElementDTO>> GetAllDesignElements()
        {
            var designElements = await _designElementRepository.GetAllAsync();
            return designElements.Select(de => new DesignElementDTO
            {
                DesignElementId = de.DesignElementId,
                Image = de.Image,
                Text = de.Text,
                Size = de.Size,
                ColorArea = de.ColorArea,
                DesignAreaId = de.DesignAreaId,
                AreaName = de.DesignArea.AreaName,
                CustomizeProductId = de.CustomizeProductId,
                ShirtColor = de.CustomizeProduct.ShirtColor,
                FullImage = de.CustomizeProduct.FullImage
            }).ToList();
        }

        public async Task<DesignElementDTO?> GetDesignElementById(int id)
        {
            var de = await _designElementRepository.GetByIdAsync(id);
            if (de == null) return null;

            return new DesignElementDTO
            {
                DesignElementId = de.DesignElementId,
                Image = de.Image,
                Text = de.Text,
                Size = de.Size,
                ColorArea = de.ColorArea,
                DesignAreaId = de.DesignAreaId,
                AreaName = de.DesignArea.AreaName,
                CustomizeProductId = de.CustomizeProductId,
                ShirtColor = de.CustomizeProduct.ShirtColor,
                FullImage = de.CustomizeProduct.FullImage
            };
        }

        public async Task UpdateDesignElement(DesignElement designElement)
        {
            await _designElementRepository.UpdateAsync(designElement);
        }

    }

}
