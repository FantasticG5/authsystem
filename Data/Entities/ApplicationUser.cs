using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities;

public class ApplicationUser : IdentityUser
{
    [ProtectedPersonalData]
    public string Firstname { get; set; } = null!;

    [ProtectedPersonalData]
    public string Lastname { get; set; } = null!;

}
