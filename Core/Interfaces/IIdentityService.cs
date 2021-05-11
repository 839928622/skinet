using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
   public interface IIdentityService
    {
        string GetUserIdentity();

        string GetUserName();
        string GetUserEmail();
    }
}
