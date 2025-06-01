using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Momo
{
    public interface IMoMoService
    {
        bool ValidateSignature(IQueryCollection query, string receivedSignature);
    }
}
