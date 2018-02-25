﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.mytopics" Codebehind="mytopics.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="MyTopicsList" Src="../controls/MyTopicsList.ascx" %>
<%@ Register tagPrefix="YAF" namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>

<br style="clear: both" />
       <asp:Panel id="TopicsTabs" runat="server">
               <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#ActiveTopicsTab" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ActiveTopics" LocalizedPage="MyTopics" />
                     </a>
                 </li>
                 <asp:PlaceHolder ID="UnansweredTopicsTabTitle" runat="server">
                 <li class="nav-item">
                     <a href="#UnansweredTopicsTab" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="UnansweredTopics" LocalizedPage="MyTopics" />
                     </a>
                 </li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="UnreadTopicsTabTitle" runat="server">
                   <li class="nav-item">
                       <a href="#UnreadTopicsTab" class="nav-link" data-toggle="tab" role="tab">
                           <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="UnreadTopics" LocalizedPage="MyTopics" />
                       </a>
                   </li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="UserTopicsTabTitle" runat="server">
                   <li class="nav-item">
                       <a href="#MyTopicsTab" class="nav-link" data-toggle="tab" role="tab">
                           <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MyTopics" LocalizedPage="MyTopics" />
                       </a>
                   </li>
		           <li class="nav-item">
		               <a href="#FavoriteTopicsTab" class="nav-link" data-toggle="tab" role="tab">
		                   <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FavoriteTopics" LocalizedPage="MyTopics" />
		               </a>
		           </li>
                 </asp:PlaceHolder>
               </ul>
              <div class="tab-content">
                <div id="ActiveTopicsTab" class="tab-pane" role="tabpanel">
                   <YAF:MyTopicsList runat="server" ID="ActiveTopics" CurrentMode="Active" AutoDatabind="True"/>
                </div>
                <asp:PlaceHolder ID="UnansweredTopicsTabContent" runat="server">
                <div id="UnansweredTopicsTab" class="tab-pane" role="tabpanel">
                   <YAF:MyTopicsList runat="server" ID="UnansweredTopics" CurrentMode="Unanswered" AutoDatabind="False"/>
                </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="UnreadTopicsTabContent" runat="server">
                <div id="UnreadTopicsTab" class="tab-pane" role="tabpanel">
                   <YAF:MyTopicsList runat="server" ID="UnreadTopics" CurrentMode="Unread" AutoDatabind="False" />
                </div>
                </asp:PlaceHolder>
                 <asp:PlaceHolder ID="UserTopicsTabContent" runat="server">
                <div id="MyTopicsTab" class="tab-pane" role="tabpanel">
                   <YAF:MyTopicsList runat="server" ID="MyTopics" CurrentMode="User" />
                </div>
                <div id="FavoriteTopicsTab" class="tab-pane" role="tabpanel">
                   <YAF:MyTopicsList runat="server" ID="FavoriteTopics" CurrentMode="Favorite" AutoDatabind="False" />
                </div>
                </asp:PlaceHolder>
                  </div>
             </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="ActiveTopicsTab" />
        <asp:Button id="ChangeTab" OnClick="ChangeTabClick" runat="server" style="display:none" />
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div class="float-right">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div class="float-left">
    <YAF:IconLegend ID="IconLegend1" runat="server" />
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
