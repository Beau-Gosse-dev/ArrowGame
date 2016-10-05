using System;

namespace Assets.Networking
{
    class Match : ICloneable
    {
        //TODO: Clean up (and rename?) this class
        public string friendName;
        public string friendId;
        public string matchId;
        public float leftHealth;
        public float rightHealth;

        public object Clone()
        {
            Match clonedMatch = new Match();
            clonedMatch.friendName = friendName;
            clonedMatch.friendId = friendId;
            clonedMatch.matchId = matchId;
            clonedMatch.leftHealth = leftHealth;
            clonedMatch.rightHealth = rightHealth;

            return clonedMatch;
        }
    }
}
