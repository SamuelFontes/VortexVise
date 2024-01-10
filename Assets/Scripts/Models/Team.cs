
public class Team
{
    public TeamLayer TeamLayer { get; private set; }
    public ProjectileTeamLayer ProjectileTeamLayer { get; private set; }
    public int NumberOfActors { get; private set; }
    public Team(TeamLayer teamLayer,  ProjectileTeamLayer projectileTeamLayer)
    {
        TeamLayer = teamLayer;
        ProjectileTeamLayer = projectileTeamLayer;
        NumberOfActors = 0;
    }

    public void AddActorToTeam()
    {
        NumberOfActors++;
    }
    public void RemoveActorFromTeam()
    {
        NumberOfActors--;
    }
    public int GetTeamLayer()
    {
        return (int)TeamLayer;
    }
    public int GetProjectileTeamLayer()
    {
        return (int)ProjectileTeamLayer;
    }
    public void AddPlayerToTeam(Player player)
    {
        player.SetPlayerTeam(this);
        // Set the layer on unity
        player.gameObject.layer = GetTeamLayer();
        AddActorToTeam();
    }
}
