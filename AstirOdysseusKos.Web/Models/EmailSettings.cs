namespace AstirOdysseusKos.Web.Models
{
    public class EmailSettings
    {
        public string From { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<string> mailTo { get; set; }
        public List<string> mailToNewsletter { get; set; }
        public List<string> mailToRequest { get; set; }
        public List<string> mailToCareer { get; set; }
        

        public string nameSubject { get; set; }
    }
}
