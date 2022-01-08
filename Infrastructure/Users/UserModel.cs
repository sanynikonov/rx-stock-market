﻿using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Users;

public class UserModel : IdentityUser<int>
{
    public ICollection<CompanyModel> RequestedCompanies { get; set; } = Array.Empty<CompanyModel>();
}