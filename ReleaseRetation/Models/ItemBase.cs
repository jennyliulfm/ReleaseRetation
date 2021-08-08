namespace ReleaseRetation.Models
{
    public class ItemBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ItemBase()
        {
        }

        public ItemBase(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return $"Id: {this.Id} Name: {this.Name}";
        }

    }
}