using System.ComponentModel.DataAnnotations;
using System;
using PRBD_Framework;

namespace MyPoll.Model;

public class Admin : User{
    public Admin() { }

    public Admin(int UserId, string Name, string Mail , string Password) {}

}
