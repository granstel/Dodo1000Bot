namespace Dodo1000Bot.Models.PublicApi
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DepartmentType Type { get; set; }

        public DepartmentState State { get; set; }

        public object[] SocialNetworkLinks { get; set; }
    }
}
