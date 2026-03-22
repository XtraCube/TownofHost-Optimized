using static TOHO.Options;

namespace TOHO.Roles.AddOns.Common;

public class Rookie : IAddon
{
    public CustomRoles Role => CustomRoles.Rookie;
    private const int Id = 38800;
    public AddonTypes Type => AddonTypes.Harmful;
    public static OptionItem RookieChance;
    public void SetupCustomOption()
    {
        SetupAdtRoleOptions(Id, CustomRoles.Rookie, canSetNum: true, teamSpawnOptions: true);
        RookieChance = IntegerOptionItem.Create(Id + 10, "RookieChance", (5, 100, 5), 50, TabGroup.Addons, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Rookie])
            .SetValueFormat(OptionFormat.Percent);
    }
    public void Init()
    { }
    public void Add(byte playerId, bool gameIsLoading = true)
    { }
    public void Remove(byte playerId)
    { }

    public static void OnTaskComplete(PlayerControl player)
    {
        var rand = IRandom.Instance;

        if (rand.Next(1, 100) <= RookieChance.GetInt()) player.RpcResetTasks();
        return;
    }
}
