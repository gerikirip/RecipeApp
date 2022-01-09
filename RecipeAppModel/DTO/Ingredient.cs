using System;

namespace RecipeAppModel
{
    public class Ingredient:IComparable<Ingredient>
    {
        public string Name;
        public double Amount;
        public string Unit;

        int IComparable<Ingredient>.CompareTo(Ingredient other)
        {
            return String.Compare(this.Name,other.Name);
        }

    }
}