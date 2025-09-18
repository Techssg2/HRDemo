namespace Aeon.Academy.Common.Workflow
{
    public enum Right
    {
        None = 0,
        View = 1,
        Edit = 2,
        Delete = 4,
        Full = View | Edit | Delete
    }
}
