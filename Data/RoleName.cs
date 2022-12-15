namespace ASP_MVC.Data
{
    public static class RoleName
    {
        public static readonly List<string> DefaultRoles = new()
        {
            Administrator,
            Editor,
            Member
        };
        public const string Administrator = "Administrator";
        public const string Editor = "Editor";
        public const string Member = "Member";
    }
}