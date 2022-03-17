using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Jackfruit.Models
{
    public class CommandDef
    {
        public CommandDef(
            string id,
            IEnumerable<string> path,
            string? description
            //string[] aliases, 
            //List<MemberDef> members, 
            //string handlerMethodName, 
            //List<CommandDef> subCommands, 
            //string returnType
            )
        {
            Id = id;
            Description = description;
            //Aliases = aliases;
            //Members = members;
            //HandlerMethodName = handlerMethodName;
            //SubCommands = subCommands;
            Path = path;
            //ReturnType = returnType;
        }

        public string Id { get; }
        public string? Description { get; }
        //public string[] Aliases { get; }

        ////Options, args, and services in order of handler parameters
        //public List<MemberDef> Members { get; }
        //public string HandlerMethodName { get; }
        //public List<CommandDef> SubCommands { get; }
        public IEnumerable<string> Path { get; }
        //public string ReturnType { get; }

    }
}
