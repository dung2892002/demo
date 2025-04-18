﻿using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.Services
{
    public interface IDepartmentService : IBaseService<Department>
    {
        Task<Department?> GetByName(string name);
    }
}
