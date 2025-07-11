using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(AppUser user);

    }
}
