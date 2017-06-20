using System.Threading.Tasks;

namespace TitanBot.Util
{
    public static class TaskUtil
    {
        public static void DontWait(this Task task) { }
    }
}
