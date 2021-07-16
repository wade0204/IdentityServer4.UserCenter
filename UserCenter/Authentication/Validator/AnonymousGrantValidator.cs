using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Validator
{
    public class AnonymousGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public AnonymousGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public string GrantType => "anonymous";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var claims = new List<Claim>() { new Claim("role", GrantType) };
            context.Result = new GrantValidationResult(GrantType, GrantType, claims);
        }
    }
}
