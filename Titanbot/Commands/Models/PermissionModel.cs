namespace Titanbot.Commands.Models
{
    public class PermissionModel
    {
        public string PermissionKey { get; }
        public ulong DefaultPermission { get; }

        public PermissionModel(string key, ulong permission)
        {
            PermissionKey = key;
            DefaultPermission = permission;
        }
    }
}