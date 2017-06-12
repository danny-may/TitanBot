using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.Responses
{
    public struct PermissionCheckResponse
    {
        public bool IsSuccess { get; private set; }
        public CallInfo[] Permitted { get; private set; }
        public string ErrorMessage { get; private set; }

        public static PermissionCheckResponse FromSuccess(CallInfo[] permitted)
            => new PermissionCheckResponse() { IsSuccess = true, Permitted = permitted };
        public static PermissionCheckResponse FromError(string message)
            => new PermissionCheckResponse() { IsSuccess = false, ErrorMessage = message };
        public static PermissionCheckResponse FromError()
            => new PermissionCheckResponse() { IsSuccess = false };
    }
}
