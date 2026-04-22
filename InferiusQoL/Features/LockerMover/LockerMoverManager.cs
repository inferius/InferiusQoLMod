namespace InferiusQoL.Features.LockerMover;

using System;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Globalni MonoBehaviour na DontDestroyOnLoad host GO. V Update() detekuje
/// keybind + cil hracova cursoru. Pokud cursor miri na podporovany StorageContainer
/// a clipboard je v odpovidajicim stavu (prazdny -> grab plne skrine; plny ->
/// place do prazdne skrine stejneho techType), provede akci.
///
/// Vanilla vyber co lze stehovat:
/// - Locker (velky locker)
/// - SmallLocker (Wall Locker)
/// - SmallStorage (Waterproof Locker - carryable, ale lze i na stene)
/// - LuggageBag (Carryall)
///
/// Vehicle storage, Nuclear Reactor, Bioreactor a dalsi specialni kontejnery
/// nejsou podporovany (specialni logika nebo fixed umisteni).
/// </summary>
public class LockerMoverManager : MonoBehaviour
{
    private const float DUPLICATE_PRESS_COOLDOWN = 0.3f;
    private float _lastActionTime = -1f;

    private void Update()
    {
        try
        {
            var cfg = InferiusConfig.Instance;
            if (!cfg.LockerMoverEnabled) return;

            var player = Player.main;
            if (player == null) return;

            if (!TryParseKey(cfg.LockerMoverKey, out var key)) return;
            if (!UnityEngine.Input.GetKeyDown(key)) return;

            if (Time.time - _lastActionTime < DUPLICATE_PRESS_COOLDOWN) return;

            if (cfg.LockerMoverRequireEmptyHands && !HasEmptyHands(player))
            {
                ShowToast("Locker Mover: odlozte nastroj z ruky");
                return;
            }

            var target = ResolveHoverTarget(player);
            if (target == null) return;

            _lastActionTime = Time.time;
            HandleAction(target);
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.LockerMover, "Update threw", ex);
        }
    }

    private static bool HasEmptyHands(Player player)
    {
        var inv = Inventory.main;
        if (inv == null) return true;
        return inv.GetHeldObject() == null;
    }

    private static bool TryParseKey(string name, out KeyCode key)
    {
        if (Enum.TryParse<KeyCode>(name, ignoreCase: true, out key)) return true;
        key = KeyCode.G;
        return false;
    }

    private static HoverTarget? ResolveHoverTarget(Player player)
    {
        var hand = player.guiHand;
        if (hand == null) return null;

        var active = hand.activeTarget;
        if (active == null) return null;

        var sc = active.GetComponentInParent<StorageContainer>();
        if (sc == null) return null;
        if (sc.container == null) return null;

        var techType = ResolveTechType(sc);
        if (!IsSupported(techType)) return null;

        return new HoverTarget
        {
            Container = sc,
            TechType = techType,
        };
    }

    private static bool IsSupported(TechType tt) =>
        tt == TechType.Locker
        || tt == TechType.SmallLocker
        || tt == TechType.SmallStorage
        || tt == TechType.LuggageBag;

    private static TechType ResolveTechType(StorageContainer sc)
    {
        var go = sc.gameObject;

        var techTag = go.GetComponent<TechTag>() ?? go.GetComponentInParent<TechTag>();
        if (techTag != null && techTag.type != TechType.None) return techTag.type;

        var pickupable = go.GetComponent<Pickupable>();
        if (pickupable != null)
        {
            var tt = pickupable.GetTechType();
            if (tt != TechType.None) return tt;
        }

        var constructable = go.GetComponent<Constructable>() ?? go.GetComponentInParent<Constructable>();
        if (constructable != null && constructable.techType != TechType.None) return constructable.techType;

        return TechType.None;
    }

    private void HandleAction(HoverTarget target)
    {
        bool clipboardEmpty = LockerMoverClipboard.IsEmpty;
        bool containerEmpty = target.Container.container.count == 0;

        // Rozhodovaci tabulka:
        //   clipboard | container | akce
        //   empty     | empty     | nothing (silent)
        //   empty     | full      | Grab
        //   full      | empty     | Place (jen stejny techType)
        //   full      | full      | nothing (toast: nejdriv place jinde nebo zrus)

        if (clipboardEmpty && containerEmpty) return;

        if (clipboardEmpty && !containerEmpty)
        {
            DoGrab(target);
            return;
        }

        if (!clipboardEmpty && containerEmpty)
        {
            DoPlace(target);
            return;
        }

        ShowToast($"Locker Mover: clipboard obsazen ({LockerMoverClipboard.ItemCount} itemu, {LockerMoverClipboard.SourceTechType}). Vyprazdnete ho nejdriv.");
    }

    private static void DoGrab(HoverTarget target)
    {
        var sc = target.Container;
        int count = LockerMoverClipboard.Grab(sc.container, target.TechType, sc.width, sc.height);
        if (count > 0)
            ShowToast($"Locker Mover: {count} itemu presunuto do clipboardu");
    }

    private static void DoPlace(HoverTarget target)
    {
        if (LockerMoverClipboard.SourceTechType != target.TechType)
        {
            ShowToast($"Locker Mover: jiny typ skrine (clipboard: {LockerMoverClipboard.SourceTechType}, target: {target.TechType})");
            return;
        }

        int before = LockerMoverClipboard.ItemCount;
        bool allPlaced = LockerMoverClipboard.Place(target.Container.container);
        int after = LockerMoverClipboard.ItemCount;
        int placed = before - after;

        if (allPlaced)
            ShowToast($"Locker Mover: {placed} itemu umisteno");
        else
            ShowToast($"Locker Mover: umisteno {placed}/{before}, {after} zbyva (malo mista?)");
    }

    private static void ShowToast(string msg)
    {
        try { ErrorMessage.AddMessage(msg); }
        catch { }
        QoLLog.Info(Category.LockerMover, msg);
    }

    private sealed class HoverTarget
    {
        public StorageContainer Container = null!;
        public TechType TechType;
    }
}
