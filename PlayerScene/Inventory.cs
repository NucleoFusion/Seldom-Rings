using Godot;

namespace SeldomRings.PlayerScene;

public class Inventory
{
  [Export] public Item[] Items;
  [Export] public int InventorySize;
}