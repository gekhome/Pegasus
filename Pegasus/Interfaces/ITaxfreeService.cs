using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface ITaxfreeService
    {
        void Create(TaxFreeViewModel data);
        void Destroy(TaxFreeViewModel data);
        List<TaxFreeViewModel> Read();
        void Update(TaxFreeViewModel data);
    }
}