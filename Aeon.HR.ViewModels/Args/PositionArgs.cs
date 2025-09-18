namespace Aeon.HR.ViewModels.Args
{
    public class PositionArgs: QueryArgs
    {
        public PositionArgs()
        {
            Order = "";
            Page = 0;
            Limit = 10;

            HasApplicants = false;
            Assignment = true;
        }

        public string Keyword { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public bool HasApplicants { get; set; }
        public bool Assignment { get; set; }
        public string StatusCode { get; set; }
    }
}