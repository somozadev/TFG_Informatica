using UnityEngine;

namespace General
{
    public static class EventManager
    {
        public delegate void OnEconomyUpdatedEventHandler();

        public static event OnEconomyUpdatedEventHandler EconomyUpdated;

        public delegate void OnRewardClaimedEventHandler();

        public static event OnRewardClaimedEventHandler RewardClaimed;

        public delegate void OnTimerStartedEventHandler();

        public static event OnTimerStartedEventHandler TimerStarted;

        public delegate void OnTimerEndedEventHandler();

        public static event OnTimerEndedEventHandler TimerEnded;

        public delegate void OnLoadingStartsEventHandler();

        public delegate void OnLoadingEndsEventHandler();

        public static event OnLoadingEndsEventHandler LoadingEnds;
        public static event OnLoadingStartsEventHandler LoadingStarts;

        public static void OnEconomyUpdated()
        {
            EconomyUpdated?.Invoke();
        }

        public static void OnRewardClaimed()
        {
            RewardClaimed?.Invoke();
        }

        public static void OnTimerStarted()
        {
            TimerStarted?.Invoke();}

        public static void OnTimerEnded()
        {
            TimerEnded?.Invoke();
        }

        public static void OnLoadingEnds()
        {
            LoadingEnds?.Invoke();
        }

        public static void OnLoadingStarts()
        {
            LoadingStarts?.Invoke();
        }
    }
}