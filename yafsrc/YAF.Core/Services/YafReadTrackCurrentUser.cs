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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     YAF Read Tracking Methods
    /// </summary>
    public class YafReadTrackCurrentUser : IReadTrackCurrentUser, IHaveServiceLocator
    {
        #region Fields

        /// <summary>
        ///     The session state.
        /// </summary>
        private readonly HttpSessionStateBase sessionState;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafReadTrackCurrentUser" /> class. The yaf read track current user.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="sessionState">The session State.</param>
        public YafReadTrackCurrentUser(IServiceLocator serviceLocator, HttpSessionStateBase sessionState)
        {
            this.ServiceLocator = serviceLocator;
            this.sessionState = sessionState;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the last visit.
        /// </summary>
        public DateTime LastRead
        {
            get
            {
                var lastRead = this.sessionState["LastRead"] == null
                                   ? null
                                   : this.sessionState["LastRead"].ToType<DateTime?>();

                if (!lastRead.HasValue && this.UseDatabaseReadTracking)
                {
                    lastRead = this.Get<IDbFunction>().GetData.user_lastread(this.CurrentUserId);
                }
                else
                {
                    lastRead = this.Get<IYafSession>().LastVisit;
                }

                return lastRead ?? DateTimeHelper.SqlDbMinTime();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///     Gets the current user id.
        /// </summary>
        protected int CurrentUserId
        {
            get
            {
                return YafContext.Current.PageUserID;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this user is guest.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this user is guest; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGuest
        {
            get
            {
                return YafContext.Current.IsGuest;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use database read tracking].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use database read tracking]; otherwise, <c>false</c>.
        /// </value>
        protected bool UseDatabaseReadTracking
        {
            get
            {
                return this.Get<YafBoardSettings>().UseReadTrackingByDatabase && !this.IsGuest;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get forum read.
        /// </summary>
        /// <param name="forumId">The forum ID of the Forum</param>
        /// <param name="readTimeOverride">The read Time Override.</param>
        /// <returns>
        ///     Returns the DateTime object from the Forum ID.
        /// </returns>
        public DateTime GetForumRead(int forumId, DateTime? readTimeOverride = null)
        {
            DateTime? readTime;

            if (this.UseDatabaseReadTracking)
            {
                if (readTimeOverride.HasValue)
                {
                    // use it if it's not the min value...
                    readTime = readTimeOverride.Value > DateTimeHelper.SqlDbMinTime()
                                   ? readTimeOverride.Value
                                   : this.GetRepository<ForumReadTracking>().Lastread(this.CurrentUserId, forumId);
                }
                else
                {
                    readTime = this.GetRepository<ForumReadTracking>().Lastread(this.CurrentUserId, forumId);
                }
            }
            else
            {
                this.Get<ILogger>().Info("3");

                readTime = this.GetSessionForumRead(forumId);

                if (!readTime.HasValue)
                {
                    // use the last visit...
                    readTime = this.LastRead;
                }
            }

            return readTime ?? DateTimeHelper.SqlDbMinTime();
        }

        /// <summary>
        ///     The get topic read.
        /// </summary>
        /// <param name="topicId">The topicID you wish to find the DateTime object for.</param>
        /// <param name="readTimeOverride">The read Time Override.</param>
        /// <returns>
        ///     Returns the DateTime object from the topicID.
        /// </returns>
        public DateTime GetTopicRead(int topicId, DateTime? readTimeOverride = null)
        {
            DateTime? readTime;

            if (this.UseDatabaseReadTracking)
            {
                if (readTimeOverride.HasValue)
                {
                    // use it if it's not the min value...
                    readTime = readTimeOverride.Value > DateTimeHelper.SqlDbMinTime()
                                   ? readTimeOverride.Value
                                   : this.GetRepository<TopicReadTracking>().Lastread(this.CurrentUserId, topicId);
                }
                else
                {
                    // last option is to load from the forum...
                    readTime = this.GetRepository<TopicReadTracking>().Lastread(this.CurrentUserId, topicId);
                }
            }
            else
            {
                readTime = this.GetSessionTopicRead(topicId);

                if (!readTime.HasValue)
                {
                    // use the last visit...
                    readTime = this.LastRead;
                }
            }

            return readTime ?? DateTimeHelper.SqlDbMinTime();
        }

        /// <summary>
        ///     The set forum read.
        /// </summary>
        /// <param name="forumId">The forum ID of the Forum</param>
        public void SetForumRead(int forumId)
        {
            if (this.UseDatabaseReadTracking)
            {
                this.GetRepository<ForumReadTracking>().AddOrUpdate(this.CurrentUserId, forumId);
            }
            else
            {
                this.Get<IYafSession>().SetForumRead(forumId, DateTime.UtcNow);
            }
        }

        /// <summary>
        ///     The set topic read.
        /// </summary>
        /// <param name="topicId">The topic id to mark read.</param>
        public void SetTopicRead(int topicId)
        {
            if (this.UseDatabaseReadTracking)
            {
                this.GetRepository<TopicReadTracking>().AddOrUpdate(this.CurrentUserId, topicId);
            }
            else
            {
                this.Get<IYafSession>().SetTopicRead(topicId, DateTime.UtcNow);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the session forum read.
        /// </summary>
        /// <param name="forumId">The forum id.</param>
        /// <returns>
        ///     The get session forum read.
        /// </returns>
        private DateTime? GetSessionForumRead(int forumId)
        {
            var forumReadHashtable = this.Get<IYafSession>().ForumRead;

            if (forumReadHashtable != null && forumReadHashtable.ContainsKey(forumId))
            {
                return forumReadHashtable[forumId].ToType<DateTime>();
            }

            return null;
        }

        /// <summary>
        ///     Gets the session topic read.
        /// </summary>
        /// <param name="topicId">The topic id.</param>
        /// <returns>
        ///     The get session topic read.
        /// </returns>
        private DateTime? GetSessionTopicRead(int topicId)
        {
            var topicReadHashtable = this.Get<IYafSession>().TopicRead;

            if (topicReadHashtable != null && topicReadHashtable.ContainsKey(topicId))
            {
                return topicReadHashtable[topicId].ToType<DateTime>();
            }

            return null;
        }

        #endregion
    }
}