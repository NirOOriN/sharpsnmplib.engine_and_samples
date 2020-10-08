// Handler mapping class.
// Copyright (C) 2009-2010 Lex Li
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using NooN.SnmpEngine.Extended;

namespace NooN.SnmpEngine.Pipeline
{
    /// <summary>
    /// Handler mapping class, who is used to map incoming messages to their handlers.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class HandlerMapping
    {
        private readonly VersionFlags _versionMapping;
        private readonly CommandType _commandMapping;
        private readonly IMessageHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Deprecated, use the constructor with enum VersionMatching and CommandMatching")]
        public HandlerMapping(string version, string command, IMessageHandler handler)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _commandMapping = ConvertToCommandMapping(command);
            _versionMapping = ConvertToVersionMapping(version);
            _handler = handler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version, flagged enum</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
        public HandlerMapping(VersionFlags version, CommandType command, IMessageHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _versionMapping = version;
            _commandMapping = command;
            _handler = handler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version, flagged enum</param>
        /// <param name="command">The command.</param>
        /// <param name="type">The type.</param>
        /// <param name="assembly">The assembly.</param>
        public HandlerMapping(VersionFlags version, CommandType command, string type, string assembly)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _versionMapping = version;
            _commandMapping = command;
            _handler = CreateMessageHandler(assembly, type);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="command">The command.</param>
        /// <param name="type">The type.</param>
        /// <param name="assembly">The assembly.</param>
        [Obsolete("Deprecated, use the constructor with enum VersionMatching and CommandMatching")]
        public HandlerMapping(string version, string command, string type, string assembly)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _versionMapping = ConvertToVersionMapping(version);
            _commandMapping = ConvertToCommandMapping(command);
            _handler = CreateMessageHandler(assembly, type);
        }

        private static IMessageHandler CreateMessageHandler(string assemblyName, string type)
        {
            foreach (var assembly in from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                let name = assembly.GetName().Name
                                where string.Compare(name, assemblyName, StringComparison.OrdinalIgnoreCase) == 0
                                select assembly)
            {
                return (IMessageHandler)Activator.CreateInstance(assembly.GetType(type));
            }

            return (IMessageHandler)Activator.CreateInstance(AppDomain.CurrentDomain.Load(assemblyName).GetType(type));
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>The handler.</value>
        public IMessageHandler Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Determines whether this instance can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     <c>true</c> if this instance can handle the specified message; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(ISnmpMessage message)
        {
            return VersionMatched(message) && CommandMatched(message);
        }

        private bool CommandMatched(ISnmpMessage message)
        {
            var typeCode = message.Pdu().TypeCode;
            return _commandMapping == CommandType.All || (int)typeCode == (int)_commandMapping;
        }

        private bool VersionMatched(ISnmpMessage message)
        {
            return _versionMapping == VersionFlags.All || MatchVersionCode(message.Version, _versionMapping);
        }

        private static bool StringEquals(string left, string right)
        {
            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static VersionFlags ConvertToVersionMapping(string versions)
        {          
            if (string.IsNullOrEmpty(versions))
                return VersionFlags.None;

            if (versions == "*")
                return VersionFlags.All;

            var result = VersionFlags.None;
            var versionArray = versions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var version in versionArray)
            {
                if (StringEquals(version, "v1"))
                    result |= VersionFlags.V1;
                else if (StringEquals(version, "v2"))
                    result |= VersionFlags.V2;
                else if (StringEquals(version, "v3"))
                    result |= VersionFlags.V3;
            }
            return result;
        }

        private static CommandType ConvertToCommandMapping(string command)
        {
            if (string.IsNullOrEmpty(command))
                return CommandType.None;
            if (command == "*")
                return CommandType.All;

            foreach(var commandMap in Enum.GetNames(typeof(CommandType)).Cast<CommandType>())
            {
                if (commandMap == CommandType.None || commandMap == CommandType.All)
                    continue;

                var commandString = commandMap.ToString();
                if (StringEquals(command + "RequestPdu", commandString) || StringEquals(command + "Pdu", commandString))
                    return commandMap;
            }

            return CommandType.None;
        }

        private static bool MatchVersionCode(VersionCode msgVersion, VersionFlags flags)
        {
            //GetFlags would be more "elegant", but this is simply faster and for now good enough to maintain
            if (msgVersion == VersionCode.V1 && (flags & VersionFlags.V1) == VersionFlags.V1) return true;
            if (msgVersion == VersionCode.V2 && (flags & VersionFlags.V2) == VersionFlags.V2) return true;
            if (msgVersion == VersionCode.V2U && (flags & VersionFlags.V2) == VersionFlags.V2) return true;
            if (msgVersion == VersionCode.V3 && (flags & VersionFlags.V3) == VersionFlags.V3) return true;

            return false;
        }
    }
}
