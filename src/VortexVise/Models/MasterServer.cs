using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Models;

public class MasterServer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ServerName { get; set; } = "";
    public string ServerURL { get; set; } = "";
    public string ServerUDP { get; set; } = "";
}
