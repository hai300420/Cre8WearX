using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enum
{
    public enum UserRole
    {

        None = 0,
        Customer,
        Designer,
        Staff,
        Manager,
        Admin,
    }

    public enum UserStatus
    {
        None = 0,
        Active,
        Deleted,
    }
    public enum UserGender
    {
    //    None = 0,
        Male = 1,
        Female = 0
    }

}
