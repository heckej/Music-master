using Crunch.NET.Response.Directive;

namespace Crunch.NET.ConnectionTasks
{
    public static class IConnectionTaskExtensions
    {

        public static StartConnectionDirective ToConnectionDirective(this IConnectionTask task, string token = null)
        {
            return new StartConnectionDirective(task, token);
        }
    }
}
