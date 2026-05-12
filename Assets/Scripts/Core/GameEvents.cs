using Assets.Scripts.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core
{
    public enum GameResult { Win, Lose }
    public static class GameEvents
    {
        public static event Action<GameResult> OnGameFinished;
        public static event Action OnGameReset;
        public static event Action<CellView, CellData> OnPlayerHitMine;
        public static event Action<bool> OnFullscreenAdActive;
        public static event Action OnInfoShow;
        public static event Action OnInfoHide;

        public static void RaiseGameFinished(GameResult result) => OnGameFinished?.Invoke(result);
        public static void RaiseGameReset() => OnGameReset?.Invoke();
        public static void RaisePlayerHitMine(CellView cell, CellData data) => OnPlayerHitMine?.Invoke(cell, data);
        public static void RaiseFullscreenAdActive(bool active) => OnFullscreenAdActive?.Invoke(active);
        public static void RaiseInfoShow() => OnInfoShow?.Invoke();
        public static void RaiseInfoHide() => OnInfoHide?.Invoke();
    }
}
