using System;

namespace AridityTeam.Base
{
    public interface IConCommand : IConCommandBase
    {
        /// <summary>
        /// Executes the <see cref="ConCommand"/>.
        /// </summary>
        public void Execute(string? args);

        /// <summary>
        /// Gets the help string.
        /// </summary>
        /// <returns></returns>
        public string? GetHelpString();

        /// <summary>
        /// Executes the <see cref="ConCommand"/>.
        /// </summary>
        /// <param name="callback">Execute handler of the <see cref="ConCommand"/> to run.</param>
        public void Execute(ConCommandExecuteHandler? callback, ConCommandArgs args);

        /// <summary>
        /// Return the name of the ConCommand.
        /// </summary>
        /// <returns>Returns the name of the ConCommand.</returns>
        public string? GetName();

        /// <summary>
        /// Gets the ConCommand.
        /// </summary>
        /// <returns></returns>
        public IConCommand GetConCommand();
    }
}
