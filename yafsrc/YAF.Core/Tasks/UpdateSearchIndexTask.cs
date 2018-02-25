/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Tasks
{
    #region Using

    using System;
    using System.Linq;
    using System.Threading;

    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The Update SearchI ndex task.
    /// </summary>
    public class UpdateSearchIndexTask : IntermittentBackgroundTask
    {
        #region Constants and Fields

        /// <summary>
        ///   The _task name.
        /// </summary>
        private const string _TaskName = "UpdateSearchIndexTask";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UpdateSearchIndexTask" /> class.
        /// </summary>
        public UpdateSearchIndexTask()
        {
            // set interval values...
            this.RunPeriodMs = 3600000;
            this.StartDelayMs = 30000;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets TaskName.
        /// </summary>
        public static string TaskName => _TaskName;

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                // get all boards...
                var boardIds = this.GetRepository<Board>().ListTyped().Select(x => x.ID).ToList();

                // go through each board...
                foreach (var boardId in boardIds)
                {
                    var messages = this.GetRepository<Message>().GetAllMessagesByBoard(boardId);

                    this.Get<ISearch>().AddUpdateSearchIndex(messages);

                    // sleep for a second...
                    Thread.Sleep(1000);
                }
            }
            catch (Exception x)
            {
                this.Logger.Error(x, "Error In {0} Task".FormatWith(TaskName));
            }
        }

        #endregion
    }
}