using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// 自定义 Resource Owner Password Validator
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //目前只有testuser
        private readonly TestUserStore _testUserStore;
        private readonly ISystemClock _clock;
        public CustomResourceOwnerPasswordValidator(TestUserStore testUserStore, ISystemClock clock)
        {
            _testUserStore = testUserStore;
            _clock = clock;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_testUserStore.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _testUserStore.FindByUsername(context.UserName);

                context.Result = new GrantValidationResult(
                    subject: user.SubjectId ?? throw new ArgumentException("Subject ID not set", nameof(user.SubjectId)),
                    authenticationMethod: OidcConstants.AuthenticationMethods.Password,
                    authTime: _clock.UtcNow.UtcDateTime,
                    claims: user.Claims
                );
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
            return Task.CompletedTask;
        }
    }
}
