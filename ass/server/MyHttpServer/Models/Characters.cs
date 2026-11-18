using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyHttpServer.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public string Comment { get; set; } = "";

        public Character() { }

        public Character(int id, string name, string imageUrl, string description, string comment)
        { 
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            Description = description;
            Comment = comment;
        }
    }
}
