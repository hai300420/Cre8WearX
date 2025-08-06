using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO> GetListProductsAsync();
        Task<ResponseDTO> GetAvailableProductsAsync();
        Task<ResponseDTO> GetProductByIdAsync(int id);
        Task<ResponseDTO> CreateProductAsync(ProductCreateDTO productDto);
        Task<ResponseDTO> UpdateProductAsync(ProductUpdateDTO productDto);
        Task<ResponseDTO> DeleteProductAsync(int id);
    }

}
