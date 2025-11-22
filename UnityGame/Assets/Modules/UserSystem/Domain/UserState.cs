using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityGame.Assets.Modules.UserSystem.Domain
{
    public class UserState
    {
        public string UserName { get; private set; }
        public string TeamName { get; private set; }
        public string UserId { get; private set; }

        public UserState(string userName, string teamName, string userId = null)
        {
            UserName = userName;
            TeamName = teamName;
            UserId = userId;
        }
    }
}