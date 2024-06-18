using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IApokleismoiService
    {
        void Create(ApokleismoiViewModel data);
        void Destroy(ApokleismoiViewModel data);
        List<ApokleismoiViewModel> Read();
        void Update(ApokleismoiViewModel data);
    }
}