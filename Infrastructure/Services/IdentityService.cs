using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
  public  class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <inheritdoc />
        public string GetUserIdentity()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        /// <inheritdoc />
        public string GetUserName()
        {
            return _context.HttpContext.User.Identity.Name;
        }

        /// <inheritdoc />
        public string GetUserEmail()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
        }
    }
}
