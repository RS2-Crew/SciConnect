using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Application.Features.Keywords.Queries.ViewModels
{
    public class KeywordViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public KeywordViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public KeywordViewModel()
        {
        }
    }
}
