using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Level
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public GameObject LevelPrefab { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeathMatch { get; set; }
    public bool IsCoop { get; set; }
}
