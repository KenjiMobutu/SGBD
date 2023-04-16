namespace MyPoll.Model;

public class Admin : User{
    public Admin() {
        Role = Role.Admin;
    }

    public Admin(int userId,string name, string mail , string password)
       :base(userId,name,mail,password) {
        Role = Role.Admin;
    }
    

}
