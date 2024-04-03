namespace BlazorStack.Portal.Services
{
    public interface IAccountManagement
    {
        public Task<string> Login(string email, string password);

        /// <summary>
        /// Log out the logged in user.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public Task Logout();

        /// <summary>
        /// Registration service.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>The result of the request serialized to <see cref="FormResult"/>.</returns>
        public Task<List<string>> Register(string email, string password);

        //public Task<bool> CheckAuthenticatedAsync();
    }
}
