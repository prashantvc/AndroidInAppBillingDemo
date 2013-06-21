package com.xamarin.InAppBillingLibrary;

/**
 * Created with IntelliJ IDEA.
 * User: prashantvc
 * Date: 21/06/13
 * Time: 2:08 PM
 * */
public final class ItemType {

    private ItemType() {}

    public static String getInApp()   {
        return "inapp";
    }

    public static String getSubscription()
    {
        return "subs";
    }
}
