using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    // Never use this Id to get the object on an frame update
    public Player(string id, string cameraId)
    {
        Id = id;
        CameraId = cameraId;
    }
    public string Id { get; set; }
    public string CameraId { get; set; }
    public Teams Team { get; set; } 
}
