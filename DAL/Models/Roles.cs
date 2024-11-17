using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static string[] GetRoles()
            => [Admin, Customer];

        public static bool IsRoleValid(this string role)
            => GetRoles().Contains(role);
    }
}
