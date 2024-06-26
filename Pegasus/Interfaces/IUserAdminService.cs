﻿using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUserAdminService
    {
        void Create(UserAdminViewModel data);
        void Destroy(UserAdminViewModel data);
        List<UserAdminViewModel> Read();
        void Update(UserAdminViewModel data);
    }
}