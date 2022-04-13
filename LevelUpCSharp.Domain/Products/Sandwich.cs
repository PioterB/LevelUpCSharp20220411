using System;
using LevelUpCSharp.Collections;

namespace LevelUpCSharp.Products
{
    public class Sandwich : IKindable<SandwichKind>
    {
        public Sandwich()
        {
        }

        public SandwichKind Kind { get; set; }
    
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
