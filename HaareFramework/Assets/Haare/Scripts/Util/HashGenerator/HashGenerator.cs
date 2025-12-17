namespace Haare.Scripts.Util.HashGenerator
{
    public static class HashGenerator
    {
        public static int GetUniqueHashCode()
        {
            string UniqueId = System.Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(UniqueId))
            {
                return 0;
            }
            return UniqueId.GetHashCode();
        }
    }
}