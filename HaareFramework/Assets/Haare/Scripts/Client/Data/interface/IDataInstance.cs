namespace Haare.Scripts.Client.Data
{
    public interface IDataInstance
    {
        public int Hash { get; set; }
        void Save();
    }
}