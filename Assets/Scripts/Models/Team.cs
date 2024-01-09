using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
