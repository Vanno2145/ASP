using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();

app.MapWhen(
  context => context.Request.Path == "/",
  appBuilder => appBuilder.Run(async context =>
  {
      var users = context.RequestServices.GetService<IUserRepository>();
      var st = new StringBuilder();
      st.Append("<table><tr><th>Id</th><th>Name</th><th>Age</th><th></th><th>Edit</th><th>Delete</th></tr>");
      foreach (var item in users.GetAllUsers())
      {
          st.Append($"<tr><td>{item.Id}</td><td>{item.Name}</td><td>{item.Age}</td><td><a href='/getUser?id={item.Id}'>Get</a></td><td><a href='/editUser?id={item.Id}'>Edit</a></td><td><a href='/deleteUser?id={item.Id}'>Delete</a></td></tr>");
      }
      st.Append("</table>");
      context.Response.ContentType = "text/html; charset=utf-8";
      await context.Response.WriteAsync("<ul><li><a href='\\'>Index Page</a></li><li><a href='\\addUser'>Add User</a></li></ul>" +
        "<div><h2>All users: </h2></div>" +
        "<div>" + st.ToString() + "</div>");
  })
);

app.MapWhen(
  context => context.Request.Path == "/addUser" && context.Request.Method == "GET",
  appBuilder => appBuilder.Run(async context =>
  {
      context.Response.ContentType = "text/html; charset=utf-8";
      await context.Response.WriteAsync("<h2>Add User</h2><form action=\"/addUser\" method=\"post\"><label for=\"name\">Name:</label><br><input type=\"text\" id=\"name\" name=\"Name\" required><br><label for=\"age\">Age:</label><br><input type=\"number\" id=\"age\" name=\"Age\" required><br><br><button type=\"submit\">Add</button></form>");
  })
);

app.MapWhen(
  context => context.Request.Path == "/editUser" && context.Request.Method == "GET",
  appBuilder => appBuilder.Run(async context =>
  {
      context.Response.ContentType = "text/html; charset=utf-8";
      var users = context.RequestServices.GetService<IUserRepository>();
      string strId = context.Request.Query["id"];
      if (int.TryParse(strId, out int id))
      {
          var user = users.GetUser(id);
          if (user != null)
          {
              await context.Response.WriteAsync("<h2>Edit User</h2><form action=\"/editUser\" method=\"post\"><input type=\"hidden\" name=\"Id\" value=\"" + user.Id + "\" /><label for=\"name\">Name:</label><br /><input type=\"text\" id=\"name\" name=\"Name\" value=\"" + user.Name + "\" required /><br /><label for=\"age\">Age:</label><br /><input type=\"number\" id=\"age\" name=\"Age\" value=\"" + user.Age + "\" required /><br /><br /><button type=\"submit\">Save</button></form>");
          }
          else
          {
              await context.Response.WriteAsync($"<h2>Can't find user with id - {strId}</h2>");
          }
      }
  })
);


app.Run();

interface IUserRepository
{
    void AddUser(User user);
    void UpdateUser(User user);
    User GetUser(int id);
    void DeleteUser(int id);
    List<User> GetAllUsers();
    int GetLastId();
}

class UserRepository : IUserRepository
{
    private List<User> users;

    public UserRepository() => users = new List<User>();

    public void AddUser(User user)
    {
        users.Add(user);
    }

    public void UpdateUser(User user)
    {
        var currentUser = users.FirstOrDefault(e => e.Id == user.Id);
        if (currentUser != null)
        {
            currentUser.Name = user.Name;
            currentUser.Age = user.Age;
        }
    }

    public void DeleteUser(int id)
    {
        users = users.Where(e => e.Id != id).ToList();
    }


    public List<User> GetAllUsers()
    {
        return users;
    }

    public int GetLastId()
    {
        return users.Count > 0 ? users[users.Count - 1].Id : 1;
    }

    public User GetUser(int id)
    {
        return users.FirstOrDefault(e => e.Id == id);
    }
}


class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }

    public override string ToString()
    {
        return $"Id - {Id}, Name - {Name}, Age - {Age}";
    }
}



