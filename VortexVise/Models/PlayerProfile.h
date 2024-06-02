#include "iostream"


class PlayerProfile
{
public: 
    public std::string Id;
    public int Gamepad { get; set; } = -9;
    public string Name { get; set; } = string.Empty;
    public Skin Skin { get; set; } = new Skin() { Id = "" };
};