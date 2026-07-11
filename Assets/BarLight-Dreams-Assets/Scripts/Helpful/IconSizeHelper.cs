using UnityEngine;

public static class IconSizeHelper
{
    public static Vector2 GetDrinkSize(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.CherryVelvet:
            case DrinkType.GreenVelvet:
            case DrinkType.TequilaSunrise:
            case DrinkType.ThePinkLady:
            case DrinkType.AmberSparkle:
            case DrinkType.LemonZest:
            case DrinkType.LotusDream:
            case DrinkType.TropicalWave:
                return new Vector2(60, 60);

            case DrinkType.ChocoCherryPop:
            case DrinkType.GoldenHour:
            case DrinkType.GreenHornet:
            case DrinkType.GinAndTonic:
            case DrinkType.AmberMoonlight:
            case DrinkType.AuroraCloud:
            case DrinkType.JadeBreeze:
            case DrinkType.MysticMartini:
            case DrinkType.CitrusMargarita:
            case DrinkType.GreenOasis:
            case DrinkType.MidnightOrchid:
            case DrinkType.VelvetKiss:
                return new Vector2(50, 50);

            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector2 GetDrinkRecipeSize(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.CherryVelvet:
            case DrinkType.GreenVelvet:
            case DrinkType.TequilaSunrise:
            case DrinkType.ThePinkLady:
                return new Vector2(70, 90);

            case DrinkType.ChocoCherryPop:
            case DrinkType.GoldenHour:
            case DrinkType.GreenHornet:
            case DrinkType.GinAndTonic:
                return new Vector2(60, 80);

            case DrinkType.AmberSparkle:
            case DrinkType.LemonZest:
            case DrinkType.LotusDream:
                return new Vector2(45, 90);

            case DrinkType.TropicalWave:
                return new Vector2(70, 90);

            case DrinkType.AmberMoonlight:
            case DrinkType.AuroraCloud:
            case DrinkType.JadeBreeze:
            case DrinkType.MysticMartini:
            case DrinkType.CitrusMargarita:
            case DrinkType.GreenOasis:
            case DrinkType.MidnightOrchid:
            case DrinkType.VelvetKiss:
                return new Vector2(80, 90);

            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector2 GetDrinkOrderSize(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.CherryVelvet:
            case DrinkType.GreenVelvet:
            case DrinkType.TequilaSunrise:
            case DrinkType.ThePinkLady:
                return new Vector2(50, 70);

            case DrinkType.ChocoCherryPop:
            case DrinkType.GoldenHour:
            case DrinkType.GreenHornet:
            case DrinkType.GinAndTonic:
                return new Vector2(40, 70);

            case DrinkType.AmberMoonlight:
            case DrinkType.AuroraCloud:
            case DrinkType.JadeBreeze:
            case DrinkType.MysticMartini:

            case DrinkType.CitrusMargarita:
            case DrinkType.GreenOasis:
            case DrinkType.MidnightOrchid:
            case DrinkType.VelvetKiss:
                return new Vector2(60, 70);

            case DrinkType.AmberSparkle:
            case DrinkType.LemonZest:
            case DrinkType.LotusDream:
                return new Vector2(45, 70);

            case DrinkType.TropicalWave:
                return new Vector2(50, 70);

            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector2 GetIngredientSize(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Ice:
                return new Vector2(40, 50);

            case IngredientType.Peach:
            case IngredientType.Strawberry:
            case IngredientType.Cherry:
            case IngredientType.Apple:
            case IngredientType.Lemon:
            case IngredientType.Grape:
            case IngredientType.Orange:
            case IngredientType.Pineapple:
                return new Vector2(40, 50);

            case IngredientType.Mystic_Absinthe:
            case IngredientType.Verdant_Bite:
            case IngredientType.Obsidian_Gin:
            case IngredientType.Radiant_Dew:
            case IngredientType.Golden_Rum_Rush:
            case IngredientType.Golden_Gin:
            case IngredientType.Ivory_Bloom:
            case IngredientType.Vanilla_Crash:
            case IngredientType.Honey_Flame:
            case IngredientType.Azuze_Spirit:
            case IngredientType.Citrus_Ember:
            case IngredientType.Pink_Lady:
                return new Vector2(25, 60);

            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector2 GetOrderRecipeSize(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.CherryVelvet:
            case DrinkType.GreenVelvet:
            case DrinkType.TequilaSunrise:
            case DrinkType.ThePinkLady:
            
            case DrinkType.ChocoCherryPop:
            case DrinkType.GinAndTonic:
            case DrinkType.GoldenHour:
            case DrinkType.GreenHornet:

            case DrinkType.TropicalWave:
                return new Vector2(110, 150);
            
            case DrinkType.AmberSparkle:
            case DrinkType.LemonZest:
            case DrinkType.LotusDream:
                return new Vector2(75, 150);
            
            case DrinkType.AmberMoonlight:
            case DrinkType.AuroraCloud:
            case DrinkType.JadeBreeze:
            case DrinkType.MysticMartini:
            case DrinkType.CitrusMargarita:
            case DrinkType.GreenOasis:
            case DrinkType.MidnightOrchid:
            case DrinkType.VelvetKiss:
                return new Vector2(130, 130);
            
            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector2 GetOrderIngredientSize(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Ice:
            
            case IngredientType.Peach:
            case IngredientType.Strawberry:
            case IngredientType.Cherry:
            case IngredientType.Apple:
            case IngredientType.Lemon:
            case IngredientType.Grape:
            case IngredientType.Orange:
            case IngredientType.Pineapple:
                return new Vector2(50,50);


            case IngredientType.Mystic_Absinthe:
                return new Vector2(35, 60);

            case IngredientType.Verdant_Bite:
            case IngredientType.Obsidian_Gin:
            case IngredientType.Radiant_Dew:
            case IngredientType.Golden_Rum_Rush:
            case IngredientType.Golden_Gin:
            case IngredientType.Ivory_Bloom:
            case IngredientType.Vanilla_Crash:
            case IngredientType.Honey_Flame:
            case IngredientType.Azuze_Spirit:
            case IngredientType.Citrus_Ember:
            case IngredientType.Pink_Lady:
                return new Vector2(35, 80);

            default:
                return new Vector2(0,  0);
        }
    }
}
