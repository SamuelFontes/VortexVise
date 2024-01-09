
public class Team
{
    public Teams TeamLayer { get; private set; }
    public int NumberOfActors { get; private set; }
    public Team(Teams team)
    {
        TeamLayer = team;
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
}
