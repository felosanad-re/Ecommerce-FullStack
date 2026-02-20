using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services.Contract.AuthServices
{
    public interface IDbInitialization
    {
        Task CreateInitializationAsync();
    }
}
