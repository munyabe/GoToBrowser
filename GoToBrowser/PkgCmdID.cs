// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;
using System.ComponentModel.Design;
using GoToBrowser.Utils;

namespace GoToBrowser
{
    static class PkgCmdIDList
    {
        public const uint configureCommand = 0x100;
        public const uint goToBrowserCommand1 = 0x101;

        /// <summary>
        /// 設定できる Go to Brouser コマンド数の上限値です。
        /// </summary>
        public const int MAX_COMMAND_COUNT = 8;

        /// <summary>
        /// Go to Brouser コマンドの Id を取得します。
        /// </summary>
        /// <param name="index">コマンドのインデックス</param>
        /// <returns>コマンド Id</returns>
        public static CommandID GetCommandId(int index)
        {
            if (index < 0 || 7 < index)
            {
                throw new ArgumentOutOfRangeException(string.Format("The int argument [{0}] must be positive and less than 8.", index));
            }

            return new CommandID(GuidList.guidGoToBrowserCmdSet, (int)goToBrowserCommand1 + index);
        }

        /// <summary>
        /// Go to Brouser コマンドのインデックスを取得します。
        /// </summary>
        /// <param name="commandID">コマンド Id</param>
        /// <returns>コマンドのインデックス</returns>
        public static int GetCommandIndex(CommandID commandID)
        {
            Guard.ArgumentNotNull(commandID, "commandID");
            return commandID.ID - (int)goToBrowserCommand1;
        }
    };
}