using Microsoft.AspNetCore.SignalR;


public class MessageHub : Hub
{
    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();
    private static Dictionary<string, List<string>> groupMembers = new Dictionary<string, List<string>>();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (!userConnections.ContainsKey(userId))
        {
            userConnections.Add(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;

        if (userConnections.ContainsKey(userId))
        {
            userConnections.Remove(userId);
        }

        // Remove the user from any groups they might be part of
        foreach (var group in groupMembers.Keys)
        {
            if (groupMembers[group].Contains(userId))
            {
                groupMembers[group].Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public void SetUserId(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var connectionId = Context.ConnectionId;

            // Update the user ID associated with this connection
            userConnections[userId] = connectionId;
        }
    }

    public async Task SendMessageToUser(string targetUserId, string message)
    {
        if (userConnections.TryGetValue(targetUserId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }

    public async Task SendMessageToGroup(string groupId, string message)
    {
        if (groupMembers.TryGetValue(groupId, out var members))
        {
            await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", message);
        }
    }

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        if (!groupMembers.ContainsKey(groupId))
        {
            groupMembers.Add(groupId, new List<string>());
        }

        groupMembers[groupId].Add(Context.UserIdentifier);

        // Notify group members about the new user joining
        await SendMessageToGroup(groupId, $"{Context.UserIdentifier} has joined the group.");
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);

        if (groupMembers.ContainsKey(groupId))
        {
            groupMembers[groupId].Remove(Context.UserIdentifier);

            // Notify group members about the user leaving
            await SendMessageToGroup(groupId, $"{Context.UserIdentifier} has left the group.");

            // Remove the group if there are no more members
            if (groupMembers[groupId].Count == 0)
            {
                groupMembers.Remove(groupId);
            }
        }
    }

    // Other hub methods...
}
