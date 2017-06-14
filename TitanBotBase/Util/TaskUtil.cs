using System.Threading.Tasks;

namespace TitanBotBase.Util
{
    public static class TaskUtil
    {
        public static void DontWait(this Task task) { }
    }
}
