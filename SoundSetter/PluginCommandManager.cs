﻿using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using SoundSetter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable ForCanBeConvertedToForeach

namespace SoundSetter
{
    public class PluginCommandManager<THost> : IDisposable
    {
        private readonly ICommandManager commands;
        private readonly (string, CommandInfo)[] pluginCommands;
        private readonly THost host;

        public PluginCommandManager(THost host, ICommandManager commands)
        {
            this.commands = commands;
            this.host = host;

            this.pluginCommands = host.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                                                            BindingFlags.Static | BindingFlags.Instance)
                .Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
                .SelectMany(GetCommandInfoTuple)
                .ToArray();

            AddCommandHandlers();
        }

        // http://codebetter.com/patricksmacchia/2008/11/19/an-easy-and-efficient-way-to-improve-net-code-performances/
        // Benchmarking this myself gave similar results, so I'm doing this to somewhat counteract using reflection to access command attributes.
        // I like the convenience of attributes, but in principle it's a bit slower to use them as opposed to just initializing CommandInfos directly.
        // It's usually sub-1 millisecond anyways, though. It probably doesn't matter at all.
        private void AddCommandHandlers()
        {
            for (var i = 0; i < this.pluginCommands.Length; i++)
            {
                var (command, commandInfo) = this.pluginCommands[i];
                this.commands.AddHandler(command, commandInfo);
            }
        }

        private void RemoveCommandHandlers()
        {
            for (var i = 0; i < this.pluginCommands.Length; i++)
            {
                var (command, _) = this.pluginCommands[i];
                this.commands.RemoveHandler(command);
            }
        }

        private IEnumerable<(string, CommandInfo)> GetCommandInfoTuple(MethodInfo method)
        {
            var handlerDelegate =
                (IReadOnlyCommandInfo.HandlerDelegate)Delegate.CreateDelegate(
                    typeof(IReadOnlyCommandInfo.HandlerDelegate), this.host, method);

            var command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
            var aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
            var helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();
            var doNotShowInHelp = handlerDelegate.Method.GetCustomAttribute<DoNotShowInHelpAttribute>();

            ArgumentNullException.ThrowIfNull(command);

            var commandInfo = new CommandInfo(handlerDelegate)
            {
                HelpMessage = helpMessage?.HelpMessage ?? string.Empty,
                ShowInHelp = doNotShowInHelp == null,
            };

            // Create list of tuples that will be filled with one tuple per alias, in addition to the base command tuple.
            var commandInfoTuples = new List<(string, CommandInfo)> { (command.Command, commandInfo) };
            if (aliases != null)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < aliases.Aliases.Length; i++)
                {
                    commandInfoTuples.Add((aliases.Aliases[i], commandInfo));
                }
            }

            return commandInfoTuples;
        }

        public void Dispose()
        {
            RemoveCommandHandlers();
        }
    }
}