using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Utils
{
    public class FirebaseAuthService
    {
        public async Task<string> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                return payload.Email; // Return email if valid
            }
            catch (Exception)
            {
                return null; // Invalid token
            }
        }
    }
}
