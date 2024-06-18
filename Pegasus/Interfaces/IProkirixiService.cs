using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IProkirixiService
    {
        void Create(ProkirixisViewModel data);
        void Destroy(ProkirixisViewModel data);
        List<ProkirixisViewModel> Read();
        void Update(ProkirixisViewModel data);
    }
}