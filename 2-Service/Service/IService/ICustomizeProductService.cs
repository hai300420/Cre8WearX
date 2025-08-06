using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _2_Service.Service.IService
{
    public interface ICustomizeProductService
    {
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts();
        Task<CustomizeProduct> GetCustomizeProductById(int id);
        Task AddCustomizeProduct(CustomizeProduct customizeProduct);
        Task UpdateCustomizeProduct(CustomizeProduct customizeProduct);
        Task DeleteCustomizeProduct(int id);
        Task<List<ProductCustomizationCountDto>> GetProductCustomizationCounts();
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProductsAsync();
        Task<CustomizeProduct> GetCustomizeProductByIdAsync(int id);
        Task<IEnumerable<CustomizeProduct>> GetCustomizeProductsByCurrentUserAsync(int userId);
        Task UpdateCustomizeProductAsync(CustomizeProduct product);
        Task DeleteCustomizeProductAsync(int id);
        // Task<CustomizeProduct> CreateCustomizeProductAsync(CustomizeProduct product);
        Task<CustomizeProduct> CreateCustomizeProductAsync(CreateCustomizeDto dto);
        Task<CustomizeProductWithOrderResponse> CreateCustomizeProductWithOrderAsync(CreateCustomizeWithOrderDto dto);
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts(int pageNumber, int pageSize);
        Task<IEnumerable<CustomizeProduct>> GetMyCustomizeProducts(int pageNumber, int pageSize);

    }
}
