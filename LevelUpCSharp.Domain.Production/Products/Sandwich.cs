using System;
using LevelUpCSharp.Collections;

namespace LevelUpCSharp.Products
{
    public class Sandwich : IKindable<SandwichKind>
    {
        private readonly SandwichKind _kind;
        private readonly DateTimeOffset _expirationDate;
        private readonly string[] _ingredients;

        public Sandwich(SandwichKind kind, DateTimeOffset expirationDate, params string[] ingredients)
        {
            _kind = kind;
            _expirationDate = expirationDate;
            _ingredients = ingredients;
        }

        public SandwichKind Kind => _kind;

        public DateTimeOffset ExpirationDate => _expirationDate;

        public int IngredientsCount => _ingredients.Length;
    }
}
