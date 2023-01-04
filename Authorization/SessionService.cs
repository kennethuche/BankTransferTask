using BankTransferTask.Core.Services;
using System.Security.Claims;

namespace BankTransferTask.Authorization;

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly ILogger<SessionService> logger;
 
    public SessionService(IHttpContextAccessor contextAccessor, ILogger<SessionService> logger)
    {
        this.contextAccessor = contextAccessor;
        this.logger = logger;

    }
    public ValueTask<string> GetSessionId()
    {
        throw new NotImplementedException();
    }

    private Dictionary<string, Claim> claimsDictionary;
    private static readonly object claimsLocker = new object();

    private void InitClaimsDict()
    {
        var claims = contextAccessor.HttpContext.User.Claims;
        claimsDictionary = new Dictionary<string, Claim>();
        foreach (var claim in claims)
        {
            lock (claimsLocker)
            {
                claimsDictionary[claim.Type] = claim;
            }
        }
    }

    private string instanceSession;

    /// <summary>
    /// Gets The Session Details Of Current User
    /// </summary>
    /// <returns>
    /// Returns  Null if user is unauthenticated
    /// </returns>
    public async ValueTask<string> GetSession()
    {
        try
        {
            if (instanceSession is null) instanceSession = await ProcessSession();
            return instanceSession;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to resolve session");
            return null;
        }
    }
    /// <summary>
    /// Processes HTTP Context Into A Session
    /// </summary>
    /// <returns></returns>
    private async Task<string> ProcessSession()
    {
        if (contextAccessor.HttpContext.User is null) return null;
        if (claimsDictionary is null) InitClaimsDict();

        var contextItems = contextAccessor.HttpContext.Items;

        var sessionId = claimsDictionary["Id"].Value;

      
        if (string.IsNullOrEmpty(sessionId)) sessionId = null;

        logger.LogInformation($"Retreved Session Id");
        return sessionId;

    }


}

