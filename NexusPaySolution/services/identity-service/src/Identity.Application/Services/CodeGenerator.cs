using Identity.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services
{
    public class CodeGenerator : ICodeGenerator
    {
        public CodeGenerator()
        {
            _random = new Random();
        }

        private readonly Random _random;

        public int GenerateCode()
        {
            return _random.Next(1256, 9876);
        }
    }
}
