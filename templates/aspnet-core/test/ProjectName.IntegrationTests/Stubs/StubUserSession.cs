namespace ProjectName.IntegrationTests.Stubs
{
    public class StubUserSession : IUserSession
    {
        public StubUserSession(long userId)
        {
            UserId = userId.ToString();
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public bool IsGranted(string permission)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthenticated { get; }
        public string UserId { get; }
        public string UserName { get; }
        public string BranchId { get; }
        public string BranchName { get; }
        public bool IsHeadOffice { get; }
        public IReadOnlyList<string> Permissions { get; }
        public IReadOnlyList<string> Roles { get; }
        public IReadOnlyList<Claim> Claims { get; }
        public string UserDisplayName { get; }
        public string UserBrowserName { get; }
        public string UserIP { get; }
        public string ImpersonatorUserId { get; }
    }
}