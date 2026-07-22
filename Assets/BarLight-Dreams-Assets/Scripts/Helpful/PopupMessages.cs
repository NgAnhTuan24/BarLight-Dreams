using UnityEngine;

public static class PopupMessages
{
    #region Drink Mixer Messages

    private static readonly string[] mixSuccessMessages =
    {
        "Perfect!",
        "Excellent!",
        "Great Mix!",
        "Looks Delicious!",
        "Nicely Done!",
        "Recipe Complete!",
        "Fantastic!",
        "That's the One!",
        "Smooth Mix!",
        "Drink Ready!"
    };

    private static readonly string[] mixFailMessages =
    {
        "Wrong Recipe!",
        "Oops!",
        "That Didn't Work!",
        "Recipe Failed!",
        "Wrong Ingredients!",
        "Try Again!",
        "Not Quite Right!",
        "The Mix Is Off!",
        "Something Went Wrong!",
        "Better Luck Next Time!"
    };

    public static string GetSuccessMessage()
    {
        return GetRandom(mixSuccessMessages);
    }

    public static string GetFailMessage()
    {
        return GetRandom(mixFailMessages);
    }

    #endregion

    #region Customer Messages

    private static readonly string[] thanksMessages =
    {
        "Thanks!",
        "Delicious!",
        "Perfect!",
        "Wonderful!",
        "Exactly What I Wanted!",
        "Excellent!",
        "Love It!",
        "Tastes Great!",
        "Amazing!",
        "You're Awesome!"
    };

    private static readonly string[] slowThanksMessages =
    {
        "Too Slow...",
        "Finally...",
        "That Took A While.",
        "Better Late Than Never.",
        "I Almost Left."
    };

    private static readonly string[] angryMessages =
    {
        "Too Slow!",
        "I'm Leaving!",
        "Terrible Service!",
        "I've Waited Long Enough!",
        "Forget It!"
    };

    private static readonly string[] wrongDrinkMessages =
    {
        "Wrong Drink!",
        "That's Not Mine!",
        "I Didn't Order This!",
        "Wrong Order!",
        "This Isn't What I Wanted!"
    };

    public static string GetThanksMessage()
    {
        return GetRandom(thanksMessages);
    }

    public static string GetSlowMessage()
    {
        return GetRandom(slowThanksMessages);
    }

    public static string GetAngryMessage()
    {
        return GetRandom(angryMessages);
    }

    public static string GetWrongDrinkMessage()
    {
        return GetRandom(wrongDrinkMessages);
    }

    public static string GetTipMessage(int amount)
    {
        string[] tipMessages =
        {
            $"Thanks! +{amount} Tip!",
            $"Keep The Change! +{amount}",
            $"Excellent Service! +{amount}",
            $"You Earned +{amount}!",
            $"Great Job! +{amount}"
        };

        return GetRandom(tipMessages);
    }

    #endregion

    private static string GetRandom(string[] messages)
    {
        return messages[Random.Range(0, messages.Length)];
    }
}
