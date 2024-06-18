using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class TeacherRegistryService : ITeacherRegistryService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public TeacherRegistryService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> Read()
        {
            var data = (from d in entities.sqlTEACHERS_WITH_AITISEIS_UNIQUE
                        orderby d.FULLNAME, d.AFM
                        select d).ToList();
            return data;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}