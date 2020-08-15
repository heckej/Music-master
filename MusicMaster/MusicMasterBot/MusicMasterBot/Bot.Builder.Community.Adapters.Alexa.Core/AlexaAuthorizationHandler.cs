using System;
using System.Threading.Tasks;
using Crunch.NET.Request;
using Microsoft.Extensions.Logging;

namespace Bot.Builder.Community.Adapters.Crunch.Core
{
    /// <summary>
    /// Alexa Authorization Handler.
    /// </summary>
    public class AlexaAuthorizationHandler
    {
        /// <summary>
        /// SignatureCertChainUrl header name.
        /// </summary>
        public const string SignatureCertChainUrlHeader = @"SignatureCertChainUrl";

        /// <summary>
        /// Signature header name.
        /// </summary>
        public const string SignatureHeader = @"Signature";

        private readonly ILogger _logger;

        public AlexaAuthorizationHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Verify the skill request is for the skill with the specified skill id.
        /// </summary>
        /// <param name="skillRequest">Incoming SkillRequest from Alexa.</param>
        /// <param name="alexaSkillId">Alexa Skill Id.</param>
        /// <returns>True if the SkillRequest is for this skill.</returns>
        public virtual bool ValidateSkillId(CrunchRequest skillRequest, string alexaSkillId)
        {
            return true;
        }

        /// <summary>
        /// Authorize the SkillRequest is coming from Alexa.
        /// </summary>
        /// <param name="skillRequest">Incoming SkillRequest from Alexa.</param>
        /// <param name="requestBody">Full request body from Alexa.</param>
        /// <param name="signatureChainUrl">Signature Chain Url. This is the SignatureCertChainUrl header value.</param>
        /// <param name="signature">Signature. This is the Signature header value.</param>
        /// <returns>True if this is a valid SkillRequest otherwise false.</returns>
        public virtual async Task<bool> ValidateSkillRequest(CrunchRequest skillRequest, string requestBody, string signatureChainUrl, string signature)
        {
            if (skillRequest == null)
            {
                _logger.LogError("Validation failed. No incoming skill request.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(signatureChainUrl))
            {
                _logger.LogError("Validation failed. Empty SignatureCertChainUrl header.");
                return false;
            }

            Uri certUrl;
            try
            {
                certUrl = new Uri(signatureChainUrl);
            }
            catch
            {
                _logger.LogError($"Validation failed. SignatureChainUrl not valid: {signatureChainUrl}.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(signature))
            {
                _logger.LogError("Validation failed. Empty Signature header.");
                return false;
            }

            if (!RequestVerification.RequestTimestampWithinTolerance(skillRequest))
            {
                _logger.LogError("Validation failed. Request timestamp outside of tolerance.");
                return false;
            }
            if (!await RequestVerification.Verify(signature, certUrl, requestBody))
            {
                _logger.LogError("Validation failed. Alexa certificate validation failed.");
                return false;
            }

            return true;
        }
    }
}
