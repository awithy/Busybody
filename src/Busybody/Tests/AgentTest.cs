﻿using Busybody.Config;

namespace Busybody.Tests
{
    public class AgentTest : IBusybodyTest
    {
        public bool Execute(HostConfig host, HostTestConfig test)
        {
            return true;
        }
    }
}