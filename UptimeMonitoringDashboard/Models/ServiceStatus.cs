namespace UptimeMonitor.Models
{
    public class ServiceStatus
    {
        public string? Name { get; set; }
        public string? IsUp { get; set; }
        public string? LastChecked { get; set; }
        public string? ResponseTime { get; set; }

        public ServiceStatus() { }
        public ServiceStatus(string name, string isup, string lastchecked, string responsetime)
        {

            Name = name;
            if (isup == "1")
            {
                IsUp = "true";
            }
            else if (isup == "2")
            {
                IsUp = "false";
            }
            else
            {
                IsUp = "Unkown";
            }
            LastChecked = lastchecked;
            ResponseTime = responsetime;
        }
    }
}