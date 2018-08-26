using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GuideHaven.Models.GuideController;

namespace GuideHaven.Models
{
    public class CommentsHub : Hub
    {
        public Task JoinGroup(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public Task LeaveGroup(int id)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
        }

        public Task AddComment(int id, CommentOutput comment)
        {
            return Clients.Group(id.ToString()).SendAsync("addcomment", comment);
        }

    }
}
